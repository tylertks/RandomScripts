using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenLights : MonoBehaviour{
	public GameObject Light;
	List<Rect> rooms = new List<Rect>();
	[Range(0,1)]
	public float brightness;
	int room = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void AddRoom(Rect r)
	{
		rooms.Add (r);
	}
	public void StartLighting()
	{
		StartCoroutine (Lights ());
	}
	IEnumerator Lights()
	{
		while (room < rooms.Count) {
			//Debug.Log ("looking for room " + room);
			Rect Rec =  rooms[room];
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
		//StartSpawn ();
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
}
