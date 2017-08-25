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


	// HEX
	float offsetX = 20*0.8660254f;
	float offsetZ = 15;

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
		//float tilePosX;
		//float tilePosZ;
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {

				// HEX time!

				// Create instance of prefab
				tiles[x,z] = Instantiate (Prefab, calculateTilePosFromCoords(x,z), Quaternion.Euler(new Vector3(90,0,0)));
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
	/******************************** Tile Management HEX!! *************************************/
	/********************************************************************************************/

	Vector3 calculateTilePosFromCoords(int x,int z){
		// For HEX TILES
		float tempX;
		float tempZ;
		tempZ = z * offsetZ;
		if (z % 2 == 0) {
			// For even rows
			tempX = x*offsetX;
		} else {
			// For odd rows
			tempX = (x+0.5f)*offsetX;
		}
		return new Vector3 (tempX, 0, tempZ);
	}

	public int getIntDistanceFromCoords(int[] pos1, int[] pos2){
		// Returns the integer distence between two locations.
		/// HEX
		print ("WSERDFTVGYBUHNJIMK");
		return cubeDistance(cartesianToCube(pos1), cartesianToCube(pos2));
		//return (Mathf.Abs (pos1 [0] - pos2 [0]) + Mathf.Abs (pos1 [1] - pos2 [1]));
	}

	public int getIntDistanceFromCube(int[] cube1, int[] cube2){
		// Returns the integer distence between two locations.
		/// HEX
		return cubeDistance(cube1, cube2);
		//return (Mathf.Abs (pos1 [0] - pos2 [0]) + Mathf.Abs (pos1 [1] - pos2 [1]));
	}

	public int[] cubeToCartesian(int[] cube){
		// HEX
		// Change to cube coords to make maths easier
		int[] tempPos = new int[2];
		tempPos [0] = cube[0] + (cube[2] - cube[2] % 2) / 2;
		tempPos [1] = cube[2];
		return tempPos;
	}

	public int[] cartesianToCube(int[] tempPos){
		// HEX
		// Change to cube coords to make maths easier
		int[] tempCube = new int[3];
		tempCube [0] = tempPos[0] - (tempPos[1] - tempPos[1] % 2) / 2;
		tempCube [2] = tempPos[1];
		tempCube [1] = -tempCube[0]-tempCube[2];
		return tempCube;
	}
	public int cubeDistance(int[] cube1,int[] cube2){
		// HEX
		return (Mathf.Abs (cube1 [0] - cube2 [0]) + Mathf.Abs (cube1 [1] - cube2 [1]) + Mathf.Abs (cube1 [2] - cube2 [2])) / 2;

	}
	public bool isInRange(int x1, int z1, int x2, int z2, int range ){
		// Checks if [x1,z1] is within range of [x2,z2]
		//return (Mathf.Abs(x1-x2) + Mathf.Abs(z1-z2)) <= range;

		// HEX
		return (getIntDistanceFromCoords(new int[] {x1,z1},new int[] {x2,z2} ) <= range);


	}
	public bool isCubeInRange(int[] cube1,int[] cube2,int range){


		return(getIntDistanceFromCube (cube1, cube2) <= range);
	}

	public List<int[]> getInRange(int[] cubePos, int range){
		// HEX
		// provides a list of tile coordinates in cube within range of cubePos
		// Does not check on boundaries


		List<int[]> tempList = new List<int[]> ();
		// BRUTE FORCE, CHECK EVERYTHING
		/*int[] tempCube;
		for (int i = 0; i < NcellX; i++) {
			for (int j = 0; j < NcellZ; j++) {
				tempCube = cartesianToCube (new int[]{ i, j });
				print ("Cart: " + i.ToString() + " " + j.ToString() + " Cube: " + tempCube[0] + " " + tempCube[1] + " " + tempCube[2] + " ");
				if (isCubeInRange(cubePos,cartesianToCube(new int[]{i,j}),range)){

					tiles [i, j].GetComponent<MapGridUnit> ().setInRange ();
				}
			}
		}*/
		// BE ELEGANT
		for(int i = -range;i < range +1; i++){
			for (int j = Mathf.Max (-range, -i - range); j < Mathf.Min (range, -i + range) +1; j++) {
				tempList.Add (new int[]{i + cubePos[0],j + cubePos[1],-i-j + cubePos[2]} );
			}
		}
		return tempList;
	}

	public void setInRange(int x, int z, int range){
		// Will recolour the tiles within range of the position (x,z) to the "In range" colour
		// HEX
		List<int[]> toSet = getInRange (cartesianToCube (new int[]{ x, z }),range);
		int[] tempCart;
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				tiles [tempCart [0], tempCart [1]].GetComponent<MapGridUnit> ().setInRange ();
			}
		}

		/*for (int i = -range; i < range+1; i++){
			for (int j = - (range - Mathf.Abs (i)); j < (range - Mathf.Abs (i)) + 1; j++) {
				if (isIntInBoundaries (i+x, j+z)) {
					tiles [i+x, j+z].GetComponent<MapGridUnit> ().setInRange ();
					//print ("HERE I AM <<<<<<<<");
				}
			}
		}*/
	}
	public void setOutOfRange(int x, int z, int range){
		// Will recolour the tiles within range of the position (x,z) back to the normal colour
		// HEX
		List<int[]> toSet = getInRange (cartesianToCube (new int[]{ x, z }),range);
		int[] tempCart;
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				tiles [tempCart [0], tempCart [1]].GetComponent<MapGridUnit> ().reColour ();
			}
		}
	}

	public bool isOccupied(int x,int z){
		return tiles [x, z].GetComponent<MapGridUnit> ().isOccupied;
	}
	public bool isOccupied(int[] x){
		return tiles [x[0], x[1]].GetComponent<MapGridUnit> ().isOccupied;
	}

	public void selectRange(Vector3 pos, int range){
		int[] posInt = getTileCoordsFromPos (pos);
		int x = posInt [0];
		int z = posInt [1];
		List<int[]> toSet = getInRange (cartesianToCube (new int[]{ x, z }),range);
		int[] tempCart;
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				selectRangeUnit(tempCart [0], tempCart [1]);
			}
		}
		selectCentralUnit(x,z);
	}

	public void selectRange(int[] posInt, int range){
		List<int[]> toSet = getInRange (cartesianToCube (posInt),range);
		int[] tempCart = new int[]{0,0};
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				selectRangeUnit(tempCart [0], tempCart [1]);
			}
		}
		selectCentralUnit(posInt[0],posInt[1]);
	}
	public void selectRangeUnit(int i,int j){
		tiles [i, j].GetComponent<MapGridUnit> ().Select ();
	}
	public void selectCentralUnit(int i,int j){
		tiles [i, j].GetComponent<MapGridUnit> ().centralSelect ();
	}

	public void deSelectRange(Vector3 pos, int range){
		int[] posInt = getTileCoordsFromPos (pos);
		int x = posInt [0];
		int z = posInt [1];
		List<int[]> toSet = getInRange (cartesianToCube (new int[]{ x, z }),range);
		int[] tempCart;
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				deSelectUnit(tempCart [0], tempCart [1]);
			}
		}
	}
	public void deSelectRange(int[] posInt, int range){
		List<int[]> toSet = getInRange (cartesianToCube (posInt),range);
		int[] tempCart;
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				deSelectUnit(tempCart [0], tempCart [1]);
			}
		}
	}

	public void deSelectUnit(int i, int j){
		tiles [i, j].GetComponent<MapGridUnit> ().reColour ();
	}
	public void deSelectUnit(int[] i){
		deSelectUnit (i [0], i [1]);
	}
	public void deSelectAll(){
		for (int i = 0; i < NcellZ; i++) {
			for (int j = 0; j < NcellX; j++) {
				deSelectUnit (i, j);
			}
		}
	}

	public GameObject getTile(int[] posInt){
		return tiles[posInt[0],posInt[1]];
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

		// UNTESTED

		int[] posInt = new int [2];
		posInt [1] = (int) Mathf.Floor((pos[2]-0.5f*offsetZ) / offsetZ);
		if (posInt [1] % 2 == 0) {
			posInt [0] = (int)Mathf.Floor ((pos [0]+0.5f*offsetX) / offsetX - 0.5f);
		} else {
			posInt [0] = (int)Mathf.Floor ((pos [0]+0.5f*offsetX) / offsetX);
		}
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
		return getIntDistanceFromCoords(posInt1,posInt2);
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

	public List<int[]> getPath(int[] pos1, int[]pos2, int maxDist){
		int wiggleRoom = maxDist - getIntDistanceFromCoords (pos1, pos2);
		int[] currentPos = pos1;
		List<int[]> path = new List<int[]>();
		for(int i = 0;i<maxDist;i++){
			currentPos = moveTowards (currentPos, pos2);
			if (isOccupied (currentPos)) {
				Debug.Log ("Failed moving, someone in the way"+currentPos.ToString());
				return null;
			}
			path.Add (currentPos);
			if (currentPos == pos2) {
				break;
			}
		}
		return path;

	}
	public int[] moveTowards(int[] pos1,int[] pos2){
		int deltaX = pos1[0] - pos2[0];
		int deltaZ = pos1[1] - pos2[1];
		if (Mathf.Abs (deltaX) >= Mathf.Abs (deltaZ)) {
			if (pos1 [0] > pos2 [0]) {
				pos1 [0] -= 1;
			} else if (pos1 [0] < pos2 [0]) {
				pos1 [0] += 1;
			}
		} else if (Mathf.Abs (deltaX) <= Mathf.Abs (deltaZ)) {
			if (pos1 [1] > pos2 [1]) {
				pos1 [1] -= 1;
			} else if (pos1 [1] < pos2 [1]) {
				pos1 [1] += 1;
			}
		}
		return pos1;
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
	public int[] ForceIntInsideBoundaries(int[] pos){
		if (pos [0] < 0) {
			pos [0] = 0;
		} else if (pos [0] > NcellX-1) {
			pos [0] = NcellX-1;
		}
		if (pos [1] < 0) {
			pos [1] = 0;
		} else if (pos [1] > NcellZ-1) {
			pos [1] = NcellZ-1;
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

	public int[] MirrorInsideBoundaries(int[] pos){
		// mirrors coordinates back onto map
		// Should preserve range..  (IE if I have range N and the input pos is within N squares of me, so will the reflected point)
		if (pos [0] < 0) {
			pos [0] = -pos [0];
		}if (pos [0] > NcellX) {
			pos [0] =  2 * pos [0] - NcellX;
		}
		if (pos [1] < 0) {
			pos [1] = -pos [1];
		}if (pos [1] > NcellZ) {
			pos [1] = 2 * pos [1] - NcellZ;
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
