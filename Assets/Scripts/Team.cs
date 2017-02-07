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
	void spawnEnemy(){
		GameObject tempArmy = GameObject.Instantiate (enemyPrefab, new Vector3 (45, 2, 35), Quaternion.identity);
		armies.Add (tempArmy);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
