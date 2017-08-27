using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateo_animation : MonoBehaviour {
	Transform TR;
	ParticleSystem PS;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		TR = gameObject.transform;
		PS = gameObject.GetComponent<ParticleSystem> ();
		var PSmain = PS.main.startRotation.constant;
		PSmain = Time.frameCount;
		TR.rotation = Quaternion.AngleAxis(Time.frameCount,new Vector3(1,0,0));
	}
}
