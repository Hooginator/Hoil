﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGridUnit : MonoBehaviour {
	public float[] resources;// array of resource value of grid
	public Material shad;// to let me change tile colour
	private int uniqueResources;
	private int maxResources;
	public Renderer rend;

	// Use this for initialization
	void Start () {
		// WANT: Create Map tile game object from an Asset (I think)
		// Change Colour?
		// Add terrain that belongs to one map (Mountains, trees, buildings)
		// Array of shit that impacts the resources (adjacency? over/under use?)
	}

	public void Initialize(float[] resourcesIn, int uniqueResourcesIn, int maxResourcesIn){
		uniqueResources = uniqueResourcesIn;
		rend = GetComponent<Renderer>();
		resources = new float[uniqueResourcesIn];
		resources = resourcesIn;
		maxResources = maxResourcesIn;
		if(uniqueResources >= 3){
			rend.material.color = new Color(resources[0]/maxResources,resources[1]/maxResources,resources[2]/maxResources,255);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// WANT: reevaluate over/under use and alter resource value 
		// Is it close to player? If not maybe destroy it until you're close again.
	}
}