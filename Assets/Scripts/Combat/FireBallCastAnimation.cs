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
	float speed;
	int areaSize;
	Vector3 currentPos;
	Vector3 casterPos;
	// conjuringPos For where a fireball would emerge, not right on the caster.
	Vector3 conjuringPos;
	Vector3 stop;
	// Use this for initialization
	void Start () {
		TR = this.transform;
	}
	public void init(Vector3 startPos, Vector3 stopPos, ColourPalette colourPalette,int size,float timeToCast,float timeToLand){
		TR = this.transform;
		casterPos = startPos;
		stop = stopPos;
		conjuringPos = Vector3.MoveTowards (casterPos, stopPos, 0.5f);
		areaSize = size;
		currentPos = conjuringPos; 
		TR.position = currentPos;
		currentStage = "Conjuring";
		t = 0;
		travelDistance = Vector3.Distance (startPos, stopPos);
		//Debug.Log (" TRAVEL DISTANCE" + travelDistance.ToString ());
		castFrames = (int) (timeToCast *60);// hardcoded average fps :D
		travelFrames = (int) (timeToLand*60);
		//Debug.Log ("Cast frames: " +  Time.deltaTime.ToString () + "   Travel Frames: " + travelFrames.ToString ());
		speed = travelDistance  / timeToLand;
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
				temp.duration = timeToCast;
				tempPS.Play ();
				temp.startColor = colourPalette.getColour (auraCount);
				auraCount++;
			} else if (tempTR.CompareTag ("travel")) {
				tempPS.Stop ();
				temp.startDelay = timeToCast;
				temp.duration = timeToLand;
				tempPS.Play ();
				temp.startColor = colourPalette.getColour (travelCount);
				travelCount++;
			} else if (tempTR.CompareTag ("landing")) {
				tempPS.Stop ();
				temp.startDelay = timeToCast + timeToLand;
				float oldSpeed = temp.startSpeed.constant;
				// Adjust explosion radius
				temp.startSpeed = oldSpeed * Mathf.Pow( size,1.2f);
				tempPS.Play ();
				temp.startColor = colourPalette.getColour (landingCount);
				landingCount++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		t++;
		if (currentStage == "Conjuring") {
			// Do conjuring animation, no movement
			if (t > castFrames) {
				// Done Conjuring
				currentStage = "Traveling";
				currentPos = conjuringPos;
			}
		}else if (currentStage == "Traveling") {
			// Move
			currentPos = Vector3.MoveTowards(currentPos, stop, speed * Time.deltaTime);
			if (t > castFrames + travelFrames) {
				// Done Traveling
				currentStage = "Landing";
				currentPos = stop;
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
