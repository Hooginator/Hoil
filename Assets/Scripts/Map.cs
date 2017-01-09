using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
	public int NcellX;
	public int NcellZ;
	public int uniqueResources;
	//public MapGridUnit[,] MapGrid; 
	public GameObject Prefab;
	private GameObject tempGO; // temp object to have for each grid point so we can call the Initialize function in each
	private float[] resources;
	private int maxResources = 255;

	System.Random random = new System.Random();
	// Use this for initialization
	void Start () {
		resources = new float[uniqueResources]; // resources to be applied to each cell
		//MapGrid = new MapGridUnit[NcellX,NcellZ];
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {
				//print (x * 20);
				tempGO = Instantiate (Prefab, new Vector3 (x*10, 0, z*10), Quaternion.identity);
				MapGridUnit tempMGU = tempGO.AddComponent<MapGridUnit>();
				resources = getResources (x, z);
				tempMGU.Initialize(resources,uniqueResources,maxResources);
				print (resources[0]);
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
