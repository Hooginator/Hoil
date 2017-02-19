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

	// "Red" or "Blue" right now
	public string team;

	// Boolean that will disable movement if this army is tied up in battle with another
	public bool isMoving;

	// Battle Stats
	public int level;

	// points to prefab of this object so I can pass that to a battle.
	public GameObject prefab;


	void Start () {
		//level = Random.Range(3,25);
		// Update display of level on the model
		updateLevelIndicator ();
		RB = GetComponent<Rigidbody> ();
		TR = GetComponent<Transform> ();
		current = TR.position;
		// Select location to walk towards
		NewTarget ();
		RotationSpeed = 0.1f;
		isMoving = true;
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
		if (isMoving) {
			tempPos = (Vector3.MoveTowards (current, target, maxDistanceDelta * moveSpeed));
			TR.position = tempPos;
			current = tempPos;
			difference = current - target;
			// If we are close enough to the target location, find a new target.
			if (difference.magnitude < 0.0001) {
				NewTarget ();
			}
			// Move angle towards where eemy is going
			Vector3 angle = Vector3.RotateTowards (TR.forward, difference, RotationSpeed, 2.0F);
			// Apply angle
			TR.rotation = Quaternion.LookRotation (angle);
		}

	}
	void doBattle(GameObject enemy){
		// Battle with another computer enemy.  For now it just checks which is bigger and kills the other one, later they will stall for a while and maybe let you join the battle
		int enemylevel = enemy.GetComponent<EnemyBehavior> ().level;
		if (enemylevel > level) {
			print (team + " army loses D:");
			// Reduce level of team
			GameObject.Find("GameManager").GetComponent<gameManager>().ReduceTeamLevel(level, team);
			GameObject.Destroy (gameObject);
		} else if (enemylevel > level) {
			print (team + " army wins :D");
			// reduce level of enemy team
			GameObject.Find("GameManager").GetComponent<gameManager>().ReduceTeamLevel(enemylevel, enemy.GetComponent<EnemyBehavior> ().team);
			GameObject.Destroy (enemy);
		} else {
			// Still locks enemy movement, I will add to this later
			print ("tie game");
		}
	}

	void NewTarget(){
		// Determines a new location, within maxTargetDistance of the enemy for the enemy to move towards.
		float randomNumber1 = Random.Range(-maxTargetDistance, maxTargetDistance);
		float randomNumber2 = Random.Range(-maxTargetDistance, maxTargetDistance);
		target = new Vector3(current[0]+randomNumber1,2,current[2]+randomNumber2);

		// Mirrors any target that would be outside the boundary back in, this makes the guy not just slide across the edge
		var map = GameObject.Find ("Map");
		target = map.GetComponent<Map> ().MirrorInsideBoundaries (target);
	}

	void OnCollisionEnter(Collision col){
		// Only if what we collided with was an enemy, and we are still moving around
		if (isMoving && col.gameObject.GetComponent<EnemyBehavior>() != null) {
			// check if its an enemy
			if(col.gameObject.GetComponent<EnemyBehavior>().team != team){
				// Find game manager as that has the loading and unloading functions
				var gameManager = GameObject.Find ("GameManager");
				// If we're not already in battle, load it up. 
				if (gameManager.GetComponent<gameManager> ().inBattle != true) {
					print ("Enemies hit each other!@#$!");
					isMoving = false;
					doBattle (col.gameObject);
					//gameManager.GetComponent<SceneManager> ().UnLoadScene ("Hoil");
				}
			}
		}
	}
}
