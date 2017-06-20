﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************************/
// Tracks the world map teams and manages their growth / decay
// Spawns individual enemies near the base.
/***********************************************************/

public class Team : MonoBehaviour {
	public GameObject mainBase;
	public int power;
	public string teamName;
	public GameObject enemyPrefab;

	// For Enemy Spawning
	public float maxRange;
	public float minRange;

	public int level = 200; // Current total level of 
	public int spawnLevelMean;
	public int spawnLevelRange;

	public List<GameObject> armies;


	/********************************************************************************************/ 
	/************************************* Initialization ***************************************/ 
	/********************************************************************************************/


	// Use this for initialization
	void Start () {
		armies = new List<GameObject>();
		mainBase = gameObject;
	} 
	public void Initialize(){
		// Initialize Armies
		mainBase = gameObject;
		armies.Clear();
		spawnLevelMean = 10;
		spawnLevelRange = 3;
		spawnEnemies();
		updateLevelIndicator ();
	}

	/********************************************************************************************/ 
	/************************************* Create Units *****************************************/ 
	/********************************************************************************************/

	public void spawnEnemies(){
		// Spawn a bunch of Armies based on team levels.
		int tempLevel = level;
		int spawnLevel;
		int timer = 0;
		while (tempLevel >= 0) {
			// Generate a random level to be assigned to the new enemy
			spawnLevel = Random.Range (spawnLevelMean - spawnLevelRange, spawnLevelMean + spawnLevelRange);
			spawnEnemy (spawnLevel);
			// Keep track of how many levels of enemies have been created
			tempLevel -= spawnLevel;
			//print ("Spawned "+teamName+" lvl "+spawnLevel.ToString ()+" "+tempLevel+" to go");
			timer++;
			if (timer > 20) {
				tempLevel = -10;
			}
		}
	}

	void spawnEnemy(int lvl){
		// Creates a new Army at the given level somewhere around the base.
		//print ("Spawning enenmy");
		Vector3 basepos = mainBase.GetComponent<Transform> ().position;
		minRange = 30f;
		maxRange = 60f;
		// Distance the enemy will spawn from the main base location
		float dist = Random.Range (minRange, maxRange);
		// angle from the base the enemy will spawn
		float angle = Random.Range (0f, 2f*Mathf.PI);
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
		tempBehave.team = teamName;
		tempArmy.GetComponent<BasicEnemyAnimations> ().updateLevel (lvl);
		armies.Add (tempArmy);
	}


	/********************************************************************************************/ 
	/************************************* Upkeep ***********************************************/ 
	/********************************************************************************************/

	// Update is called once per frame
	void Update () {
		// Every so often add to a bases power level
		if (Time.frameCount % 300 == 20) {
			//print ("Add to level");
			level += 1;
			updateLevelIndicator ();
		}
	}

	// Update level indicator
	public void updateLevelIndicator(){
		// As of now Army Text HAS to be the first child element in the base game object.
		GameObject levelText = gameObject.transform.GetChild(0).gameObject;
		//print ("Update team level strings");
		levelText.GetComponent<TextMesh>().text = level.ToString ();
	}
}
