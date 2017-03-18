using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMovementControls : MonoBehaviour {
	// Lets you take control of the character on the World Map.  

	public float moveSpeed; // max Force that can be applied to object
	public Rigidbody RB; // needed to apply forces to object.
	public Transform TR; // Needed to rotate object
	public int RotationSpeed; // How fast player can turn towards target
	private Vector3 input = new Vector3(0,0,1); // Vector for the user input for direction.  Default set to ensure Player has a default facing direction.
	private Vector3 currentPos; // temp position to calculate where to go each frame.

	/********************************************************************************************/ 
	/**************************************** Upkeep ********************************************/ 
	/********************************************************************************************/

	void Update () {
		// Read input from user and turn it into a vector
		input = new Vector3 (Input.GetAxis ("Horizontal"), 0.0f, Input.GetAxis ("Vertical"));
		// Apply force to Player
		RB.AddForce(input*moveSpeed); 

		// Get angle to have the character look towards
		Vector3 angle = Vector3.RotateTowards(transform.forward, input, RotationSpeed, 0.0F);
		// Apply angle
		TR.rotation = Quaternion.LookRotation(angle);

		// Keep player within World map
		var map = GameObject.Find ("Map");
		currentPos = TR.position;
		// Take any vector outside the map and set it to the nearest in-bounds position, also use the Keep at Y2 so we don't go flying off
		TR.position = map.GetComponent<Map> ().ForceInsideBoundaries (keepAtY2(currentPos));

		/*// Check which tile you are on
		MapGridUnit currentTile = map.GetComponent<Map> ().getTileFromPos(currentPos).GetComponent<MapGridUnit>();
		currentTile.resources [0] = 1;
		currentTile.resources [1] = 1;
		currentTile.reColour ();*/

	}

	Vector3 keepAtY2(Vector3 todo){
		// Force the vector to stay on the Y = 2 plane
		todo.y = 2;
		return todo;
	}

	/********************************************************************************************/ 
	/************************************* Initialization ***************************************/ 
	/********************************************************************************************/

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

	/********************************************************************************************/ 
	/************************************* Hit Detection ****************************************/ 
	/********************************************************************************************/

	void OnCollisionEnter(Collision col){
		print ("Collision at:  " + col.collider.transform.position.ToString ());
		// Only if what we collided with was an enemy
		if (col.gameObject.GetComponent<EnemyBehavior>() != null) {
			// Find game manager as that has the loading and unloading functions
			var gameManager = GameObject.Find ("GameManager");
			// If we're not already in battle, load it up. 
			if (gameManager.GetComponent<gameManager> ().inBattle != true) {
				print ("Gonna Load Up Battle");
				gameManager.GetComponent<gameManager> ().StartBattle(col.gameObject);
				//gameManager.GetComponent<SceneManager> ().UnLoadScene ("Hoil");
			}
		}
	}
}
