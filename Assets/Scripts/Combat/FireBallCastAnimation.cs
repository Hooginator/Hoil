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
	int areaSize;
	Vector3 currentPos;
	Vector3 start;
	Vector3 stop;
	// Use this for initialization
	void Start () {
		TR = this.transform;
	}
	public void init(Vector3 startPos, Vector3 stopPos, ColourPalette colourPalette,int size){
		TR = this.transform;
		start = startPos;
		stop = stopPos;
		areaSize = size;
		currentPos = startPos;
		TR.position = currentPos;
		currentStage = "Conjuring";
		t = 0;
		travelDistance = Vector3.Distance (startPos, stopPos);
		castFrames = 30;
		travelFrames = 30;
		castTime = castFrames * Time.deltaTime;
		travelTime = travelFrames * Time.deltaTime;
		distPerFrame = travelDistance / travelFrames;
		//reColour(new Color(1,1,0));

		// Loop through all parts of the animation and change their starting time based on the calculated travel time
		// I will add the recolouring here as well 
		int auraCount = 0;
		int travelCount = 0;
		int landingCount = 0;
		foreach (Transform tempTR in TR) {
			var tempPS = tempTR.GetComponent<ParticleSystem> ();
			var temp = tempPS.main;

			if (tempTR.CompareTag ("aura")) {
				// Must stop particle system animation to change duration
				tempPS.Stop ();
				temp.duration = castTime;
				tempPS.Play ();
				temp.startColor = colourPalette.getColour (auraCount);
				auraCount++;
			} else if (tempTR.CompareTag ("travel")) {
				tempPS.Stop ();
				temp.startDelay = castTime;
				temp.duration = travelTime;
				tempPS.Play ();
				temp.startColor = colourPalette.getColour (travelCount);
				travelCount++;
			} else if (tempTR.CompareTag ("landing")) {
				tempPS.Stop ();
				temp.startDelay = castTime + travelTime;
				float oldSpeed = temp.startSpeed.constant;
				// Adjust explosion radius
				temp.startSpeed = oldSpeed * Mathf.Pow( size,1.2f);
				tempPS.Play ();
				temp.startColor = colourPalette.getColour (landingCount);
				landingCount++;
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

	public void reColour(Color newColour){
		bool done = false;
		int i = 0;
		foreach (Transform tempTR in TR){
			var temp = tempTR.GetComponent<ParticleSystem> ().main;
			temp.startColor = newColour;
			i++;
		}
	}



}
