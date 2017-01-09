using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
	//Size of the Map's grid
	public int NcellX;
	public int NcellZ;
	// Number of Unique Resources that will be in the game
	public int uniqueResources;
	// Prefabricated Tile Game Object used for the map visual
	public GameObject Prefab;
	// Array of resources to be determined and applied to each Grid Unit on initialization
	private float[] resources;
	// Maximum number of resources available to one grid unit
	private int maxResources = 255;
	// 2D Array of the tiles used for the map
	public GameObject[,] tiles;

	System.Random random = new System.Random();
	// Use this for initialization
	void Start () {
		// Initialize arrays
		resources = new float[uniqueResources];
		tiles = new GameObject[NcellX,NcellZ];
		// Loop through all grid places to be used for initialization
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {
				// Create instance of prefab
				tiles[x,z] = Instantiate (Prefab, new Vector3 (x, 0, z), Quaternion.identity);
				// Add MapGridUnit to prefab (maybe could just be in prefab?)
				tiles[x,z].AddComponent<MapGridUnit>();
				// Assign Map.cs as the parent of the Map Tiles
				tiles[x,z].transform.SetParent (this.transform);
				// Function to randomly assign resources
				resources = getResources (x, z);
				// Initialize MapGridUnit with the resource values.
				tiles[x,z].GetComponent<MapGridUnit>().Initialize(resources,uniqueResources,maxResources);

			}
		}
	}
	float[] getResources(int x, int z){
		// Function to generate the number of each resource that will be in a tile once it spawns. 
		float[] generatedResources = new float[uniqueResources];
		for (int i = 0; i < uniqueResources; i++) {
			int randomNumber = random.Next(0, maxResources);
			generatedResources [i] = randomNumber;
		}
		return generatedResources;
	}
	// Update is called once per frame
	void Update () {
	}
}
