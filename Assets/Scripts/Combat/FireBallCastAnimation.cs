using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallCastAnimation : MonoBehaviour {

	string currentStage;
	Transform TR;
	Transform aura1;
	Transform aura2;
	float rotationSpeed;
	int t;
	float travelDistance;
	int travelFrames;
	int castFrames;
	float castTime;
	float travelTime;
	float distPerFrame;
	Vector3 currentPos;
	Vector3 start;
	Vector3 stop;
	// Use this for initialization
	void Start () {
		TR = this.transform;
	}
	public void init(Vector3 startPos, Vector3 stopPos){
		TR = this.transform;
		start = startPos;
		stop = stopPos;
		currentPos = startPos;
		TR.position = currentPos;
		currentStage = "Conjuring";
		t = 0;
		travelDistance = Vector3.Distance (startPos, stopPos);
		castFrames = 130;
		travelFrames = 30;
		castTime = castFrames * Time.deltaTime;
		travelTime = travelFrames * Time.deltaTime;
		distPerFrame = travelDistance / travelFrames;
		//reColour(new Color(1,1,0));
		foreach (Transform tempTR in TR) {
			var temp = tempTR.GetComponent<ParticleSystem> ().main;
			if (tempTR.CompareTag ("aura")) {
				temp.duration = castTime;
			} else if (tempTR.CompareTag ("travel")) {
				temp.startDelay = castTime;
				temp.duration = travelTime;
			} else if (tempTR.CompareTag ("landing")) {
				temp.startDelay = castTime + travelTime;
			}
		}
		// Rotation
		//aura1 = TR.FindChild("Aura1");
		//aura2 = TR.FindChild("Aura2");
		//var temp1 = aura1.GetComponent<ParticleSystem> ().main;
		//temp1.duration = castTime;
		//var temp2 = aura1.GetComponent<ParticleSystem> ().main;
		//temp2.duration = castTime;
		//rotationSpeed = 100.2f;
	}
	
	// Update is called once per frame
	void Update () {
		t++;
		if (currentStage == "Conjuring") {
			// Do conjuring animation
			if (t > castFrames) {
				currentStage = "Traveling";
			}
			// Rotation attempt..
			//aura1.Rotate (Vector3.up, rotationSpeed);
			//aura1.Rotate (Vector3.up, -rotationSpeed);
		}else if (currentStage == "Traveling") {
			currentPos = Vector3.MoveTowards(currentPos, stop, distPerFrame);


			if (t > castFrames + travelFrames) {
				currentStage = "Landing";

			}
		}
		TR.position = currentPos;
	}

	void reColour(Color newColour){
		bool done = false;
		int i = 0;
		foreach (Transform tempTR in TR){
			var temp = tempTR.GetComponent<ParticleSystem> ().main;
			temp.startColor = newColour;
			i++;
		}
	}



}
