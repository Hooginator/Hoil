using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGridUnit : MonoBehaviour {
	public float[] resources;// array of resource value of grid
	public Material shad;// to let me change tile colour
	private int uniqueResources;
	private int maxResources;
	public Renderer rend;
	public int reduceSaturation; // max saturation for a grid unit

	// Use this for initialization
	void Start () {
		reduceSaturation = 180;
		if (reduceSaturation >= uniqueResources) {
			reduceSaturation = uniqueResources - 1;
		}
		// WANT: Create Map tile game object from an Asset (I think)
		// Change Colour?
		// Add terrain that belongs to one map (Mountains, trees, buildings)
		// Array of shit that impacts the resources (adjacency? over/under use?)
	}

	public void Initialize(float[] resourcesIn, int uniqueResourcesIn, int maxResourcesIn){
		uniqueResources = uniqueResourcesIn;
		// Needed for colour
		rend = GetComponent<Renderer>();
		// Set resources to what was input
		resources = new float[uniqueResourcesIn];
		resources = resourcesIn;
		maxResources = maxResourcesIn;
		// Change colour
		if(uniqueResources >= 3){
			rend.material.color = new Color((int) (resources[0]/(maxResources-reduceSaturation)),(int) (resources[1]/maxResources),0,255);
		}
	}

	public void reColour(){
		// Reapply the colour based on resources
		rend = GetComponent<Renderer>();
		if(uniqueResources >= 3){
			rend.material.color = new Color(resources[0]/maxResources,resources[1]/maxResources,resources[2]/maxResources,255);
		}
		if (uniqueResources == 2) {
			rend.material.color = new Color((resources[0]/(maxResources-reduceSaturation)),0,resources[1]/(maxResources-reduceSaturation),255);
		}
	}
	// Update is called once per frame
	void Update () {
		// WANT: reevaluate over/under use and alter resource value 
		// Is it close to player? If not maybe destroy it until you're close again.
	}
}
