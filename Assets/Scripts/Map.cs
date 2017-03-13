using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
	//Size of the Map's grid
	public int NcellX;
	public int NcellZ;
	// X and Z dimensional size of each grid unit used used to space them out appropriately.
	public int gridSize = 10;
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
	// Boundaries of the map
	private float Xmin;
	private float Zmin;
	private float Xmax;
	private float Zmax;
	// RNG for map generation
	System.Random random = new System.Random();

	// Game Manager Object
	private gameManager gameMan;

	// Use this for initialization
	void Awake () {
		// Set Boundaries of the map
		Xmin =  -0.5f*gridSize;
		Zmin =  -0.5f*gridSize;
		Xmax =  (NcellX-0.5001f)*gridSize;
		Zmax =  (NcellZ-0.5001f)*gridSize;

		// Initialize arrays
		resources = new float[uniqueResources];
		tiles = new GameObject[NcellX,NcellZ];


		gameMan = GameObject.Find ("GameManager").GetComponent<gameManager>();

		// Loop through all grid places to be used for initialization
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {
				// Create instance of prefab
				tiles[x,z] = Instantiate (Prefab, new Vector3 (x*10, 0, z*10), Quaternion.identity);
				// Add MapGridUnit to prefab (maybe could just be in prefab?)
				tiles[x,z].AddComponent<MapGridUnit>();
				// Assign Map.cs as the parent of the Map Tiles
				tiles[x,z].transform.SetParent (this.transform);
				// Function to randomly assign resources
				if (NcellX >= 1 && NcellZ >= 1) {
					resources = getResources ((float)x / (NcellX - 1), (float)z / (NcellZ - 1));
				} else { // To prevent divide by zero if there's a 1xwhatever size map
					resources = getResources (0.5f, 0.5f);
				}
				// Initialize MapGridUnit with the resource values.
				tiles[x,z].GetComponent<MapGridUnit>().Initialize(resources,uniqueResources,maxResources);

			}
		}
		// Move some resources around so that there isn't such a sharp contrast
		/*for (int i = 0; i < 1; i++) {
			//redistributeResources ();
			redistributeResources ();
		}*/
		// Apply Colour to each tile
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {
				print (x.ToString () + "  " + NcellX.ToString ());
				print (z.ToString () + "  " + NcellZ.ToString ());
				tiles [x, z].GetComponent<MapGridUnit> ().reColour ();
			}
		}
	}
	public GameObject getTileFromPos(Vector3 pos){
		int posX = (int) Mathf.Floor((pos [0] - Xmin) / gridSize);
		int posZ = (int) Mathf.Floor((pos [2] - Zmin) / gridSize);
		print (posX.ToString () + "  " + posZ.ToString ());
		return tiles [posX, posZ];
	}
	float[] getResources(float x, float z){
		// X and Z are the amount through the map (0 to 1)
		//print (x.ToString() + "  " + z.ToString ());
		// Function to generate the number of each resource that will be in a tile once it spawns. 
		float[] generatedResources = new float[uniqueResources];
		// RNG WAY I'M LEAVING OUT FOR NOW
		/*for (int i = 0; i < uniqueResources; i++) {

			int randomNumber = random.Next(0, maxResources);
			generatedResources [i] = randomNumber;
		}*/
		if(gameMan.inBattle){
			generatedResources = gameMan.groundTileResources;
		}else{

			// Gives gradient that is strong at the extreme but dies off rather quickly so we have uncontested space between teams
			generatedResources [0] = (int) maxResources * Mathf.Pow((x + z)/2f,5);
			generatedResources [1] = (int) maxResources * Mathf.Pow(1f-(x + z)/2f,5);
		}
		return generatedResources;
	}

	void redistributeResources(){
		// If we are not on the border, do trade with nearest neighbour
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {
				if (x != 0) {
					tradeResources (x, x - 1, z, z);
				}
				if (z != 0) {
					tradeResources (x, x, z-1, z);
				}
				if (x != NcellX-1) {
					tradeResources (x, x + 1, z, z);
				}
				if (z != NcellZ-1) {
					tradeResources (x, x, z, z+1);
				}
			}
		}
	}
	public Vector3 ForceInsideBoundaries(Vector3 pos){
		// takes a vector and places it barely within the borders if it is outside.
		print("Force "+pos.ToString()+ " inside " + Xmax.ToString()+" "+Xmin.ToString());
		if (pos [0] < Xmin) {
			pos [0] = Xmin;
		} else if (pos [0] > Xmax) {
			pos [0] = Xmax;
		}
		if (pos [2] < Zmin) {
			pos [2] = Zmin;
		} else if (pos [2] > Zmax) {
			pos [2] = Zmax;
		}
		return pos;
	}
	public Vector3 MirrorInsideBoundaries(Vector3 pos){
		// Takes a vector and mirrors it inside the boundaries if it is outside.  The mirroring is done along the edge of the map in each dimension
		if (pos [0] < Xmin) {
			pos [0] = 2*Xmin-pos[0];
		} else if (pos [0] > Xmax) {
			pos [0] = 2*Xmax-pos[0];
		}
		if (pos [2] < Zmin) {
			pos [2] = 2*Zmin-pos[2];
		} else if (pos [2] > Zmax) {
			pos [2] = 2*Zmax-pos[2];
		}
		return pos;
	}


	void tradeResources(int x1, int x2, int y1, int y2){
		// Difference between the two grid units' resources
		int rDiff;
		// Square root of that difference for changing values
		int sqrtDiff;
		// Loop over all resources
		for (int i = 0; i < uniqueResources; i++) {
			// Difference in resources
			rDiff = (int)(tiles [x1, y1].GetComponent<MapGridUnit> ().resources [i] - tiles [x2, y2].GetComponent<MapGridUnit> ().resources [i]);
			if (rDiff != 0) {
				// Sqrt of difference so that everything doesn't get flat too fast
				sqrtDiff = (int)Mathf.Sqrt (Mathf.Abs (rDiff)) * rDiff / Mathf.Abs (rDiff);
			} else {
				sqrtDiff = 0;
			}
			// Update Resources
			tiles [x1, y1].GetComponent<MapGridUnit> ().resources [i] -= sqrtDiff;
			tiles [x2, y2].GetComponent<MapGridUnit> ().resources [i] += sqrtDiff;
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
