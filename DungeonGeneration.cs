using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGeneration : MonoBehaviour {
	Grid g = new Grid();
	[Range(1,1000)]
	public int gridWidth;
	[Range(1,1000)]
	public int gridHeight;
	public bool overlappingRooms; 
	int tryCount=0;
	public GameObject wall;
	[Range(1,500)]
	public int rooms;
	[Range(1,500)]
	public int minRoomSize;
	[Range(100,250000)]
	public int maxRoomSize;
	[Range(5,500)]
	public int maxWidth;
	[Range(5,500)]
	public int maxHeight;
	[Range(5,250)]
	public int minWidth;
	[Range(5,250)]
	public int minHeight;
	public bool AddLights = false;
	public GameObject Light;
	[Range(0,1)]
	public float brightness;
	public bool spawnObjects;
	public Spawn[] objectsToSpawn;
	public GameObject playerSpawn;
	int spawn =0;
	int spawnNum;
	int numRooms;
	int room = 0;
	List<Vector2> spawnableArea = new List<Vector2>();
	List<Rect> doneRooms = new List<Rect> ();
	// Use this for initialization
	void Start () 
	{
		Time.timeScale = 0;
		g.Initialize (rooms,gridWidth,gridHeight,!overlappingRooms);
		for (int i = 0; i < rooms; i++) {
			CreateRoom ();
		}
		Wall ();
		numRooms = g.NumRooms ();
		if (AddLights) {
			StartCoroutine (GenLights ());
		} else {
			StartSpawn ();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*if (Input.GetButton ("Jump")) {
			Application.LoadLevel (0);
		}*/
	}
	void CreateRoom()
	{
		tryCount++;
		if (tryCount < 25) {
			int w = Mathf.RoundToInt (Random.Range (minWidth, maxWidth));
			int h = 0;
			if (minHeight < maxRoomSize/w && maxRoomSize / w < maxHeight) {
				float height = maxRoomSize / w;
				h = Mathf.RoundToInt (Random.Range (minHeight, height));
			} else {
				h = Mathf.RoundToInt (Random.Range (minHeight, maxHeight));
			}
			if (w * h >= minRoomSize && w * h <= maxRoomSize) {
				tryCount = 0;
				g.NewRoom (w, h);
			} else {
				CreateRoom ();
				return;
			}
		} else {
			tryCount = 0;
			Debug.Log ("exceeded maximum tries, skipping room");
		}
	}
	void Wall()
	{
		float y = wall.GetComponent<Transform> ().localScale.y / 2;
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeight; j++) {
				if (g.check(i,j)) {
					Instantiate (wall, new Vector3 (i, y, j), Quaternion.identity);
				}
			}
		}
	}
	IEnumerator GenLights()
	{
		while (room < numRooms) {
			//Debug.Log ("looking for room " + room);
			Rect Rec = g.GetRoom (room);
			//Debug.Log ("Getting Room " + room + " of " + (g.NumRooms()-1) + ", Coordinates: " + g.GetRoom (room));
			room++;
			if (Rec.width > Rec.height) {
				//Light Radius = height/2;
				//Debug.Log ("Spawning " + Mathf.Floor(Rec.width/Rec.height) +" Lights in Room Horizontally with Radius " + Rec.height/2);
				SpawnLightsInRoom(Rec.x,Rec.y,Rec.height,Mathf.Ceil(Rec.width/Rec.height), true);
				yield return null;
			} else {
				//Debug.Log ("Spawning " + Mathf.Floor(Rec.height/Rec.width) +" Lights in Room Vertically with Radius " + Rec.width/2);
				SpawnLightsInRoom (Rec.x, Rec.y, Rec.width, Mathf.Ceil(Rec.height / Rec.width), false);
				yield return null;
			}
		}
		StartSpawn ();
	}
	void SpawnLightsInRoom(float x, float y, float diameter, float factor, bool horiz= true)
	{
		GameObject temp = null;
		for (int i = 0; i <= factor; i++) {			
			if (horiz == true) {
				//Debug.Log ("Creating Light #" + i + " at " + ((x - diameter / 2) + diameter * (i + 1)) + ", " + (y + diameter / 2));
				temp = Instantiate (Light, new Vector3 ((x - diameter / 2) + diameter * (i + 1), diameter*BrightnessFactor(7)/2, y + diameter / 2), Quaternion.identity) as GameObject;
				//Debug.Log ("Setting Brightness to " + diameter*BrightnessFactor(7));
				temp.GetComponent<Light>().range=diameter*BrightnessFactor(7);
			} else {
				//Debug.Log ("Creating Light#" + i + " at" + (x + diameter / 2) + ", " + ((y - diameter / 2) + diameter * (i + 1)));
				temp = Instantiate (Light, new Vector3 (x + diameter / 2, diameter*BrightnessFactor(7)/2, (y - diameter / 2) + diameter * (i + 1)), Quaternion.identity) as GameObject;
				//Debug.Log ("Setting Brightness to " + diameter*BrightnessFactor(7));
				temp.GetComponent<Light>().range=diameter*BrightnessFactor(7);
			}

		}
	}
	float BrightnessFactor(float max)
	{
		return (1 + (brightness * max));
	}
	void StartSpawn()
	{
		if (spawnObjects) {
			spawnNum = objectsToSpawn.Length;
			CreateSpawnableArea ();
			//Debug.Log ("Spawnable Tiles: " + spawnableArea.Count);
			SpawnPlayer ();
		}
	}
	void CreateSpawnableArea()
	{
		for (int rm = 1; rm < g.NumRooms(); rm++) {
			//Debug.Log ("Adding Room " + rm + " to spawnable area\nCoordinates: "+g.GetRoom(rm));
			for (int x =(int)g.GetRoom(rm).x+1; x < g.GetRoom(rm).width+g.GetRoom(rm).x; x++) {
				for (int y = (int)g.GetRoom(rm).y+1; y < g.GetRoom(rm).height+g.GetRoom(rm).y; y++) {
					Vector2 coord = new Vector2 (x, y);
					if (CheckSpawnXY (coord)) {
						spawnableArea.Add (coord);
					}
				}
			}
			doneRooms.Add (g.GetRoom (rm));
		}
		Vector2 playerSpawn = g.GetCenter (g.GetRoom(0));
		for (int i = 0; i < spawnableArea.Count; i++) {
			for (int x = (int)playerSpawn.x-5; x <= playerSpawn.x+5; x++) {
				for (int y = (int)playerSpawn.y-5; y <= playerSpawn.y+5; y++) {
					if (spawnableArea [i] == new Vector2 (x, y)) {
						spawnableArea.RemoveAt (i);
					}
				}
			}
		}
	}
	bool CheckSpawnXY(Vector2 v)
	{
		for (int i = 0; i < doneRooms.Count; i++) {
			if (doneRooms [i].Contains (v)) {
				return false;
			}
		}
		return true;
	}
	void SpawnPlayer()
	{
		Vector2 pSpawn = g.GetCenter (g.GetRoom (0));
		Instantiate (playerSpawn, new Vector3(pSpawn.x,0.5f,pSpawn.y), Quaternion.identity);
		//Debug.Log ("Player spawned at " + pSpawn);
		StartCoroutine (SpawnObjects ());
	}
	IEnumerator SpawnObjects()
	{
		while (spawn < spawnNum) {
			
			if (objectsToSpawn [spawn].spawnNumber > 0) {
				int i = Mathf.FloorToInt (Random.Range (0, spawnableArea.Count));
				//Debug.Log ("Getting position #" + i);
				//Debug.Log("Coordinates = " + spawnableArea [i]);
				Vector2 spawnLoc = spawnableArea [i];
				Instantiate (objectsToSpawn [spawn].spawnObject, new Vector3 (spawnLoc.x, 1, spawnLoc.y), Quaternion.identity);
				objectsToSpawn [spawn].spawnNumber--;
				spawnableArea.RemoveAt (i);
				if (objectsToSpawn [spawn].children != null) {
					for (int c = 0; c < objectsToSpawn[spawn].children.Length; c++) {
						Rect r = new Rect (spawnLoc.x - objectsToSpawn[spawn].children[c].spawnWithin, spawnLoc.y - objectsToSpawn[spawn].children[c].spawnWithin, objectsToSpawn[spawn].children[c].spawnWithin*2 + 1, objectsToSpawn[spawn].children[c].spawnWithin*2 + 1);
						List<Vector2> L = new List<Vector2> ();
						for (int b = 0; b < spawnableArea.Count; b++) {
							if (r.Contains (spawnableArea [b])) {
								L.Add (spawnableArea [b]);
							}
						}
						for (int d = 0; d < objectsToSpawn[spawn].children[c].spawnNumber; d++) {
							if(L.Count>0){
								int f = Mathf.FloorToInt(Random.Range(0,L.Count));
								Vector2 vec = L[f];
								Instantiate (objectsToSpawn [spawn].children [c].spawnChildObject, new Vector3 (vec.x, 1, vec.y), Quaternion.identity);
								for (int e = 0; e < spawnableArea.Count; e++) {
									if(vec == spawnableArea[e]){
										spawnableArea.RemoveAt(e);
									}
								}
								L.RemoveAt(f);
							}
						}
					}
				}
				yield return  null;
			} else {
				spawn++;
				yield return null;
			}
		}
		Debug.Log ("Finished");
		Time.timeScale = 1;
		yield return null;
	}
}
[System.Serializable]
public class Spawn
{
	public GameObject spawnObject;
	[Range(0,1000)]
	public int spawnNumber;
	public ChildSpawn[] children;
}
[System.Serializable]
public class ChildSpawn
{
	public GameObject spawnChildObject;
	[Range(0,100)]
	public int spawnNumber;
	[Range(0,50)]
	public float spawnWithin;
}
[System.Serializable]
public class Grid
{
	[Range(1,1000)]
	int width;
	[Range(1,1000)]
	int height;
	public bool squareRooms = false;
	int[,] squares;
	List<Rect> rooms = new List<Rect>();
	int r=0;
	int trySq = 0;
	Vector2 adjust = new Vector2 (-2, -2);
	public void Initialize(int a, int w, int h, bool b)
	{
		Debug.Log (string.Format("Initializing level area {0} x {1}, non-overlapping rooms = {2}",w, h, b));
		width = w;
		height = h;
		squareRooms = b;
		squares = new int[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				squares[i,j]=0;
				//Debug.Log (i + ", " + j + "=" + squares [i, j]);
			}
		}
		//rooms=new Rect[a];
		//Debug.Log (squares + "\n" + rooms[0]);
	}
	public void NewRoom(int w, int h, int x = -1, int y = -1, bool hallway=false)
	{
		if(x == -1){
			x = Mathf.RoundToInt(Random.Range(1,width-w));
			//Debug.Log ("x = " + x + ", width = " + w);
		}
		if (y == -1) {
			y = Mathf.RoundToInt (Random.Range (1, height - h));
			//Debug.Log ("y= " + y + ", height = " + h);
		}
			//rooms [r] = new Rect (x,y,w,h);
		if (squareRooms && !hallway) {
			trySq++;
			if (trySq > 10) {
				Debug.Log (w + "x" + h + " Room overlapping: moving to next room");
				trySq = 0;
				return;
			} else {
				if (!CheckSq (x, y, w, h)) {
					Debug.Log ("try number "+trySq+", "+w+"x"+h+" Room overlapping: trying again");
					NewRoom (w, h);
					return;
				} else {
					Debug.Log ("room not overlapping, building room");
					trySq = 0;
				}
			}
		}
		Rect rec = new Rect(x,y,w,h);
		Debug.Log ("Adding Room " + rec);
		rooms.Add(rec);
		//Debug.Log (rooms [r]);
		r++;
		for (int i = x; i <= x+w; i++) {
			for (int j = y; j <= y+h; j++) {
				if (i == x || i == x + w || j == y || j == y + h) {
					//Debug.Log ("i =" + i + ", j =" + j);
					if (squares [i, j] ==0) {
						squares [i, j]++;
					}
					//Debug.Log (i + ", " + j + " = " + squares [i, j]);
				} else {
						squares [i, j]+=2;	
				}
			}			
		}
		if (r > 1) {
			if (hallway == false) {
				if (!CheckRm (w, h, x, y)) {
					CreateHallway ();
				}
			}
		}
	}
	public bool check(int x, int y)
	{
		if (squares [x, y] ==1) {
			return true;
		} else {
			return false;
		}
	}
	bool CheckRm(int w, int h, int x, int y)
	{
		for (int i = x; i <= x+w; i++) {
			for (int j = y; j <= y+h; j++) {
				if (squares [i, j] >2) {
					//Debug.Log ("Room number " +(r-1)+" connected");
					return true;
				}
			}
		}
		//Debug.Log ("Room number "+(r-1) +" not connected");
		return false;
	}
	public bool CheckSq(int x, int y, int w, int h)
	{
		for (int i = x; i <= x + w; i++) {
			for (int j = y; j <= y + h; j++) {
				if (squares [i, j] > 0) {
					return false;
				}
			}
		}
		return true;
	}
	void CreateHallway()
	{
		//Debug.Log("Connecting rooms: " + (r-1) + " and " + (r-2));
		Vector2 center1 = GetCenter (rooms [r-1]);
		Debug.Log (center1 + ", is the center of " + rooms [r - 1]);
		Vector2 center2 = Vector2.zero;
		if (squareRooms && r>3) {
			center2 = GetCenter (rooms [r - 4]);
			Debug.Log (center2 + ", is the center of " + rooms [r - 4]);
		} else {
			center2 = GetCenter (rooms [r - 2]);
			Debug.Log (center2 + ", is the center of " + rooms [r - 2]);
		}
		center1 += adjust;
		center2 += adjust;
		int height = (int)(center2.y - center1.y);
		int width = (int)(center2.x - center1.x);
		//Debug.Log ("height: " + height + ", width: " + width + ", Projected Distance: " + Mathf.Sqrt ((height * height) + (width * width)) + ", Actual Distance: " + Vector2.Distance (center1, center2));
		//Create Vertical Hallway
		//Create Horizontal Hallway
		Hhallway (center1, center2, width, height);
	}
	public Vector2 GetCenter(Rect j)
	{
		int x = Mathf.RoundToInt(j.x + (j.width / 2));
		int y = Mathf.RoundToInt(j.y + (j.height / 2));
		return new Vector2 (x, y);
	}
	void Hhallway(Vector2 a, Vector2 b, int w, int h)
	{
		if (Mathf.Abs (w) > 3) {
			Vector2 newV = Vector2.zero;
			if (w > 0) {
				int y = CheckLength ((int)a.y, 2, height);
				w = CheckLength ((int)a.x, w, width);
				//Debug.Log ("Creating Hallway at: " + new Rect (a.x, a.y, w, y));
				NewRoom (w, y, (int)a.x, (int)a.y, true);
				newV = new Vector2 (a.x + w-3, a.y );
				Vhallway (newV, b, h);
			} else {
				w = -w;
				w = CheckLength ((int)b.x, w, width);
				int y = CheckLength ((int)b.y, 2, height);
				//Debug.Log ("Creating Hallway at: " + new Rect (b.x, b.y, w, y));
				NewRoom (w, y, (int)b.x, (int)b.y, true);
				newV = new Vector2 (b.x + w-3, b.y);
				Vhallway (newV, a, -h);
			}
		} else {
			Vhallway (a, b, h);
		}
	}
	void Vhallway(Vector2 a, Vector2 b, int h)
	{
		if (Mathf.Abs (h) > 3) {
			if (h > 0) {
				h = CheckLength ((int)a.y, h, height);
				int w = CheckLength ((int)a.x, 2, width);
				//Debug.Log ("Creating Hallway at: " + new Rect (a.x, a.y, w, h));
				NewRoom (w, h, (int)a.x, (int)a.y, true);
			} else {
				h = -h;
				h = CheckLength ((int)b.y, h, height);
				int w = CheckLength ((int)b.x, 2, width);
				//Debug.Log ("Creating Hallway at: " + new Rect (b.x, b.y, w, h));
				NewRoom (w, h, (int)b.x, (int)b.y+2, true);
			}
		}

	}
	int CheckLength(int x, int w, int l)
	{
		int i = 0;
		if (l-(x + w) > 3) {
			i = w + 3;
		} else {
			i = w + (l - (x + w + 1));
		}
		return i;
	}
	public Rect GetRoom(int i)
	{
		//Debug.Log ("Getting Room " + i);
		return rooms [i];
	}
	public int NumRooms()
	{
		return r;
	}
}
