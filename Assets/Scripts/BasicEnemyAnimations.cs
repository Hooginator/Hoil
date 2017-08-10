﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***********************************************************/
// Dictates how the 3 outer pieces rotate around the central piece for the basix enemy.
// This will be attached to the prefab of each basic enemy following these animations.
// I want to create a general "Character Animations" that each individual Animation will inherit from.
// general animation will hold the level indicator and hold spots for the animation calls I'll need (death, attack, power up..)
/***********************************************************/

public class BasicEnemyAnimations : MonoBehaviour {
	// Central Sphere
	public GameObject sphere;
	public Vector3 spherePos;

	public GameObject[] cubes;
	public float[] angleXZ;
	public float[] angleY;
	public Vector3 centralPos;
	public float targetDistance;
	public Vector3 axisUp;
	public float rotationSpeed;
	public string animationType;
	public int level;
	// Use this for initialization
	Vector3 topOrientation;
	Vector3 desiredTopOrientation;
	bool topRotation;
	float maxRadiansTop;
	void Start () {
		cubes = new GameObject[3];
		sphere = gameObject.transform.GetChild(1).gameObject;
		for (int i = 0; i < 3; i++) {
			cubes[i] = gameObject.transform.GetChild(i+2).gameObject;
		}
		targetDistance = 3;
		axisUp = new Vector3 (0, 1, 0);
		rotationSpeed = 100f;
		topOrientation = axisUp;
		desiredTopOrientation = axisUp;
		animationType = "normal";
		topRotation = false;

		// Update display of level on the model
		var gameMan = GameObject.Find ("GameManager");
		if (gameMan.GetComponent<gameManager> ().checkInBattle()) {
			// Don't display level when in battle (it makes things cluttered)
			hideLevelIndicator ();
		} else {
			// Put the text of character's level above head in world map.
			updateLevelIndicator ();

		}
		//setTopVector(new Vector3(1,-99,1));

	}
	
	// Update is called once per frame
	void Update () { 
		//gameObject.transform.GetChild (0).rotation = topRotation;
		spherePos = sphere.transform.position;
		if (animationType == "normal") {
			for (int i = 0; i < 3; i++) {
				cubes [i].transform.RotateAround (spherePos, topOrientation, rotationSpeed * Time.deltaTime);
			}
		} else if (animationType == "dying") {
			for (int i = 0; i < 3; i++) {
				cubes [i].transform.RotateAround (spherePos, topOrientation, 3 * rotationSpeed * Time.deltaTime);
				cubes [i].transform.Translate(topOrientation * Mathf.Sin(0.1f*Time.frameCount) * 0.4f * rotationSpeed * Time.deltaTime);
			}
		} else if (animationType == "casting") {
			for (int i = 0; i < 3; i++) {
				cubes [i].transform.RotateAround (spherePos, topOrientation, 3 * rotationSpeed * Time.deltaTime);
			}
		}
		if (topRotation) {
			maxRadiansTop = 0.04f * rotationSpeed * Time.deltaTime;
			print (maxRadiansTop.ToString () + "   " + Time.frameCount);
			if (topOrientation == desiredTopOrientation) {
				//print ("DONE WITH ROTATINGGGGGG");
				topRotation = false;
			} else {
				topOrientation = Vector3.RotateTowards (topOrientation, desiredTopOrientation, maxRadiansTop, 1);
				for (int i = 0; i < 3; i++) {
					cubes [i].transform.RotateAround (spherePos, Vector3.Cross (topOrientation, desiredTopOrientation), (180 / Mathf.PI) * maxRadiansTop);
				}
			}
		}

	}
	public void castTowards(Vector3 target){
		// Do cast animation towards target location
		setTopVector(target);
		animationType = "casting";
		StartCoroutine(setDefaultIn(1));
	}

	IEnumerator setAnimationTypeIn(string type, float t){
		// Gives animations a second to go off before starting next turn
		yield return new WaitForSeconds (t);
		animationType = type;
	}

	IEnumerator setDefaultIn(float t){
		// Gives animations a second to go off before starting next turn
		yield return new WaitForSeconds (t);
		animationType = "normal";
		setTopVector (new Vector3 (0, 1, 0));
	}


	public void setTopVector(Vector3 newTop){
		desiredTopOrientation = newTop;
		desiredTopOrientation.Normalize();
		topRotation = true;
	}

	Vector3 rotateClockwise(Vector3 relativePos){
		
		float angle = getXZAngle (relativePos);
		angle += 0.01f;
		//print (relativePos.ToString () + "   pos 1");
		relativePos = getPosFromXZAngle (angle);
		//print (relativePos.ToString () + "   pos 2");
		return relativePos;
	}
	float getXZAngle(Vector3 pos){
		float angle = Mathf.Asin (pos [0] / (Mathf.Sqrt (pos [0] * pos [0] + pos [2] * pos [2])));
		print ("From vec " + pos.ToString () + " to angle " + angle.ToString ());
		return angle;
	}
	Vector3 getPosFromXZAngle(float angle){
		Vector3 pos = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle));
		print ("From angle  "+  angle.ToString () + " to vec" + pos.ToString ());
		//pos = scaleToTargetDistance (pos);
		return pos;
	}
	Vector3 scaleToTargetDistance(Vector3 pos){
		pos = pos * targetDistance;
		return pos;
	}

	// Update level indicator
	public void updateLevelIndicator(){
		GameObject levelText = gameObject.transform.GetChild(0).gameObject;
		//print ("Update level strings");
		//levelText.SetActive(true);
		levelText.GetComponent<MeshRenderer>().enabled = true;
		levelText.GetComponent<TextMesh>().text = level.ToString ();
		levelText.GetComponent<MeshRenderer>().enabled = true;
	}
	public void hideLevelIndicator(){
		GameObject levelText = gameObject.transform.GetChild(0).gameObject;
		//levelText.SetActive (false);
		levelText.GetComponent<MeshRenderer>().enabled = false;
	}
	public void updateLevel(int lvl){
		level = lvl;
		updateLevelIndicator ();
	}

}
