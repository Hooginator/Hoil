using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGridUnit : MonoBehaviour {
	public float[] resources;// array of resource value of grid
	public int uniqueResources; // Number of unique resources
	public Vector3 position;

	// Use this for initialization
	void Start () {
		resources = new float[uniqueResources]; // set initial resources from outside I think is best.
		transform.position = position;// Set position of Map unit
		// WANT: Create Map tile game object from an Asset (I think)
		// Change Colour?
		// Add terrain that belongs to one map (Mountains, trees, buildings)
		// Array of shit that impacts the resources (adjacency? over/under use?)
	}
	
	// Update is called once per frame
	void Update () {
		// WANT: reevaluate over/under use and alter resource value 
		// Is it close to player? If not maybe destroy it until you're close again.
	}
}
