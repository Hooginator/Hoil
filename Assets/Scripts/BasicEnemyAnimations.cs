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
	// Current position management
	public GameObject sphere;
	public Vector3 spherePos;
	public GameObject[] cubes;
	Vector3 topOrientation;
	float sphereDelta;

	// Target position management
	public Vector3 centralPos;
	Vector3 desiredTopOrientation;
	public List<GameObject> desiredPositions;
	public List<Quaternion> baseRotations;
	public List<Vector3> basePositions;

	// Constants
	public Vector3 axisUp;
	public float rotationSpeed;
	float recoilAcceleration;
	float maxRadiansTopRotation;

	// Animation Management
	public float targetDistance;
	public string animationType;
	public int level;
	bool topRotation;
	bool recoiling;
	Vector3 recoilSpeed; 
	Vector3 recoilDirection;

	void Start () {
		cubes = new GameObject[3];
		desiredPositions = new List<GameObject> ();
		sphere = gameObject.transform.GetChild(1).gameObject;
		var tempTR = new GameObject();
		desiredPositions.Add(Instantiate(tempTR,sphere.transform.position,sphere.transform.rotation));
		basePositions.Add (new Vector3 (0, 0, 0));
		baseRotations.Add (new Quaternion (0, 0, 0, 0));
		for (int i = 0; i < 3; i++) {
			cubes[i] = gameObject.transform.GetChild(i+2).gameObject;
			desiredPositions.Add(Instantiate(tempTR,cubes[i].transform.position,cubes[i].transform.rotation));
			basePositions.Add (cubes[i].transform.position - sphere.transform.position);
			baseRotations.Add (cubes[i].transform.rotation);
		}
		targetDistance = 3;
		axisUp = new Vector3 (0, 1, 0);
		rotationSpeed = 100f;
		maxRadiansTopRotation = 0.03f * rotationSpeed * Time.deltaTime;
		topOrientation = axisUp;
		desiredTopOrientation = axisUp;
		animationType = "normal";
		topRotation = false;
		recoiling = false;
		recoilAcceleration = 1.2f;
		sphereDelta = 0;

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

	void setInitialPositions(){

	}
	// Update is called once per frame
	void Update () { 
		spherePos = sphere.transform.position;
		if (animationType == "normal") {
			for (int i = 0; i < 3; i++) {
				desiredPositions [i+1].transform.RotateAround (spherePos, topOrientation, rotationSpeed * Time.deltaTime);
				desiredPositions [i+1].transform.position += new Vector3(0,0.02f*Mathf.Sin (Time.frameCount * 0.02f),0);
			}
			// Trying to get a floating height that varies slowly with time.  Currently not playing nice with Recoil

			desiredPositions[0].transform.position += new Vector3(0,0.02f*Mathf.Sin (Time.frameCount * 0.02f),0);
		} else if (animationType == "dying") {
			for (int i = 0; i < 3; i++) {
				desiredPositions [i+1].transform.RotateAround (spherePos, topOrientation, 3 * rotationSpeed * Time.deltaTime);
				desiredPositions [i+1].transform.Translate(topOrientation * Mathf.Sin(0.1f*Time.frameCount) * 0.4f * rotationSpeed * Time.deltaTime);
			}
		} else if (animationType == "casting") {
			for (int i = 0; i < 3; i++) {
				desiredPositions [i+1].transform.RotateAround (spherePos, topOrientation, 4 * rotationSpeed * Time.deltaTime);
			}
		}
		if (recoiling) {
			for (int i = 0; i < 4; i++) {
				desiredPositions [i].transform.position += recoilSpeed;
			}
			// distance from where the sphere is and it is supposed to be
			sphereDelta = Vector3.Magnitude(desiredPositions[0].transform.position - centralPos);
			recoilSpeed += 0.2f*getReturningSphereSpeed (sphereDelta);
			recoilSpeed *= 0.2f;
			if (sphereDelta < 0.1f) {
				recoiling = false;
				desiredPositions[0].transform.position = centralPos;
			}
		}
		if (topRotation) {
			if (topOrientation == desiredTopOrientation) {
				topRotation = false;
			} else {
				topOrientation = Vector3.RotateTowards (topOrientation, desiredTopOrientation, maxRadiansTopRotation, 1);
				for (int i = 0; i < 3; i++) {
						desiredPositions [i+1].transform.RotateAround (spherePos, Vector3.Cross (topOrientation, desiredTopOrientation), (180 / Mathf.PI) * maxRadiansTopRotation);
				}
			}
		}
		goToDesired ();

	}
	public void kill(){
		animationType = "dying";
	}
	public void resetDesired(){
		// resets to default animation position
		desiredPositions [0].transform.position = centralPos;
		desiredPositions [0].transform.rotation = baseRotations[0];
		for (int i = 0; i < 3; i++) {
			desiredPositions[i+1].transform.position = basePositions[i+1] + centralPos;
			desiredPositions[i+1].transform.rotation = baseRotations[i+1];
		}
	}
	public void goToDesired(){
		// resets enemy to currently desired position
		sphere.transform.position = desiredPositions[0].transform.position;
		sphere.transform.rotation = desiredPositions[0].transform.rotation;
		//print (desiredPositions [0].transform.position.ToString ());
		for (int i = 0; i < 3; i++) {
			cubes[i].transform.position = desiredPositions[i+1].transform.position; 
			cubes[i].transform.rotation = desiredPositions[i+1].transform.rotation; 
			}
	}
	public void setPos(){
		// Sets Central pos to current position.  For use after character is moved to another tile.
		setPos (sphere.transform.position);
	}
	public void setPos(Vector3 newPos){
		// Sets Central pos to given position.  For use after character is moved to another tile.
		for (int i = 0; i < desiredPositions.Count; i++) {
			desiredPositions [i].transform.position += newPos - centralPos;
		}
		centralPos = newPos;
	}
	public void castTowards(Vector3 target,float timeToCast){
		// Do casting animation towards target location
		// Change where the top of our unit faces to be where the target of the cast is.
		setTopVector(target);
		animationType = "casting";
		// Set up what will happen once the casting animation is done and the spell is fired
		StartCoroutine(fireIn(timeToCast,target));
	}

	IEnumerator fireIn(float t,Vector3 target){
		// Gives animations a second to go off before starting next turn
		yield return new WaitForSeconds (t);
		recoilFrom (-target);
		StartCoroutine(setDefaultIn(0.5f));
	}

	IEnumerator setDefaultIn(float t){
		// Gives animations a second to go off before starting next turn
		yield return new WaitForSeconds (t);
		animationType = "normal";
		setTopVector (new Vector3 (0, 1, 0));
	}

	Vector3 getReturningSphereSpeed(float dist){
		// Returns the vector pointing the sphere to where it is supposed to go that gets larget as the sphere is farther waway
		return Vector3.Normalize(centralPos - desiredPositions[0].transform.position) * dist;
	}
	public void setTopVector(Vector3 newTop){
		desiredTopOrientation = newTop;
		desiredTopOrientation.Normalize();
		topRotation = true;
	}
	public void recoilFrom(Vector3 initialSpeed){
		recoiling = true;
		recoilSpeed = 0.2f*initialSpeed;
	}
	public void recoilFromIn(Vector3 initialSpeed,float t){
		StartCoroutine(recoilIn(initialSpeed,t));
	}
	IEnumerator recoilIn(Vector3 initialSpeed,float t){
		// Gives animations a second to go off before recoiling fromm effect
		yield return new WaitForSeconds (t);

		recoilFrom(initialSpeed);
	}

	Vector3 rotateClockwise(Vector3 relativePos){
		
		float angle = getXZAngle (relativePos);
		angle += 0.01f;
		relativePos = getPosFromXZAngle (angle);
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
		return pos;
	}
	Vector3 scaleToTargetDistance(Vector3 pos){
		pos = pos * targetDistance;
		return pos;
	}

	// Update level indicator
	public void updateLevelIndicator(){
		GameObject levelText = gameObject.transform.GetChild(0).gameObject;
		levelText.GetComponent<MeshRenderer>().enabled = true;
		levelText.GetComponent<TextMesh>().text = level.ToString ();
		levelText.GetComponent<MeshRenderer>().enabled = true;
	}
	public void hideLevelIndicator(){
		GameObject levelText = gameObject.transform.GetChild(0).gameObject;
		levelText.GetComponent<MeshRenderer>().enabled = false;
	}
	public void updateLevel(int lvl){
		level = lvl;
		updateLevelIndicator ();
	}
}
