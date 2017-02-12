﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMovementControls : MonoBehaviour {
	public float moveSpeed; // max Force that can be applied to object
	public Rigidbody RB; // needed to apply forces to object.
	public Transform TR; // Needed to rotate object
	public int RotationSpeed;
	private Vector3 input = new Vector3(0,0,1);
	private Vector3 currentPos;
	// Use this for initialization
	void Start () {
		RB = GetComponent<Rigidbody> ();
		TR = GetComponent<Transform> ();
		var gameManager = GameObject.Find ("GameManager");
		TR.position = gameManager.GetComponent<gameManager> ().playerMapPosition.position;
		RB.angularVelocity = Vector3.zero;
		RotationSpeed = 5;
		moveSpeed = 20;
	}
	
	// Update is called once per frame
	void Update () {
		// Read input from user and turn it into a vector
		input = new Vector3 (Input.GetAxis ("Horizontal"), 0.0f, Input.GetAxis ("Vertical"));

		// apply force to Player
		RB.AddForce(input*moveSpeed); 
		// Get angle to look at
		Vector3 angle = Vector3.RotateTowards(transform.forward, input, RotationSpeed, 0.0F);
		// Apply angle
		TR.rotation = Quaternion.LookRotation(angle);
		// Keep player within World map
		var map = GameObject.Find ("Map");
		currentPos = TR.position;
		TR.position = map.GetComponent<Map> ().ForceInsideBoundaries (currentPos);
	}
	void OnCollisionEnter(Collision col){
		print ("Collision at:  " + col.collider.transform.position.ToString ());
		// Only if what we collided with was an enemy
		if (col.gameObject.GetComponent<EnemyBehavior>() != null) {
			// Find game manager as that has the loading and unloading functions
			var gameManager = GameObject.Find ("GameManager");
			// If we're not already in battle, load it up. 
			if (gameManager.GetComponent<gameManager> ().inBattle != true) {
				print ("Gonna Load Up Battle");
				gameManager.GetComponent<gameManager> ().StartBattle();
				//gameManager.GetComponent<SceneManager> ().UnLoadScene ("Hoil");
			}
		}
	}
}
