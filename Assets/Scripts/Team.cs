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


	public List<GameObject> armies;

	// Use this for initialization
	void Start () {
		armies = new List<GameObject>();
		mainBase = gameObject;
	}
	public void Initialize(int PWR, string NAM){
		// Initialize Armies
		power = PWR;
		name = NAM;
		mainBase = gameObject;
		//MeshRenderer rend = enemyPrefab.GetComponent<MeshRenderer> ();
		//rend.sharedMaterial.SetColor ("_Color", colour);
		spawnEnemies();
	}
	public void spawnEnemies(){
		for (int i = 0; i < 10; i++) {
			spawnEnemy ();
		}
	}
	void spawnEnemy(){
		Vector3 basepos = mainBase.GetComponent<Transform> ().position;
		minRange = 10f;
		maxRange = 50f;
		float dist = Random.Range (minRange, maxRange);
		float angle = Random.Range (0f, 2*Mathf.PI);
		Vector3 relativeSpawn = new Vector3(dist*Mathf.Sin (angle),0,dist*Mathf.Cos (angle));
		var maaap = GameObject.Find ("Map");
		Vector3 spawnPos =  maaap.GetComponent<Map> ().MirrorInsideBoundaries (relativeSpawn + basepos);
		print (basepos.ToString () + "  " + spawnPos.ToString());
		GameObject tempArmy = GameObject.Instantiate (enemyPrefab, new Vector3 (spawnPos[0]+Random.Range(0,5), spawnPos[1]+2, spawnPos[2]+Random.Range(0,5)), Quaternion.identity);
		EnemyBehavior tempBehave = tempArmy.AddComponent<EnemyBehavior>();
		tempBehave.moveSpeed = 0.5f;
		armies.Add (tempArmy);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
