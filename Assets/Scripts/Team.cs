﻿using System.Collections;
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


	public List<GameObject> armies;

	// Use this for initialization
	void Start () {
		armies = new List<GameObject>();
		mainBase = gameObject;
	}
	public void Initialize(int PWR){
		// Initialize Armies
		power = PWR;
		//name = NAM;
		mainBase = gameObject;
		//MeshRenderer rend = enemyPrefab.GetComponent<MeshRenderer> ();
		//rend.sharedMaterial.SetColor ("_Color", colour);
		armies.Clear();
		// For some fucking reason this gets done BEFORE loading the main world, and so is in "battle", and so gets unloaded immediately.  even though I call it AFTER the load.  fuck you c#
		spawnEnemies();
	}
	public void spawnEnemies(){
		for (int i = 0; i < 10; i++) {
			spawnEnemy ();
		}
	}
	void spawnEnemy(){
		
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
		armies.Add (tempArmy);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
