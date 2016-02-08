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
				temp = Instantiate (Light, new Vector3 ((x - diameter / 2) + diameter * (i + 1), diameter*BrightnessFactor(7)/2, y + diameter / 2), Quaternion.identity) as GameObject;
				Debug.Log ("Setting Brightness to " + diameter*BrightnessFactor(7));
				temp.GetComponent<Light>().range=diameter*BrightnessFactor(7);
			} else {
				Debug.Log ("Creating Light#" + i + " at" + (x + diameter / 2) + ", " + ((y - diameter / 2) + diameter * (i + 1)));
				temp = Instantiate (Light, new Vector3 (x + diameter / 2, diameter*BrightnessFactor(7)/2, (y - diameter / 2) + diameter * (i + 1)), Quaternion.identity) as GameObject;
				Debug.Log ("Setting Brightness to " + diameter*BrightnessFactor(7));
				temp.GetComponent<Light>().range=diameter*BrightnessFactor(7);
			}

		}
	}
	float BrightnessFactor(float max)
	{
		return (1 + (brightness * max));
	}
}
