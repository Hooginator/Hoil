using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {
	public GameObject mainBase;
	public Transform target;
	public int power;
	public string name;
	public GameObject enemyPrefab;

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
		//MeshRenderer rend = enemyPrefab.GetComponent<MeshRenderer> ();
		//rend.sharedMaterial.SetColor ("_Color", colour);
		for (int i = 0; i < 3; i++) {
			spawnEnemy ();
		}
	}
	public void spawnEnemies(){
		for (int i = 0; i < 3; i++) {
			spawnEnemy ();
		}
	}
	void spawnEnemy(){
		Vector3 basepos = mainBase.GetComponent<Transform> ().position;
		Vector3 enemypos = target.position;
		Vector3 spawnPos = (2*basepos + enemypos)/3;
		print (basepos.ToString () + "  " + enemypos.ToString() + "  " + spawnPos.ToString());
		GameObject tempArmy = GameObject.Instantiate (enemyPrefab, new Vector3 (spawnPos[0]+Random.Range(0,5), spawnPos[1]+2, spawnPos[2]+Random.Range(0,5)), Quaternion.identity);
		EnemyBehavior tempBehave = tempArmy.AddComponent<EnemyBehavior>();
		tempBehave.moveSpeed = 0.5f;
		armies.Add (tempArmy);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
