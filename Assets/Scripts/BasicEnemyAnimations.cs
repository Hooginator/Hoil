using System.Collections;
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
	Quaternion rotation;
	void Start () {
		cubes = new GameObject[3];
		sphere = gameObject.transform.GetChild(1).gameObject;
		for (int i = 0; i < 3; i++) {
			cubes[i] = gameObject.transform.GetChild(i+2).gameObject;
		}
		targetDistance = 3;
		axisUp = new Vector3 (0, 1, 0);
		rotationSpeed = 50f;
		rotation = Quaternion.identity;
		rotation.eulerAngles = new Vector3(70, 0, 0);
		animationType = "normal";

		// Update display of level on the model
		var gameMan = GameObject.Find ("GameManager");
		if (gameMan.GetComponent<gameManager> ().checkInBattle()) {
			// Don't display level when in battle (it makes things cluttered)
			hideLevelIndicator ();
		} else {
			// Put the text of character's level above head in world map.
			updateLevelIndicator ();

		}

	}
	
	// Update is called once per frame
	void Update () { 
		gameObject.transform.GetChild (0).rotation = rotation;
		spherePos = sphere.transform.position;
		if (animationType == "normal") {
			for (int i = 0; i < 3; i++) {
				cubes [i].transform.RotateAround (spherePos, axisUp, rotationSpeed * Time.deltaTime);
			}
		} else if (animationType == "dying") {
			for (int i = 0; i < 3; i++) {
				cubes [i].transform.RotateAround (spherePos, axisUp, 3 * rotationSpeed * Time.deltaTime);
				cubes [i].transform.Translate(axisUp * Mathf.Sin(0.1f*Time.frameCount) * 0.4f * rotationSpeed * Time.deltaTime);
			}
		}
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
