using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAnimations : MonoBehaviour {
	public GameObject sphere;
	public Vector3 spherePos;
	public GameObject[] cubes;
	public float[] angleXZ;
	public float[] angleY;
	public Vector3 tempPos;
	public float targetDistance;
	public Vector3 axisUp;
	public float rotationSpeed;
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
	}
	
	// Update is called once per frame
	void Update () { 
		gameObject.transform.GetChild (0).rotation = rotation;
		spherePos = sphere.transform.position;
		for (int i = 0; i < 3; i++) {
			// get relative position of 
			//tempPos = cubes [i].transform.position - spherePos;
			//print(tempPos.x.ToString()+"  ASDFGHJKL");
			//tempPos = rotateClockwise(tempPos);

			cubes [i].transform.RotateAround (spherePos, axisUp, rotationSpeed * Time.deltaTime);
			//cubes [i].transform.position = tempPos + spherePos;
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
}
