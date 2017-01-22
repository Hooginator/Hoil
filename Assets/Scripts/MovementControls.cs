using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControls : MonoBehaviour {
	public float moveSpeed; // max Force that can be applied to object
	public Rigidbody RB; // needed to apply forces to object.
	public Transform TR; // Needed to rotate object
	public int RotationSpeed;
	private Vector3 input;
	// Use this for initialization
	void Start () {
		RB = GetComponent<Rigidbody> ();
		TR = GetComponent<Transform> ();
		RotationSpeed = 5;
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
		var map = GameObject.Find ("World Map");
		TR.position = map.GetComponent<Map> ().ForceInsideBoundaries (TR.position);
	}
	void OnCollisionEnter(Collision col){
		// Only if what we collided with was an enemy
		if (col.gameObject.GetComponent<EnemyBehavior>() != null) {
			// Find game manager as that has the loading and unloading functions
			var gameManager = GameObject.Find ("GameManager");
			// If we're not already in battle, load it up. 
			if (gameManager.GetComponent<sceneManager> ().inBattle != true) {
				print ("Gonna Load Up Battle");
				gameManager.GetComponent<sceneManager> ().LoadScene ("Battle");
				//gameManager.GetComponent<SceneManager> ().UnLoadScene ("Hoil");
				gameManager.GetComponent<sceneManager> ().inBattle = true;
			}
		}
	}
}
