using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bases : MonoBehaviour {
	public GameObject prefab;


	// Use this for initialization
	void Start () {
		prefab = GameObject.Find ("Base");
		Instantiate (prefab, new Vector3 (10, 5, 10), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
