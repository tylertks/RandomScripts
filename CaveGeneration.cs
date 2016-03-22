using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaveGen : MonoBehaviour {
	public int xSize;
	public int ySize;

	public int pointsOfComplexity;
	public bool useGridDistance;

	[Range(10,90)]
	public float fillPercent;

	public GameObject wall;

	public Point[,] g= null;
	List<Node> nodes = new List<Node>();
	float total;
	float filled;
	int tries = 0;

	// Use this for initialization
	void Start () {
		g = new Point[xSize, ySize];
		Debug.Log ("Creating grid " + xSize + "x" + ySize);
		total = xSize * ySize;
		CreateGrid ();
		CreateNodes ();
		CreateRooms ();
		FillEdges ();
		FillRandom ();
		Debug.Log (filled / total + " filled.");
		CreateWalls ();
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
			if (g [i, 0].IsFilled ()==false) {
				FillRoom (g [i, 0].GetNode ());
			}
			if (g [i, ySize - 1].IsFilled () == false) {
				FillRoom (g [i, ySize - 1].GetNode ());
			}
		}
		for (int i = 0; i < ySize; i++) {
			if (g [0, i].IsFilled () == false) {
				FillRoom (g [0, i].GetNode ());
			}
			if (g [xSize - 1, i].IsFilled () == false) {
				FillRoom (g [xSize - 1, i].GetNode ());
			}
		}
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
			if (GetPoint(nodes[i].point).IsFilled() == false && GetAdjacent(nodes[i]) <2) {
				FillRoom (nodes [i]);
				tries = 0;
				FillRandom ();
			} else {
				FillRandom ();
			}
		}
		else{
			tries = 0;
		}
	}
	Point GetPoint(Vector2 v)
	{
		return g [(int)v.x, (int)v.y];
	}
	int GetAdjacent(Node n)
	{
		int a = 0;
		bool wall = false;
		List<Node> tempNode = new List<Node> ();
		for (int i = 0; i < n.room.Count; i++) {
			for (int x = -1; x < 2; x++) {
				for (int y = -1; y < 2; y++) {
					if (GetPoint (new Vector2 (n.room [i].x + x, n.room [i].y + y)).IsFilled () == true) {
						if (GetPoint (new Vector2 (n.room [i].x + x, n.room [i].y + y)).GetWall () == true) {
							wall = true;
						} else {
							if (tempNode.Contains (GetPoint (new Vector2 (n.room [i].x + x, n.room [i].y + y)).GetNode ()) == false) {
								tempNode.Add (GetPoint (new Vector2 (n.room [i].x + x, n.room [i].y + y)).GetNode ());
								a++;
							}
						}
					}
				}				
			}
		}
		if (wall == true) {
			a++;
		}
		return a;
	}
}

[System.Serializable]
public class Point{
	public int xPos;
	public int yPos;
	//[NonSerialized]
	private Node node;
	//[NonSerialized]
	private bool fill = false;
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
			Debug.Log ("Creating node at : " + xPos + ", " + yPos);
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
