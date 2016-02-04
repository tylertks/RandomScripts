using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomGen : MonoBehaviour {
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
	// Use this for initialization
	void Start () 
	{
		g.Initialize (rooms);
		for (int i = 0; i < rooms; i++) {
			CreateRoom ();
		}
		Wall ();
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
		for (int i = 0; i < g.width; i++) {
			for (int j = 0; j < g.height; j++) {
				if (g.check(i,j)) {
					Instantiate (wall, new Vector3 (i, 0, j), Quaternion.identity);
				}
			}
		}
	}
			
}

[System.Serializable]
public class Rm
{
	public int startX;
	public int startY;
	public int width;
	public int height;
}

[System.Serializable]
public class Grid
{
	[Range(1,10000)]
	public int width;
	[Range(1,10000)]
	public int height;
	int[,] squares;
	Rect[] rooms;
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
		rooms=new Rect[a];
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
		if (!hallway) {
			rooms [r] = new Rect (x,y,w,h);
			//Debug.Log (rooms [r]);
			r++;
		}
		for (int i = x; i <= x+w; i++) {
			for (int j = y; j <= y+h; j++) {
				if (i == x || i == x + w || j == y || j == y + h) {
					//Debug.Log ("i =" + i + ", j =" + j);
					if(squares[i,j]==0)
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
				if (squares [i, j] >4) {
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
		Debug.Log ("height: " + height + ", width: " + width + ", Projected Distance: " + Mathf.Sqrt ((height * height) + (width * width)) + ", Actual Distance: " + Vector2.Distance (center1, center2));
		//Create Vertical Hallway
		//Create Horizontal Hallway
		Hhallway(center1,center2,width, height);
	}
	Vector2 GetCenter(Rect j)
	{
		int x = Mathf.RoundToInt(j.x + (j.width / 2));
		int y = Mathf.RoundToInt(j.y + (j.height / 2));
		return new Vector2 (x, y);
	}
	void Hhallway(Vector2 a, Vector2 b, int width, int height)
	{
		Vector2 newV = Vector2.zero;
		if (width > 0) {
			Debug.Log("Creating Hallway at: " + new Rect(a.x, a.y, width, 5));
			NewRoom (width+2, 5, (int)a.x, (int)a.y, true);
			newV = new Vector2 (a.x + width + 2, a.y);
			Vhallway (newV, b, height);
		} else {
			width = -width;
			Debug.Log("Creating Hallway at: " + new Rect(b.x, b.y, width, 5));
			NewRoom (width+2, 5, (int)b.x, (int)b.y, true);
			newV = new Vector2 (b.x + width + 2, b.y);
			Vhallway (newV, a, -height);
		}


	}
	void Vhallway(Vector2 a, Vector2 b, int height)
	{
		if (height > 0) {
			Debug.Log ("Creating Hallway at: " + new Rect (a.x, a.y, 5, height));
			NewRoom(5, height+2, (int)a.x, (int)a.y, true);
		} else {
			height = -height;
			Debug.Log ("Creating Hallway at: " + new Rect (b.x, b.y, 5, height));
			NewRoom(5, height+2, (int)b.x, (int)b.y, true);
		}

	}
}
