using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraControls : MonoBehaviour {
	public float moveSpeed; // Set to match the palyer
	public float bufferx;
	public float bufferz; // buffer is the max distance away from the player the acmera will not move at
	public Vector3 Idealpos; // Initial camera position so we know a good relative distance to the player.  WIll be replaced by 1 distance for the camera if we do cam rotation
	public Rigidbody RB; // needed to add force (ie move) the camera
	public Transform TR; // Needed to move object
	private Vector3 input; // the Force that will be applied to the camera to get it to follow the player
	private bool inBattle;



	// Update is called once per frame
	void Update () {
		var gameManager = GameObject.Find ("GameManager");
		var player = gameManager.GetComponent<gameManager>().worldPlayer;
		Vector3 Playpos = player.transform.position; // get Player position
		Vector3 Campos = TR.position; // get Camera Position
		//input = new Vector3(0,0,0); // reset input force

		//Here decides which direction gets a force applied, and later how much will be applied if we try to smooth this out.  
		// Compares Camera position with the spot initialX and initialZ away from the Player .  Based on where the camera is with respect to this desired position a force will be applied.
		input = new Vector3 ( Input.GetAxis("Cam_Right") - Input.GetAxis("Cam_Left") , 0 ,  Input.GetAxis("Cam_Up") - Input.GetAxis("Cam_Down") );
		//print (Playpos [0].ToString ()+ "   " + Campos [0].ToString() + "    " + bufferx.ToString());
		RB.AddForce(input*moveSpeed);// actually apply the input force to the camera

	}

	/********************************************************************************************/ 
	/************************************* Initialization ***************************************/ 
	/********************************************************************************************/


	// Use this for initialization
	void Start () {
		RB = GetComponent<Rigidbody>();
		var gameManager = GameObject.Find ("GameManager");
		var player = gameManager.GetComponent<gameManager>().worldPlayer;
		inBattle = gameManager.GetComponent<gameManager> ().inBattle;
		if (inBattle) {
			moveSpeed = 80;
		}else{
			moveSpeed = player.GetComponent<WorldMovementControls>().moveSpeed*4.2f; // takes max "move speed" which is really a force from the Player so we can match it.
		}
		TR = GetComponent<Transform> ();
		// Position that the camera will always aim for
		Idealpos = new Vector3(0,50,0);
		// Distance away we will start going towards idealpos
		bufferx = 28;
		bufferz = 8;
	}
}