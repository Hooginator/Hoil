using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallCastAnimation : MonoBehaviour {

	string currentStage;
	Transform TR;
	int t;
	float travelDistance;
	int travelTime;
	int castTime;
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
		castTime = 30;
		travelTime = 30;
		distPerFrame = travelDistance / travelTime;
		reColour(new Color(1,1,0));
	}
	
	// Update is called once per frame
	void Update () {
		t++;
		if (currentStage == "Conjuring") {
			// Do conjuring animation
			if (t > castTime) {
				currentStage = "Traveling";
			}
		}else if (currentStage == "Traveling") {
			currentPos = Vector3.MoveTowards(currentPos, stop, distPerFrame);


			if (t > castTime + travelTime) {
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
