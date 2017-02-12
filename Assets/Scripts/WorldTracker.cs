using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTracker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var gameManager = GameObject.Find ("GameManager").GetComponent<gameManager>();
		for (int i = 0; i < gameManager.teams.Count; i++) {
			gameManager.teams [i].GetComponent<Team> ().spawnEnemies ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
