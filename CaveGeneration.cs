using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Uses the GenLights and Spawner scripts to generate lighting and objects within the map. Points of complexity is the number of nodes.
[RequireComponent(typeof(GenLights))]
[RequireComponent(typeof(Spawner))]
public class CaveGeneration : MonoBehaviour {
	public int xSize;
	public int ySize;

	public int pointsOfComplexity;
	public bool useGridDistance;

	[Range(10,90)]
	public float fillPercent;

	public GameObject wall;

	public Point[,] g= null;
	List<Node> nodes = new List<Node>();
	List<Node> caveNodes= new List<Node>();
	float total;
	float filled;
	int tries = 0;

	GenLights lights;
	public bool generateLights;

	Spawner spawner;
	public bool spawnObjects;

	// Use this for initialization
	void Start () {
		g = new Point[xSize, ySize];
		Debug.Log ("Creating grid " + xSize + "x" + ySize);
		total = xSize * ySize;
		filled = total;
		CreateGrid ();
		CreateNodes ();
		CreateRooms ();
		FillEdges ();
		Carve ();
		Debug.Log (filled / total + " filled.");
		CreateWalls ();
		if (generateLights) {
			lights = gameObject.GetComponent<GenLights> ();
			CreateRect ();
		}
		if (spawnObjects) {
			spawner = gameObject.GetComponent<Spawner> ();
			CreateSpawnableArea ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Jump")) {
			Application.LoadLevel (1);
		}
	
	}
	void CreateGrid()
	{
		for (int i = 0; i < xSize; i++) {
			for (int j = 0; j < ySize; j++) {
				g [i, j] = new Point ();
				g [i, j].Set (i, j);
			}
		}
	}
	void CreateNodes()
	{
		for (int i = 0; i < pointsOfComplexity; i++) {
			GenNode ();
		}
	}
	void GenNode()
	{
		tries++;
		if (tries < 6) {
			int x = Mathf.FloorToInt (Random.Range (0, xSize));
			int y = Mathf.FloorToInt (Random.Range (0, ySize));
			if (g [x, y].Node () == false) {
				Node n = new Node ();
				n.point = g [x, y].Get ();
				Debug.Log ("adding node at " + x + ", " + y);
				nodes.Add (n);
				g [x, y].SetNode (nodes [nodes.Count - 1], true);
				tries = 0;
			} else {
				GenNode ();
			}
		} else {
			tries = 0;
		}
	}
	void CreateRooms()
	{
		for (int i = 0; i < xSize; i++) {
			for (int j = 0; j < ySize; j++) {
				if (g [i, j].Node() == false) {
					FindClosest (g [i, j]);
				}
			}
		}
	}
	void FindClosest(Point p)
	{
		float dist = xSize + ySize;
		Vector2 n = Vector2.zero;
		int node = 0;
		for (int i = 0; i < nodes.Count; i++) {
			if (useGridDistance) {
				if (dist>(Mathf.Abs (nodes [i].point.x - p.xPos) + Mathf.Abs (nodes [i].point.y - p.yPos))) {
					dist = Mathf.Abs (nodes [i].point.x - p.xPos) + Mathf.Abs (nodes [i].point.y - p.yPos);
					n = nodes [i].point;
					node =i;
				}
			} else {
				if (dist > Vector2.Distance (nodes [i].point, p.Get ())) {
					dist = Vector2.Distance (nodes [i].point, p.Get ());
					n = nodes [i].point;
					node=i;
				}
			}
		}
		p.SetNode (nodes[node]);
		nodes [node].AddPoint (p.Get ());
	}
	void FillEdges()
	{
		for (int i = 0; i < xSize; i++) {
			if (g [i, 0].GetWall ()==false) {
				SetWall (g [i, 0].GetNode ());
			}
			if (g [i, ySize - 1].GetWall () == false) {
				SetWall (g [i, ySize - 1].GetNode ());
			}
		}
		for (int i = 0; i < ySize; i++) {
			if (g [0, i].GetWall () == false) {
				SetWall (g [0, i].GetNode ());
			}
			if (g [xSize - 1, i].GetWall () == false) {
				SetWall (g [xSize - 1, i].GetNode ());
			}
		}
	}
	void SetWall(Node n)
	{
		Debug.Log ("Setting " + n.point + " to wall");
		foreach (Vector2 v in n.room) {
			GetPoint(v).SetWall (true);
		}
		GetPoint(n.point).SetWall (true);
	}
	void FillRoom(Node n)
	{
		g [(int)n.point.x, (int)n.point.y].SetFill ();
		filled++;
		for (int i = 0; i < n.room.Count; i++) {
			g [(int)n.room [i].x, (int)n.room [i].y].SetFill();
			filled++;
		}
	}
	void CreateWalls()
	{
		for (int i = 0; i < xSize; i++) {
			for (int j = 0; j < ySize; j++) {
				if (g [i, j].IsFilled ()) {
					Instantiate (wall, new Vector3 (i, 0, j), Quaternion.identity);
				}
			}
		}
	}
	void FillRandom()
	{
		tries++;
		if ((filled / total) < (fillPercent / 100) && tries < 21) {
			int i = Mathf.FloorToInt (Random.Range (0, nodes.Count));
			/*if (GetPoint(nodes[i].point).IsFilled() == false && GetAdjacent(nodes[i]) <2) {
				FillRoom (nodes [i]);
				tries = 0;
				FillRandom ();
			} else {
				FillRandom ();
			}*/
		}
		else{
			tries = 0;
		}
	}
	void Carve()
	{
		//Debug.Log ("Carving new room");
		tries++;
		if (filled / total > fillPercent / 100 && tries < 11) {
			if (caveNodes.Count < 1) {
				int i = Mathf.RoundToInt (Random.Range (0, nodes.Count-1));
				if (GetPoint (nodes [i].point).GetWall () == false) {
					tries = 0;
					EmptyRoom (nodes [i]);
					caveNodes.Add (nodes [i]);						
					Carve ();
				} else {
					Carve ();
				}
			} else {
				int i = Mathf.RoundToInt (Random.Range (0, caveNodes.Count-1));
				Node node = GetAdjacent (caveNodes [i]);
				if (node != caveNodes [i]) {
					tries = 0;
					caveNodes.Add (node);
					EmptyRoom (node);
				} else {
					caveNodes.RemoveAt (i);
				}
				Carve ();
			}
		} else {
			tries = 0;
		}
	}
	void EmptyRoom(Node n)
	{
		//Debug.Log ("Carving room at " + n.point);
		foreach (Vector2 v in n.room) {
			GetPoint (v).SetFill (false);
			filled--;
		}
		GetPoint (n.point).SetFill (false);
		filled--;
	}
	Point GetPoint(Vector2 v)
	{
		Vector2 vec = v;
		if (v.x >= xSize) {
			vec = new Vector2 (xSize - 1, vec.y);
		}
		if (v.x < 0) {
			vec = new Vector2 (0, vec.y);
		}
		if (v.y < 0) {
			vec = new Vector2 (vec.x, 0);
		}
		if (v.y >= ySize) {
			vec = new Vector2 (vec.x, ySize - 1);
		}
		return g [(int)vec.x, (int)vec.y];
	}
	Node GetAdjacent(Node n)
	{
		//int a = 0;
		bool wall = false;
		List<Node> tempNode = new List<Node> ();
		Node node = n;
		for (int i = 0; i < n.room.Count; i++) {
			for (int x = -1; x < 2; x++) {
				if (GetPoint (new Vector2 (n.room [i].x + x, n.room [i].y)).IsFilled () == true) {
					if (GetPoint (new Vector2 (n.room [i].x + x, n.room [i].y)).GetWall () == false) {
						//wall = true;
						if (tempNode.Contains (GetPoint (new Vector2 (n.room [i].x + x, n.room [i].y)).GetNode ()) == false) {
							tempNode.Add (GetPoint (new Vector2 (n.room [i].x + x, n.room [i].y)).GetNode ());
						}
					} 
				}
			}
			for (int y = -1; y < 2; y++) {
				if (GetPoint (new Vector2 (n.room [i].x, n.room [i].y + y)).IsFilled () == true) {
					if (GetPoint (new Vector2 (n.room [i].x, n.room [i].y + y)).GetWall () == false) {
						//wall = true;
						if (tempNode.Contains (GetPoint (new Vector2 (n.room [i].x, n.room [i].y + y)).GetNode ()) == false) {
							tempNode.Add (GetPoint (new Vector2 (n.room [i].x, n.room [i].y + y)).GetNode ());
						}
					} 
				}
			}
		}
		if (tempNode.Count > 0) {
			//a++;
			int a = Mathf.FloorToInt(Random.Range(0,tempNode.Count));
			node = tempNode [a];
		}
		return node;
	}
	Rect GetRoom(Node n){
		int left = xSize;
		int top = ySize;
		int right = 0;
		int bottom = 0;
		foreach (Vector2 v in n.room) {
			if (v.x < left) {
				left = (int)v.x;
			}
			if (v.x > right) {
				right = (int)v.x;
			}
			if (v.y < top) {
				top = (int)v.y;
			}
			if (v.y > bottom) {
				bottom = (int)v.y;
			}
		}
		return new Rect (left, top, right - left, bottom - top);
	}
	void CreateRect()
	{
		foreach (Node n in nodes) {
			if (GetPoint (n.point).IsFilled () == false) {
				lights.AddRoom (GetRoom (n));
			}
		}
		lights.StartLighting ();
	}
	void CreateSpawnableArea()
	{
		bool s = false;
		foreach (Node n in nodes) {
			if (GetPoint (n.point).IsFilled() == false) {
				for (int i = 0; i < n.room.Count; i++) {
					spawner.AddSpawnPoint (n.room [i]);
				}
				spawner.AddSpawnPoint (n.point);
				if (s == false) {
					spawner.SetPlayerSpawn (n.point);
					s = true;
				}
			}
		}
		spawner.StartSpawn ();
	}
}

[System.Serializable]
public class Point{
	public int xPos;
	public int yPos;
	//[NonSerialized]
	private Node node;
	//[NonSerialized]
	private bool fill = true;
	//[NonSerialized]
	private bool isNode = false;
	private bool isWall = false;

	public Vector2 Get()
	{
		return new Vector2 (xPos, yPos);
	}

	public void SetNode(Node n,bool b = false)
	{
		if (b) {
			//Debug.Log ("Creating node at : " + xPos + ", " + yPos);
			isNode = true;
		}
		node = n;
	}

	public void Set(int x, int y)
	{
		xPos = x;
		yPos = y;
	}
	public Node GetNode()
	{
		return node;
	}
	public void SetFill(bool b = true)
	{
		fill = b;
	}
	public bool IsFilled()
	{
		return fill;
	}
	public bool Node()
	{
		return isNode;
	}
	public void SetWall(bool b = true)
	{
		isWall = b;
	}
	public bool GetWall()
	{
		return isWall;
	}
}

[System.Serializable]
public class Node{
	public Vector2 point;
	public List<Vector2> room = new List<Vector2>();

	public void AddPoint(Vector2 p)
	{
		room.Add (p);
	}
}
