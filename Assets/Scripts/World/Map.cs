using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***********************************************************/
// Map management for the square grid
// Contains functions for range checks, line of sight (soon)
// Manages initialization and evolution of MAP GRID UNIT resources.
// has functions to help manage positions on the map (set in middle of tile, getTile..)
// Changes MAP GRIUD UNIT colour for AoE selction
/***********************************************************/

public class Map : MonoBehaviour {
	// Used to create a 2D grid of square tiles making up a map.  This class is used for both the world and combat map

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


	// Update is called once per frame
	void Update () {
	}

	/********************************************************************************************/
	/************************************* Initialization ***************************************/
	/********************************************************************************************/

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
		// Apply Colour to each tile
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {
				//print (x.ToString () + "  " + NcellX.ToString ());
				//print (z.ToString () + "  " + NcellZ.ToString ());
				tiles [x, z].GetComponent<MapGridUnit> ().reColour ();
			}
		}
	}



	/********************************************************************************************/
	/******************************** Tile Management *******************************************/
	/********************************************************************************************/

	public void setInRange(int x, int z, int range){
		// Will recolour the tiles within range of the position (x,z) to the "In range" colour
		for (int i = -range; i < range+1; i++){
			for (int j = - (range - Mathf.Abs (i)); j < (range - Mathf.Abs (i)) + 1; j++) {
				if (isIntInBoundaries (i+x, j+z)) {
					tiles [i+x, j+z].GetComponent<MapGridUnit> ().setInRange ();
					//print ("HERE I AM <<<<<<<<");
				}
			}
		}
	}
	public void setOutOfRange(int x, int z, int range){
		// Will recolour the tiles within range of the position (x,z) back to the normal colour
		for (int i = -range; i < range+1; i++){
			for (int j = - (range - Mathf.Abs (i)); j < (range - Mathf.Abs (i)) + 1; j++) {
				if (isIntInBoundaries (i+x, j+z)) {
					tiles [i+x, j+z].GetComponent<MapGridUnit> ().setOutOfRange ();
				}
			}
		}
	}

	public bool isInRange(int x1, int z1, int x2, int z2, int range ){
		// Checks if [x1,z1] is within range of [x2,z2]
		return (Mathf.Abs(x1-x2) + Mathf.Abs(z1-z2)) <= range;
	}
	public bool isOccupied(int x,int z){
		return tiles [x, z].GetComponent<MapGridUnit> ().isOccupied;
	}

	public void selectRange(Vector3 pos, int range){
		int[] posInt = getTileCoordsFromPos (pos);
		int x = posInt [0];
		int z = posInt [1];
		for (int i = -range; i < range+1; i++){
			for (int j = - (range - Mathf.Abs (i)); j < (range - Mathf.Abs (i)) + 1; j++) {
				if (isIntInBoundaries (i+x, j+z)) {
					tiles [i+x, j+z].GetComponent<MapGridUnit> ().Select ();
					//print ("HERE I AM <<<<<<<<");
				}
			}
		}
		tiles [x, z].GetComponent<MapGridUnit> ().centralSelect ();
	}
	public void deSelectRange(Vector3 pos, int range){
		int[] posInt = getTileCoordsFromPos (pos);
		int x = posInt [0];
		int z = posInt [1];
		for (int i = -range; i < range+1; i++){
			for (int j = - (range - Mathf.Abs (i)); j < (range - Mathf.Abs (i)) + 1; j++) {
				if (isIntInBoundaries (i+x, j+z)) {
					tiles [i+x, j+z].GetComponent<MapGridUnit> ().reColour ();
					//print ("HERE I AM <<<<<<<<");
				}
			}
		}
	}
	public void deSelectAll(){
		//todo
	}
	/********************************************************************************************/
	/******************************** Tile Position Management **********************************/
	/********************************************************************************************/

	public GameObject getTileFromPos(Vector3 pos){
		// returns the GameObject of the Tile which is under position given
		//print("Get Tile From Pos");
		int[] posInt = getTileCoordsFromPos(pos);
		//print (posInt[0].ToString () + "  " + posInt[1].ToString ());
		return tiles [posInt[0], posInt[1]];
	}
	public int[] getTileCoordsFromPos(Vector3 pos){
		// Gives the coordinates od the tile under a position
		int[] posInt = new int [2];
		posInt [0] = (int)Mathf.Floor ((pos [0] - Xmin) / gridSize);
		posInt [1] = (int) Mathf.Floor((pos [2] - Zmin) / gridSize);
		return posInt;
	}
	public Vector3 centreInTile(Vector3 pos){
		// Centres the Vector in its current tile.
		Vector3 temppos = getTileFromPos(pos).transform.position;
		// Keep Height
		temppos.y = pos.y;
		return temppos;

	}
	public int getIntDistance(Vector3 pos1, Vector3 pos2){
		// Returns the integer distence between two locations.
		int[] posInt1 = getTileCoordsFromPos (pos1);
		int[] posInt2 = getTileCoordsFromPos (pos2);
		return (Mathf.Abs (posInt1 [0] - posInt2 [0]) + Mathf.Abs (posInt1 [1] - posInt2 [1]));
	}
	public int getIntDistanceFromCoords(int[] pos1, int[] pos2){
		// Returns the integer distence between two locations.
		return (Mathf.Abs (pos1 [0] - pos2 [0]) + Mathf.Abs (pos1 [1] - pos2 [1]));
	}
	public Vector3 getPosFromCoords(int x, int z){
		// Returns the central position of a tile based on int inputs
		return tiles [x,z].transform.position;
	}
	public Vector3 getAbovePosFromCoords(int x, int z){
		// Returns the central position of a tile based on int inputs
		Vector3 temp =  tiles [x,z].transform.position;
		temp [1] = 5;
		return temp;
	}



	/********************************************************************************************/
	/******************************** Boundary Management ***************************************/
	/********************************************************************************************/

	public bool isPosInBoundaries(Vector3 pos){
		// Checks if the position is out of bounds
		return !(pos [0] < Xmin || pos [0] > Xmax || pos [2] < Zmin || pos [2] > Zmax);
	}
	public bool isIntInBoundaries(int x, int z){
		// Checks if the Int combo is beyond the map tiles
		return !(x < 0 || x > NcellX-1 || z < 0 || z > NcellZ-1);
	}

	public Vector3 ForceInsideBoundaries(Vector3 pos){
		// takes a vector and places it barely within the borders if it is outside.
		//print("Force "+pos.ToString()+ " inside " + Xmax.ToString()+" "+Xmin.ToString());
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

	/********************************************************************************************/
	/******************************** Resource Management ***************************************/
	/********************************************************************************************/

	float[] getResources(float x, float z){
		// Takes two floats in between 0 and 1, numerically showing how far across the map this is.
		// Called once the MAP is loaded to generate default resources for a certain spot.
		float[] generatedResources = new float[uniqueResources];

		// RNG WAY I'M LEAVING OUT FOR NOW
		/*for (int i = 0; i < uniqueResources; i++) {

			int randomNumber = random.Next(0, maxResources);
			generatedResources [i] = randomNumber;
		}*/

		if(gameMan.inBattle){
			// When in battle we will take the tile colour where from where we started the battle.
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

	void tradeResources(int x1, int x2, int y1, int y2){
		// Does an averaging out of the resources in spots 1 and 2
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


}
