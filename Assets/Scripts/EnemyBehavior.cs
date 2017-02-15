using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {
	public float moveSpeed; // max Force that can be applied to object
	public Rigidbody RB; // needed to apply forces to object.
	public Transform TR; // Needed to rotate object
	public Vector3 target; // Where the little guy will be walking towards
	private float maxTargetDistance = 10f;
	public Vector3 current; // current position
	private Vector3 tempPos; // temp position vector so I can do the math before applying it.
	private Vector3 difference;
	private float maxDistanceDelta = 0.1f;
	private float RotationSpeed;
	// Use this for initialization
	public Color colour;

	public string team;

	// Battle Stats
	public int level;

	public GameObject prefab;


	void Start () {
		//level = Random.Range(3,25);
		updateLevelIndicator ();
		RB = GetComponent<Rigidbody> ();
		TR = GetComponent<Transform> ();
		current = TR.position;
		NewTarget ();
		RotationSpeed = 0.1f;
		//moveSpeed = 0.2f;
	}
	// Update level indicator
	public void updateLevelIndicator(){
		GameObject levelText = gameObject.transform.GetChild(0).gameObject;
		print ("Update level strings");
		levelText.GetComponent<TextMesh>().text = level.ToString ();
	}


	// Update is called once per frame
	void Update () {
		// Moves enemy towards the target location.
		tempPos =  (Vector3.MoveTowards(current, target, maxDistanceDelta*moveSpeed));
		TR.position = tempPos;
		current = tempPos;
		difference = current - target;
		// If we are close enough to the target location, find a new target.
		if (difference.magnitude < 0.0000001) {
			NewTarget ();
		}
		// Move angle towards where eemy is going
		Vector3 angle = Vector3.RotateTowards(TR.forward, difference, RotationSpeed, 2.0F);
		// Apply angle
		TR.rotation = Quaternion.LookRotation(angle);

	}

	void NewTarget(){
		// Determines a new location, within maxTargetDistance of the enemy for the enemy to move towards.
		float randomNumber1 = Random.Range(-maxTargetDistance, maxTargetDistance);
		float randomNumber2 = Random.Range(-maxTargetDistance, maxTargetDistance);
		target = new Vector3(current[0]+randomNumber1,current[1],current[2]+randomNumber2);
		var map = GameObject.Find ("Map");
		// Mirrors any target that would be outside the boundary back in, this makes the guy not just slide across the edge
		target = map.GetComponent<Map> ().MirrorInsideBoundaries (target);
	}
}
