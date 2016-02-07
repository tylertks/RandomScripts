using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGeneration : MonoBehaviour {
	public Grid g;
	int tryCount=0;
	public GameObject wall;
	[Range(1,100)]
	public int rooms;
	[Range(1,50)]
	public int minRoomSize;
	[Range(1,100)]
	public int maxWidth;
	[Range(1,100)]
	public int maxHeight;
	[Range(1,25)]
	public int minWidth;
	[Range(1,25)]
	public int minHeight;
	public bool AddLights = false;
	public GameObject Light;
	[Range(0,1)]
	public float brightness;
	int numRooms;
	int room = 0;
	// Use this for initialization
	void Start () 
	{
		g.Initialize (rooms);
		for (int i = 0; i < rooms; i++) {
			CreateRoom ();
		}
		Wall ();
		numRooms = g.NumRooms ();
		if(AddLights)
			StartCoroutine (GenLights ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetButton ("Jump")) {
			Application.LoadLevel (0);
		}
	}
	void CreateRoom()
	{
		tryCount++;
		if (tryCount < 10) {
			int w = Mathf.RoundToInt(Random.Range (minWidth, maxWidth));
			int h = Mathf.RoundToInt (Random.Range (minHeight, maxHeight));
			if (w * h > minRoomSize) {
				tryCount = 0;
				g.NewRoom (w,h);
			} else {
				CreateRoom ();
			}
		}
	}
	void Wall()
	{
		float y = wall.GetComponent<Transform> ().localScale.y / 2;
		for (int i = 0; i < g.width; i++) {
			for (int j = 0; j < g.height; j++) {
				if (g.check(i,j)) {
					Instantiate (wall, new Vector3 (i, y, j), Quaternion.identity);
				}
			}
		}
	}
	IEnumerator GenLights()
	{
		while (room < numRooms) {
			Rect Rec = g.GetRoom (room);
			Debug.Log ("Getting Room " + room + " of " + (g.NumRooms()-1) + ", Coordinates: " + g.GetRoom (room));
			room++;
			if (Rec.width > Rec.height) {
				//Light Radius = height/2;
				Debug.Log ("Spawning " + Mathf.Floor(Rec.width/Rec.height) +" Lights in Room Horizontally with Radius " + Rec.height/2);
				SpawnLightsInRoom(Rec.x,Rec.y,Rec.height,Mathf.Ceil(Rec.width/Rec.height), true);
				yield return null;
			} else {
				Debug.Log ("Spawning " + Mathf.Floor(Rec.height/Rec.width) +" Lights in Room Vertically with Radius " + Rec.width/2);
				SpawnLightsInRoom (Rec.x, Rec.y, Rec.width, Mathf.Ceil(Rec.height / Rec.width), false);
				yield return null;
			}
		}

	}
	void SpawnLightsInRoom(float x, float y, float diameter, float factor, bool horiz= true)
	{
		GameObject temp = null;
		for (int i = 0; i < factor; i++) {			
			if (horiz == true) {
				Debug.Log ("Creating Light #" + i + " at " + ((x - diameter / 2) + diameter * (i + 1)) + ", " + (y + diameter / 2));
				temp = Instantiate (Light, new Vector3 ((x - diameter / 2) + diameter * (i + 1), diameter*BrightnessFactor(2.5f), y + diameter / 2), Quaternion.identity) as GameObject;
				Debug.Log ("Setting Radius to " + diameter / 2);
				temp.GetComponent<Light>().range=diameter*BrightnessFactor(4);
			} else {
				Debug.Log ("Creating Light#" + i + " at" + (x + diameter / 2) + ", " + ((y - diameter / 2) + diameter * (i + 1)));
				temp = Instantiate (Light, new Vector3 (x + diameter / 2, diameter*BrightnessFactor(2.5f), (y - diameter / 2) + diameter * (i + 1)), Quaternion.identity) as GameObject;
				temp.GetComponent<Light>().range=diameter*BrightnessFactor(4);
			}

		}
	}
	float BrightnessFactor(float max)
	{
		return 1 + brightness * max;
	}
}

[System.Serializable			]
public class Grid
{
	[Range(1,10000)]
	public int width;
	[Range(1,10000)]
	public int height;
	int[,] squares;
	List<Rect> rooms = new List<Rect>();
	int r=0;
	public void Initialize(int a)
	{
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
	public void NewRoom(int w, int h, int x = 1, int y = 1, bool hallway=false)
	{
		if(x == 1){
			x = Mathf.RoundToInt(Random.Range(1,width-w));
			//Debug.Log ("x = " + x + ", width = " + w);
		}
		if (y == 1) {
			y = Mathf.RoundToInt (Random.Range (1, height - h));
			//Debug.Log ("y= " + y + ", height = " + h);
		}
			//rooms [r] = new Rect (x,y,w,h);
		Rect rec = new Rect(x,y,w,h);
		rooms.Add(rec);
		//Debug.Log (rooms [r]);
		r++;
		for (int i = x; i <= x+w; i++) {
			for (int j = y; j <= y+h; j++) {
				if (i == x || i == x + w || j == y || j == y + h) {
					//Debug.Log ("i =" + i + ", j =" + j);
					if(squares[i,j]<1)
					squares [i, j]++;
					//Debug.Log (i + ", " + j + " = " + squares [i, j]);
				} else {
					squares [i, j]+=2;
				}
			}			
		}
		if (r > 1) {
			if (!CheckRm (w, h, x, y) && hallway == false) {
				CreateHallway ();
			}
		}
	}
	public bool check(int x, int y)
	{
		if (squares [x, y] == 1) {
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
					//Debug.Log ("connected");
					return true;
					break;
				}
			}
		}
		Debug.Log ("Room number"+(r-1) +" not connected");
		return false;
	}
	void CreateHallway()
	{
		Debug.Log("Connecting rooms: " + (r-1) + " and " + (r-2));
		Vector2 center1 = GetCenter (rooms [r-1]);
		Debug.Log (center1 + ", is the center of " + rooms [r - 1]);
		Vector2 center2 = GetCenter (rooms [r - 2]);
		Debug.Log (center2 + ", is the center of " + rooms [r - 2]);
		int height = (int)(center2.y - center1.y);
		int width = (int)(center2.x - center1.x);
		//Debug.Log ("height: " + height + ", width: " + width + ", Projected Distance: " + Mathf.Sqrt ((height * height) + (width * width)) + ", Actual Distance: " + Vector2.Distance (center1, center2));
		//Create Vertical Hallway
		//Create Horizontal Hallway
		Hhallway (center1, center2, width, height);
	}
	Vector2 GetCenter(Rect j)
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
				Debug.Log ("Creating Hallway at: " + new Rect (a.x, a.y, w, y));
				NewRoom (w, y, (int)a.x, (int)a.y, true);
				newV = new Vector2 (a.x + w-3, a.y);
				Vhallway (newV, b, h);
			} else {
				w = -w;
				w = CheckLength ((int)b.x, w, width);
				int y = CheckLength ((int)b.y, 2, height);
				Debug.Log ("Creating Hallway at: " + new Rect (b.x, b.y, w, y));
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
				Debug.Log ("Creating Hallway at: " + new Rect (a.x, a.y, w, h));
				NewRoom (w, h, (int)a.x, (int)a.y, true);
			} else {
				h = -h;
				h = CheckLength ((int)b.y, h, height);
				int w = CheckLength ((int)b.x, 2, width);
				Debug.Log ("Creating Hallway at: " + new Rect (b.x, b.y, w, h));
				NewRoom (w, h, (int)b.x, (int)b.y, true);
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
		return rooms [i];
	}
	public int NumRooms()
	{
		return r;
	}
}