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

	private List<int[]> cubeDirections;

	// Update is called once per frame
	void Update () {
	}

	/********************************************************************************************/
	/************************************* Initialization ***************************************/
	/********************************************************************************************/

	// Use this for initialization
	void Awake () {
		// Set Boundaries of the map in space
		Xmin =  -0.5f*gridSize;
		Zmin =  -0.5f*gridSize;
		Xmax =  (NcellX-0.5001f)*gridSize;
		Zmax =  (NcellZ-0.5001f)*gridSize;

		// Initialize arrays
		resources = new float[uniqueResources];
		tiles = new GameObject[NcellX,NcellZ];
		setCubeDirections ();
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
				} else { // To prevent divide by zero if there's a [1 * whatever] size map
					resources = getResources (0.5f, 0.5f);
				}
				print (resources.ToString ());
				// Initialize MapGridUnit with the resource values.
				tiles[x,z].GetComponent<MapGridUnit>().Initialize(resources,uniqueResources,maxResources);

			}
		}
		// Apply Colour to each tile
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {
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
		return getIntDistanceFromCube(cartesianToCube(pos1), cartesianToCube(pos2));
	}

	public int getIntDistanceFromCube(int[] cube1, int[] cube2){
		// Returns the integer distence between two locations.
		// HEX
		return cubeDistance(cube1, cube2);
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
		// Returns the integer distance between two cube positions
		return (Mathf.Abs (cube1 [0] - cube2 [0]) + Mathf.Abs (cube1 [1] - cube2 [1]) + Mathf.Abs (cube1 [2] - cube2 [2])) / 2;

	}
	public bool isInRange(int x1, int z1, int x2, int z2, int range ){
		// HEX
		// Checks if [x1,z1] is within range of [x2,z2]
		return (getIntDistanceFromCoords(new int[] {x1,z1},new int[] {x2,z2} ) <= range);


	}
	public bool isCubeInRange(int[] cube1,int[] cube2,int range){
		// HEX
		// Checks if two cube positions are within Range distance of each other
		return(getIntDistanceFromCube (cube1, cube2) <= range);
	}

	public List<int[]> getInRange(int[] cubePos, int range){
		// HEX
		// provides a list of tile coordinates in cube within range of cubePos
		// Does not check on boundaries
		List<int[]> tempList = new List<int[]> ();
		for(int i = -range;i < range +1; i++){
			for (int j = Mathf.Max (-range, -i - range); j < Mathf.Min (range, -i + range) +1; j++) {
				tempList.Add (new int[]{i + cubePos[0],j + cubePos[1],-i-j + cubePos[2]} );
			}
		}
		return tempList;
	}
	public List<int[]> getOpposingNeighbours(int[] cubePos, int[] source){
		// Returns the list of neighbours that are in the opposite direction as source.
		// WAYYYYYYYYYYYYYYYYYYY Ineffidient  AND doesn't even do it right!
		// Maybe start with everything and remove blocked squares
		List<int[]> tempList = new List<int[]> ();
		int[] relativePos = new int[3];







		int dist = getIntDistanceFromCube (cubePos, source);
		for (int i = 0; i < 3; i++) {
			relativePos [i] = source [i] - cubePos [i];
		}
		// Take care of the 6 perfect diagonals first
		if (relativePos [0] == 0) {
			if (relativePos [1] > 0) {
				tempList.Add (new int[]{ cubePos [0], cubePos [1] - 1, cubePos [2] + 1 });
			} else {
				tempList.Add (new int[]{ cubePos [0], cubePos [1] + 1, cubePos [2] - 1 });
			}
			/*if (dist < 2) {
				// short range, add more.
				if (relativePos [1] > 0) {
					tempList.Add (new int[]{ cubePos [0] - 1, cubePos [1], cubePos [2] + 1 });
					tempList.Add (new int[]{ cubePos [0] + 1, cubePos [1] - 1, cubePos [2] });
				} else {
					tempList.Add (new int[]{ cubePos [0] - 1, cubePos [1]+1, cubePos [2] });
					tempList.Add (new int[]{ cubePos [0] + 1, cubePos [1], cubePos [2] -1 });
				}
			}*/
		} else if (relativePos [1] == 0) {
			if (relativePos [0] > 0) {
				tempList.Add (new int[]{ cubePos [0] - 1, cubePos [1], cubePos [2] + 1 });
			} else {
				tempList.Add (new int[]{ cubePos [0] + 1, cubePos [1], cubePos [2] - 1 });
			}
			/*if (dist < 2) {
				// short range, add more.
				if (relativePos [0] > 0) {
					tempList.Add (new int[]{ cubePos [0]-1, cubePos [1]+1, cubePos [2] });
					tempList.Add (new int[]{ cubePos [0], cubePos [1]-1, cubePos [2]+1});
				} else {
					tempList.Add (new int[]{ cubePos [0], cubePos [1]+1, cubePos [2] - 1 });
					tempList.Add (new int[]{ cubePos [0] + 1, cubePos [1] - 1, cubePos [2] });
				}
			}*/
		} else if (relativePos [2] == 0) {
			if (relativePos [1] > 0) {
				tempList.Add (new int[]{ cubePos [0] + 1, cubePos [1] - 1, cubePos [2] });
			} else {
				tempList.Add (new int[]{ cubePos [0] - 1, cubePos [1] + 1, cubePos [2] });
			}
			/*if (dist < 2) {
				// short range, add more.
				if (relativePos [1] > 0) {
					tempList.Add (new int[]{ cubePos [0]+1, cubePos [1], cubePos [2] - 1 });
					tempList.Add (new int[]{ cubePos [0], cubePos [1]-1, cubePos [2] +1});
				} else {
					tempList.Add (new int[]{ cubePos [0], cubePos [1]+1, cubePos [2] - 1 });
					tempList.Add (new int[]{ cubePos [0] - 1, cubePos [1], cubePos [2] + 1 });
				}
			}*/
			// with no zeros we're not on a straight line and so will unlock 2 tiles opposite
		} else {
			if (relativePos [0] > 0) {
				if (relativePos [1] > 0) {
					tempList.Add (new int[]{ cubePos [0] - 1, cubePos [1], cubePos [2] + 1 });
					tempList.Add (new int[]{ cubePos [0], cubePos [1] - 1, cubePos [2] + 1 });
				} else {
					if (relativePos [2] > 0) {
						tempList.Add (new int[]{ cubePos [0] - 1, cubePos [1] + 1, cubePos [2] });
						tempList.Add (new int[]{ cubePos [0], cubePos [1] + 1, cubePos [2] - 1 });
					}
				}
			} else {
				if (relativePos [1] < 0) {
					tempList.Add (new int[]{ cubePos [0] + 1, cubePos [1], cubePos [2] - 1 });
					tempList.Add (new int[]{ cubePos [0], cubePos [1] + 1, cubePos [2] - 1 });
				} else {
					if (relativePos [2] > 0) {
						tempList.Add (new int[]{ cubePos [0] + 1, cubePos [1] , cubePos [2] - 1 });
						tempList.Add (new int[]{ cubePos [0] + 1, cubePos [1] - 1, cubePos [2] });
					}
				}

			}
		}
		if (tempList.Count == 0) {
			Debug.Log ("No neighbours for pos:  " + cubePos [0].ToString () + "  " + cubePos [1].ToString () + "  " + cubePos [2].ToString () + "  "
			+ "     from:   " + source [0].ToString () + "  " + source [1].ToString () + "  " + source [2].ToString ());
		} else {
			Debug.Log ("Found opposing neighbours!");

		}
		return tempList;
	}

	public List<int[]> setInLineOfSight(int[] cubePos, int range){
		List<int[]> toSet = getInLineOfSight (cubePos,range);
		int[] tempCart;
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				tiles [tempCart [0], tempCart [1]].GetComponent<MapGridUnit> ().setInRange ();
			}
		}
		return toSet;
	}

	bool isLineOfSightBlocked(int[] pos1, int[] pos2, List<int[]> blockers){
		// MISSSING LINK!!!!!
		for (int i = 0; i < blockers.Count; i++) {
			if (isLineOfSightBlocked (pos1, pos2, blockers [i])) {
				return true;
			}
		}
		return false;
	}

	bool isLineOfSightBlocked(int[] pos1, int[] pos2, int[] blockers){
		// individual chaeck for if a LOS is blocked
		return false;
	}

	public List<int[]> getInLineOfSight(int[] cubePos, int range){

		// new crack at it

		List<int[]> tempList = getInRange (cubePos, range);
		List<int[]> finalList = new List<int[]> ();
		List<int[]> blockers = new List<int[]> ();

		for (int i = 0; i < tempList.Count; i++) {
			if (isLineOfSightBlocked (tempList [i], cubePos, blockers)) {
				// Do nothing
			} else {
				finalList.Add (tempList [i]);
				if(isIntInBoundaries(tempList[i]) && isOccupied(tempList[i])){
					blockers.Add(tempList[i]);
				}
			}
		}




		/*




		List<int[]> finalList = new List<int[]> ();
		List<int[]> tempList = new List<int[]> ();
		List<int[]> toDoList = new List<int[]> ();
		toDoList.Add(cubePos);
		pathDist tempPathDist;
		int[] currentTile;
		bool done = false;
		int movementCount = 0;
		while (!done) {
			// check if we're out of MP
			if (movementCount >= range) {
				done = true;
				break;
			}
			// check if there are no more paths available
			if(toDoList.Count == 0){
				done = true;
				break;
			}
			// get nearest neighbours
			if (movementCount == 0) {
				tempList.AddRange (getInRange (cubePos, 1));
			} else {
				for (int j = 0; j < toDoList.Count; j++) {
					tempList.AddRange (	getOpposingNeighbours (toDoList [j], cubePos));
				}
			}
			toDoList = new List<int[]> ();
			for (int i = 0; i < tempList.Count; i++) {
				currentTile = tempList [i];
				// Check on Map boundaries
				if (isIntInBoundaries (cubeToCartesian (currentTile))) {
					// Check on Occupied status of tile
					if (!isOccupied (cubeToCartesian (currentTile)) ) {
						// NOTE: I used to have checks in here to ensure there were no duplicates but that performed very poorly
						// That being the case we will have duplicates in our list.  Might be worth sorting that out once at the end of the function.
						finalList.Add (currentTile);
						toDoList.Add (currentTile);
					}
				}
			}

			movementCount++;
			if (movementCount > 40) {
				Debug.Log ("Took too long looking for movement range");
				break;
			}
		}*/

		return finalList;


	}

	public List<int[]> getInMovementRange(int[] cubePos, int range){
		// HEX
		// WAYYYYYYYYYYYYYYYYYYYYYY TOO SLOW RIGHT NOW, *FIXED!!*
		// Grabs all neighbours of toDoList (starting at cubePos) and adds them to the finalList for returning
		// AND adds to the toDoList for the next pass.  passes are done until we have reached range

		List<int[]> finalList = new List<int[]> ();
		List<int[]> tempList = new List<int[]> ();
		List<int[]> toDoList = new List<int[]> ();
		toDoList.Add(cubePos);
		pathDist tempPathDist;
		int[] currentTile;
		bool done = false;
		int movementCount = 0;
		while (!done) {
			// check if we're out of MP
			if (movementCount >= range) {
				done = true;
				break;
			}
			// check if there are no more paths available
			if(toDoList.Count == 0){
				done = true;
				break;
			}
			// get nearest neighbours
			for (int j = 0; j < toDoList.Count; j++) {
				tempList.AddRange(getInRange (toDoList[j], 1));
			}
			toDoList = new List<int[]> ();
			for (int i = 0; i < tempList.Count; i++) {
				currentTile = tempList [i];
				// Check on Map boundaries
				if (isIntInBoundaries (cubeToCartesian (currentTile))) {
					// Check on Occupied status of tile
					if (!isOccupied (cubeToCartesian (currentTile)) ) {
						// NOTE: I used to have checks in here to ensure there were no duplicates but that performed very poorly
						// That being the case we will have duplicates in our list.  Might be worth sorting that out once at the end of the function.
						finalList.Add (currentTile);
						toDoList.Add (currentTile);
					}
				}
			}

			movementCount++;
			if (movementCount > 40) {
				Debug.Log ("Took too long looking for movement range");
				break;
			}
		}
			
		return finalList;
	}
	public void setInMovementRange(int[] cubePos, int range){
		// Will recolour the tiles within walking range of the CUBE position to the "In range" colour
		// HEX
		List<int[]> toSet = getInMovementRange (cubePos,range);
		int[] tempCart;
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				tiles [tempCart [0], tempCart [1]].GetComponent<MapGridUnit> ().setInRange ();
			}
		}
	}
	public void setInMovementRange(int x, int z, int range){
		int[] tempCube = cartesianToCube (new int[]{ x, z });
		setInMovementRange (tempCube, range);
	}

	public void setInRange(int x, int z, int range, string rangeType){
		// Will recolour the tiles within range of the position (x,z) to the "In range" colour
		// HEX
		List<int[]> toSet;
		if (rangeType == "LOS") {
			toSet = getInLineOfSight (cartesianToCube (new int[]{ x, z }),range);
		} else {
			toSet = getInRange (cartesianToCube (new int[]{ x, z }),range);
		}
		int[] tempCart;
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				tiles [tempCart [0], tempCart [1]].GetComponent<MapGridUnit> ().setInRange ();
			}
		}
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
		// Is there something on this cell that would block LoS or prevent someone from entering
		return tiles [x, z].GetComponent<MapGridUnit> ().isOccupied;
	}
	public bool isOccupied(int[] x){
		// Is there something on this cell that would block LoS or prevent someone from entering
		return tiles [x[0], x[1]].GetComponent<MapGridUnit> ().isOccupied;
	}
	public void setOccupied(int[] x){
		tiles [x[0], x[1]].GetComponent<MapGridUnit> ().isOccupied = true;
	}

	public void setUnOccupied(int[] x){
		tiles [x[0], x[1]].GetComponent<MapGridUnit> ().isOccupied = false;
	}
	public void selectRange(Vector3 pos, int range){
		// HEX
		// Setup map to select where to place attack with feedback in map colour
		int[] posInt = getTileCoordsFromPos (pos);
		selectRange (posInt, range);
	}

	public void selectRange(int[] posInt, int range){
		// HEX
		// Setup map to select where to place attack with feedback in map colour
		List<int[]> toSet = getInRange (cartesianToCube (posInt),range);
		int[] tempCart = new int[]{0,0};
		for (int i = 0; i < toSet.Count; i++) {
			tempCart = cubeToCartesian (toSet [i]);
			if (isIntInBoundaries(tempCart [0], tempCart [1])) {
				selectRangeUnit(tempCart [0], tempCart [1]);
			}
		}
		selectCentralUnit(posInt);
	}
	public void selectRangeUnit(int i,int j){
		// recolour cell to show that AoE will affect it
		tiles [i, j].GetComponent<MapGridUnit> ().Select ();
	}
	public void selectCentralUnit(int[] posIn){
		// Recolour central cell you are targeting
		tiles [posIn[0], posIn[1]].GetComponent<MapGridUnit> ().centralSelect ();
	}

	public void deSelectRange(Vector3 pos, int range){
		// Return all cells withing range of pos back to their original colours
		int[] posInt = getTileCoordsFromPos (pos);
		deSelectRange (posInt, range);
	}
	public void deSelectRange(int[] posInt, int range){
		// Return all cells withing range of pos back to their original colours
		// HEX
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
		// Return individual cell to its current colour (default, in range..)
		tiles [i, j].GetComponent<MapGridUnit> ().reColour ();
	}
	public void deSelectUnit(int[] i){
		// Return individual cell to its original colour
		deSelectUnit (i [0], i [1]);
	}
	public void resetUnit(int i, int j){
		// Change cell to no longer be in range, set it to its unaltered colour.
		tiles [i, j].GetComponent<MapGridUnit> ().setOutOfRange ();
	}
	public void deSelectAll(){
		// Change all cells on the map to no longer be in range, set it to its unaltered colour. 
		for (int i = 0; i < NcellZ; i++) {
			for (int j = 0; j < NcellX; j++) {
				resetUnit (i, j);
			}
		}
	}

	public GameObject getTile(int[] posInt){
		// Returns the tile at pos posInt in the array
		return tiles[posInt[0],posInt[1]];
	}


	public GameObject getTileFromPos(Vector3 pos){
		// HEX
		// returns the GameObject of the Tile which is under position given
		int[] posInt = getTileCoordsFromPos(pos);
		return tiles [posInt[0], posInt[1]];
	}
	public int[] getTileCoordsFromPos(Vector3 pos){
		// HEX
		int[] posInt = new int [2];
		// new coordinate system >_>  this is part way to cube, but I don't think extending all the way helps at all.
		posInt[1] = (int)(pos.z / offsetZ+0.5f);
		// If row is odd 
		bool rowIsOdd = posInt [1] % 2 == 1; 
		if (rowIsOdd) {
			posInt [0] = (int)((pos.x) / offsetX);
		} else {
			posInt [0] = (int)((pos.x + 0.5*offsetX) / offsetX);
		}

		float relativeZ = (pos.z+0.5f* offsetZ - (posInt[1] * offsetZ))/offsetZ;
		float relativeX;

		if (rowIsOdd) {
			relativeX = (pos.x - (posInt [0] - 0.5f) * offsetX)/offsetX;
		} else {
			relativeX = (pos.x - (posInt [0]) * offsetX)/offsetX;
		}
		if (relativeZ < (0.5f * relativeX) - 0.5f) {// bottom right
			posInt [1] -= 1;
			if (rowIsOdd) {
				posInt [0] += 1;
			}
		} else if (relativeZ < (-0.5f * relativeX) - 0.5f) {// bottom left
			posInt [1] -= 1;
			if (!rowIsOdd) {
				posInt [0] -= 1;
			}
		}
		// for testing
		//selectCentralUnit (posInt);

		return posInt;
	}

	public Vector3 centreInTile(Vector3 pos){
		// HEX
		// Centres the Vector in its current tile.
		Vector3 temppos = getTileFromPos(pos).transform.position;
		// Keep Height
		temppos.y = pos.y;
		return temppos;

	}
	public int getIntDistance(Vector3 pos1, Vector3 pos2){ 
		// HEX

		// Returns the integer distence between two locations.
		int[] posInt1 = getTileCoordsFromPos (pos1);
		int[] posInt2 = getTileCoordsFromPos (pos2);
		return getIntDistanceFromCoords(posInt1,posInt2);
	}

	public Vector3 getPosFromCoords(int x, int z){
		// Returns the central position of a tile based on int inputs
		// HEX
		return tiles [x,z].transform.position;
	}
	public Vector3 getAbovePosFromCoords(int x, int z){
		// Returns the central position of a tile based on int inputs
		// HEX
		return getAbovePosFromCoords(new int[]{x,z});
	}
	public Vector3 getAbovePosFromCoords(int[] x){
		// Returns the central position of a tile based on int inputs
		// HEX
		Vector3 temp =  tiles [x[0],x[1]].transform.position;
		temp [1] = 5;
		return temp;
	}


	/********************************************************************************************/
	/******************************** Tile Position Management **********************************/
	/********************************************************************************************/

	public struct pathDist
	{
		// structure used to determine the optimal path between points
		public int[] prev;
		public int distTraveled;
		public int distToGo;
		public pathDist(int[] prevIn,int distIn, int distToGoIn){
			this.prev = prevIn;
			this.distTraveled = distIn;
			this.distToGo = distToGoIn;
		}
	}

	public List<int[]> getPath(int[] pos1, int[]pos2, int maxDist, out int distMoved){
		// HEX PATHFINDING VERSION 1
		int[] currentPos = new int[]{pos1[0],pos1[1]};
		Dictionary<int[], pathDist> toCheck = new Dictionary<int[], pathDist>();
		toCheck.Add (currentPos,new pathDist(pos1,0, getIntDistanceFromCoords(pos1,pos2)));
		Dictionary<int[], pathDist> doneCheck = new Dictionary<int[], pathDist>();
		List<int[]> path = new List<int[]>();
		bool foundPath = false;
		int maxCycles = 20;
		int j = 0;
		int[] tempCube;
		int[] tempCart;
		int currentDistTraveled;
		int shortestDist;
		distMoved = 0;
		pathDist currentDist;
		pathDist tempPrev;

		while (!foundPath) {
			toCheck.TryGetValue (currentPos,out currentDist);
			currentDistTraveled = currentDist.distTraveled;
			for (int k = 0; k < 6; k++) {
				tempCube = getCubeNeighbour (cartesianToCube (currentPos), k);
				tempCart = cubeToCartesian (tempCube);
				//Debug.Log ("About to test cube: "+tempCube [0].ToString () + "    " + tempCube [1].ToString () + "    " + tempCube [2].ToString ());
				//Debug.Log ("About to test cart: "+currentPos [0].ToString () + "    " + currentPos [1].ToString ()  +"  against    " + pos2[0].ToString() + "  " + pos2[1].ToString());
				if (toCheck.TryGetValue (tempCart,out tempPrev) || doneCheck.TryGetValue(tempCart,out tempPrev)) {
					// Already been here, maybe check distances for shortes i
					Debug.Log("ALready been to " + tempCart[0].ToString() +"  " + tempCart[1].ToString());
				} else {
					// Add new node to check
					tempPrev = new pathDist(currentPos,currentDistTraveled+1,getIntDistanceFromCoords (tempCart, pos2));
					toCheck.Add (tempCart, tempPrev);
				}
				//Debug.Log("Path Finding checked cell:   " + tempCart[0].ToString() + "   " + tempCart[1].ToString());
			}
			shortestDist = 1000000;
			// Change currentPos to shortest distance left to check
			foreach (var tempPathDist in toCheck) {
				if (tempPathDist.Value.distTraveled + tempPathDist.Value.distToGo < shortestDist && isAvailable(tempPathDist.Key) && tempPathDist.Value.distTraveled < maxDist+1) {
					//Debug.Log ("For some reason NEVER HAPPENING!!!!!!!!!!!!!!!!!!!!!#%^*%@&!^^^^^^^^^^^^^^^^^^^^^%#^&$@#^&$(*&#@^(");
					shortestDist = tempPathDist.Value.distTraveled + tempPathDist.Value.distToGo;
					currentPos = tempPathDist.Key;
				}
			}

			if (toCheck.TryGetValue (currentPos, out currentDist)) {
				Debug.Log ("Done this round, currentPos: " + currentPos[0].ToString() + "   " + currentPos[1].ToString());
				Debug.Log ("Done this round, targetPos: " + pos2[0].ToString() + "   " + pos2[1].ToString());
				doneCheck.Add (currentPos, currentDist);
				toCheck.Remove (currentPos);
				if (currentPos[0] == pos2[0] && currentPos[1] == pos2[1] ) {
					// Found the end!!!!
					path.Add(currentPos);
					distMoved = 0;
					j = 0;
					while (!foundPath) {
						if (currentPos [0] == pos1 [0] && currentPos [1] == pos1 [1]) {
							foundPath = true;
							break;
						}
						if (doneCheck.TryGetValue (currentPos, out currentDist)) {
							// all good
							currentPos = currentDist.prev;
							distMoved += 1;
						} else {
							Debug.Log (" Could not find currentPos in our list of checked places");
						}
						Debug.Log ("Adding point " + currentPos [0].ToString () + "  " + currentPos [1].ToString () + "  to path");
						path.Add (currentPos);


						j++;
						if (j > maxCycles) {
							foundPath = true;
							Debug.Log ("Could not find path back fast enough");
						}
					}
				}
			} else {
				Debug.Log ("Path not found");
			}



			j++;
			if (j > maxCycles) {
				foundPath = true;
				Debug.Log ("Could not find path fast enough");
			}
		}
		return path;

	}
	public int[] moveTowards(int[] pos1,int[] pos2){
		// not HEX
		int[] newPos = pos1;
		int deltaX = pos1[0] - pos2[0];
		int deltaZ = pos1[1] - pos2[1];
		if (Mathf.Abs (deltaX) >= Mathf.Abs (deltaZ)) {
			if (pos1 [0] > pos2 [0]) {
				newPos [0] -= 1;
			} else if (pos1 [0] < pos2 [0]) {
				newPos [0] += 1;
			}
		} else if (Mathf.Abs (deltaX) <= Mathf.Abs (deltaZ)) {
			if (pos1 [1] > pos2 [1]) {
				newPos [1] -= 1;
			} else if (pos1 [1] < pos2 [1]) {
				newPos [1] += 1;
			}
		}
		return newPos;

	}


	public int[] getCubeNeighbour(int[] cubeIn,int direction){
		int[] cubeOut = addCube (cubeIn, cubeDirections [direction]);
		return cubeOut;
	}
	public int[] addCube(int[] cube1, int[] cube2){
		// adds two cube positions together
		int[] cubeOut = new int[3];
		for (int i = 0; i < 3; i++) {
			cubeOut [i] = cube1 [i] + cube2 [i];
		}
		return cubeOut;
	}
	public void setCubeDirections(){
		// setup a list of the 6 transformations I have to do to move to a neighbour
		cubeDirections = new List<int[]>();
		cubeDirections.Add (new int[]{ 1, -1, 0 });
		cubeDirections.Add (new int[]{ 1, 0, -1 });
		cubeDirections.Add (new int[]{ 0, -1, 1 });
		cubeDirections.Add (new int[]{ 0, 1, -1 });
		cubeDirections.Add (new int[]{ -1, 1, 0 });
		cubeDirections.Add (new int[]{ -1, 0, 1 });
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

	public bool isIntInBoundaries(int[] x){
		// Checks if the Int combo is beyond the map tiles
		return !(x[0] < 0 || x[0] > NcellX-1 || x[1] < 0 || x[1] > NcellZ-1);
	}
	public bool isAvailable(int[] x){
		if(isIntInBoundaries(x)){
			return !isOccupied (x);
		}
		return false;
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
			if (gameMan.groundTileResources != null) {
				generatedResources = gameMan.groundTileResources;
			} else {
				for(int i = 0; i < uniqueResources;i++){ 
					if(i%2 == 0){
						generatedResources[i] = (int) 255.0f * (x + z) / 2;
						print (generatedResources[i].ToString());
					}else{
						generatedResources[i] = (int) 255.0f * (2 - x - z) / 2;
					}
				}
			}
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
