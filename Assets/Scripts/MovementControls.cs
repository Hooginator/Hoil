using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControls : MonoBehaviour {
	public float moveSpeed;
	public Rigidbody RB;

	private Vector3 input;
	// Use this for initialization
	void Start () {
		RB = GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {
		input = new Vector3 (Input.GetAxis ("Horizontal"), 0.0f, Input.GetAxis ("Vertical"));
		RB.AddForce(input*moveSpeed);
		print (input);
	}
}
