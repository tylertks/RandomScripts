using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spawner : MonoBehaviour{
	public Spawn[] objectsToSpawn;
	public GameObject playerSpawn;
	[Range(0,25)]
	public int spawnSafeSpace;
	Vector2 pSpawn;
	//public ChildSpawn[] children;
	List<Vector2> spawnableArea = new List<Vector2> ();
	int spawnNum = 0;
	int spawn = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void StartSpawn()
	{
		spawnNum = objectsToSpawn.Length;
		//CreateSpawnableArea ();
		//Debug.Log ("Spawnable Tiles: " + spawnableArea.Count);
		SpawnPlayer ();
		StartCoroutine (SpawnObjects ());
	}
	public void AddSpawnPoint(Vector2 v)
	{
		spawnableArea.Add (v);
	}
	public void SetPlayerSpawn(Vector2 v)
	{
		pSpawn = v;
	}
	/*void CreateSpawnableArea()
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
			for (int x = (int)playerSpawn.x-spawnSafeSpace; x <= playerSpawn.x+spawnSafeSpace; x++) {
				for (int y = (int)playerSpawn.y-spawnSafeSpace; y <= playerSpawn.y+spawnSafeSpace; y++) {
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
	}*/
	void SpawnPlayer()
	{
		//Vector2 pSpawn = g.GetCenter (g.GetRoom (0));
		Instantiate (playerSpawn, new Vector3(pSpawn.x,0.5f,pSpawn.y), Quaternion.identity);
		//Debug.Log ("Player spawned at " + pSpawn);
		for (int i = 0; i < spawnableArea.Count; i++) {
			for (int x = (int)pSpawn.x-spawnSafeSpace; x <= pSpawn.x+spawnSafeSpace; x++) {
				for (int y = (int)pSpawn.y-spawnSafeSpace; y <= pSpawn.y+spawnSafeSpace; y++) {
					if (spawnableArea [i] == new Vector2 (x, y)) {
						spawnableArea.RemoveAt (i);
					}
				}
			}
		}
		//StartCoroutine (SpawnObjects ());
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
		//Time.timeScale = 1;
		yield return null;
	}
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
public class Spawn
{
	public GameObject spawnObject;
	[Range(0,1000)]
	public int spawnNumber;
	public ChildSpawn[] children;
}
