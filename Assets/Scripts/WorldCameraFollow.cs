﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCameraFollow : MonoBehaviour {
	public float moveSpeed; // Set to match the palyer
	public float buffer; // buffer is the max distance away from the player the acmera will not move at
	public Vector3 Idealpos = new Vector3(0,7,-5); // Initial camera position so we know a good relative distance to the player.  WIll be replaced by 1 distance for the camera if we do cam rotation
	public Rigidbody RB; // needed to add force (ie move) the camera
	public Transform TR; // Needed to move object
	private Vector3 input; // the Force that will be applied to the camera to get it to follow the player
	private bool inBattle;
	// Use this for initialization
	void Start () {
		RB = GetComponent<Rigidbody>();
		var player = GameObject.Find ("Player");
		var gameManager = GameObject.Find ("GameManager");
			inBattle = gameManager.GetComponent<gameManager> ().inBattle;
		if (inBattle) {
			moveSpeed = 20;
		}else{
			moveSpeed = player.GetComponent<WorldMovementControls>().moveSpeed*4.2f; // takes max "move speed" which is really a force from the Player so we can match it.
		}
		TR = GetComponent<Transform> ();
		TR.position = gameManager.GetComponent<gameManager>().playerMapPosition + Idealpos; // read camera default position
	}

	// Update is called once per frame
	void Update () {
		var player = GameObject.Find ("Player");
		Vector3 Playpos = player.transform.position; // get Player position
		Vector3 Campos = transform.position; // get Camera Position
		input = new Vector3(0,0,0); // reset input force

		//Here decides which direction gets a force applied, and later how much will be applied if we try to smooth this out.  
		// Compares Camera position with the spot initialX and initialZ away from the Player .  Based on where the camera is with respect to this desired position a force will be applied.
		if (Playpos [0] + Idealpos[0] > Campos [0] + buffer) {
			input [0] = 1;
		}else if(Playpos [0] + Idealpos[0] < Campos [0] - buffer){
			input[0] = -1;
		}
		if (Playpos [2] + Idealpos[2] > Campos [2] + buffer) {
			input [2] = 1;
		}else if(Playpos [2] + Idealpos[2] < Campos [2] - buffer){
			input[2] = -1;
		}
			
		RB.AddForce(input*moveSpeed);// actually apply the input force to the camera

	}
}