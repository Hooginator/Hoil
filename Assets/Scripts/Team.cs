using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {
	public GameObject mainBase;
	public int power;
	public string name;
	public GameObject enemyPrefab;

	// For Enemy Spawning
	public float maxRange;
	public float minRange;

	public int level = 200; // Current total level of 
	public int spawnLevelMean;
	public int spawnLevelRange;

	public List<GameObject> armies;

	// Use this for initialization
	void Start () {
		armies = new List<GameObject>();
		mainBase = gameObject;
	}
	public void Initialize(){
		// Initialize Armies
		//name = NAM;
		mainBase = gameObject;
		//MeshRenderer rend = enemyPrefab.GetComponent<MeshRenderer> ();
		//rend.sharedMaterial.SetColor ("_Color", colour);
		armies.Clear();
		spawnLevelMean = 10;
		spawnLevelRange = 3;
		// For some fucking reason this gets done BEFORE loading the main world, and so is in "battle", and so gets unloaded immediately.  even though I call it AFTER the load.  fuck you c#
		spawnEnemies();
	}
	public void spawnEnemies(){
		int tempLevel = level;
		int spawnLevel;
		int timer = 0;
		while (tempLevel >= 0) {
			spawnLevel = Random.Range (spawnLevelMean - spawnLevelRange, spawnLevelMean + spawnLevelRange);
			spawnEnemy (spawnLevel);
			tempLevel -= spawnLevel;
			print (tempLevel.ToString ());
			timer++;
			if (timer > 20) {
				tempLevel = -10;
			}

		}
		/* Old way to spawn enemies
		for (int i = 0; i < 10; i++) {
			spawnEnemy (level);
		}*/


	}
	void spawnEnemy(int lvl){
		
		print ("Spawning enenmy");
		Vector3 basepos = mainBase.GetComponent<Transform> ().position;
		minRange = 10f;
		maxRange = 40f;
		// Distance the enemy will spawn from the main base location
		float dist = Random.Range (minRange, maxRange);
		// angle from the base the enemy will spawn
		float angle = Random.Range (0f, 2*Mathf.PI);
		// Calculate teh Vector3 based on the RNG numbers
		Vector3 relativeSpawn = new Vector3(dist*Mathf.Sin (angle),0,dist*Mathf.Cos (angle));
		// Force the spawn location to fit inside the map.
		var maaap = GameObject.Find ("Map");
		Vector3 spawnPos =  maaap.GetComponent<Map> ().MirrorInsideBoundaries (relativeSpawn + basepos);
		//print ("Spawning Unit at:    " + spawnPos.ToString());
		GameObject tempArmy = GameObject.Instantiate (enemyPrefab, spawnPos, Quaternion.identity);
		EnemyBehavior tempBehave = tempArmy.AddComponent<EnemyBehavior>();
		tempBehave.moveSpeed = 0.5f;
		tempBehave.level = lvl;
		tempBehave.prefab = enemyPrefab;
		armies.Add (tempArmy);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
