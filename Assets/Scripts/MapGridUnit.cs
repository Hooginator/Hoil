using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGridUnit : MonoBehaviour {
	public float[] resources;// array of resource value of grid

	// Use this for initialization
	void Start () {
		// WANT: Create Map tile game object from an Asset (I think)
		// Change Colour?
		// Add terrain that belongs to one map (Mountains, trees, buildings)
		// Array of shit that impacts the resources (adjacency? over/under use?)
	}

	public void Initialize(float[] resourcesIn, int uniqueResourcesIn){
		resources = new float[uniqueResourcesIn];
		resources = resourcesIn;
	}
	
	// Update is called once per frame
	void Update () {
		// WANT: reevaluate over/under use and alter resource value 
		// Is it close to player? If not maybe destroy it until you're close again.
	}
}
