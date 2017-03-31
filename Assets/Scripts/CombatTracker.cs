using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTracker : MonoBehaviour {
	// Management Functions for the combat flow.
	// Good Guys
	public List<CharacterClass> playerCharacters  = new  List<CharacterClass>();
	int maxPlayerCharacters;
	// Bad Guys
	public List<CharacterClass> enemyCharacters  = new  List<CharacterClass>();
	public int maxEnemyCharacters;

	// Prefabs of the visuals used for characters in battle
	public GameObject[] CharacterPrefabs;// = new GameObject[3];

	// List of Players and Enemies visuals in the current battle.
	public GameObject[] playerSprites;
	public GameObject[] enemySprites;

	public float experienceEarned;

	public Vector3 targetLocation;
	bool selectingTargetLocation;

	GameObject BattleMenu;

	public Map map;
	public Vector3 temppos;
	// Number of turns that have gone by, so I can kill the infinite loops witha  failsafe
	private int nTurns = 0;

	// Temp boolean to stop from pressing enter, ie "Submit" once and blasting through all the menus and target selection. 
	public bool wasUp;
	// Map coords for where a selection of a map tile originates from
	public int selectingFromX;
	public int selectingFromZ;
	public int selectingRange;
	public int areaRange;

	// Name of the action to be performed.  For when selecting a target, after we have chosen Attack, Item, Move etc..
	public string actionToDo;
	// Character doing the action
	public CharacterClass actionFrom;
	// Character taking the action, thgis could probably be a temp variable
	public CharacterClass actionTo;
	public List<CharacterClass> targetsToDo;
	// Coordinates selected
	int[] coords = new int[2];


	/********************************************************************************************/ 
	/**************************************** Upkeep ********************************************/ 
	/********************************************************************************************/

	// Update is called once per frame
	void Update () {
		temppos = new Vector3 (0, 0, 0);
		// Check if we are currenntly picking a cell to attack
		if (selectingTargetLocation) {
			if (Input.GetButtonDown ("Left")) {
				temppos -= new Vector3(map.gridSize,0,0);
			}else if (Input.GetButtonDown ("Right")) {
				temppos += new Vector3(map.gridSize,0,0);
			}	
			if (Input.GetButtonDown ("Down")) {
				temppos -= new Vector3(0,0,map.gridSize);
			}else if (Input.GetButtonDown ("Up")) {
				temppos += new Vector3(0,0,map.gridSize);
			}
			if(temppos != new Vector3(0,0,0)){
				map.deSelectRange (targetLocation, areaRange);
				map.getTileFromPos (targetLocation).GetComponent<MapGridUnit>().reColour ();
				targetLocation += temppos;
				targetLocation = map.ForceInsideBoundaries (targetLocation);
				print ("Target: " +targetLocation.ToString ());
				//map.getTileFromPos (targetLocation).GetComponent<MapGridUnit>().Select ();
				map.selectRange (targetLocation, areaRange);
			}
			//map.getTileFromPos (targetLocation).GetComponent<MapGridUnit>().Select ();
			if (Input.GetButtonUp ("Submit")) {
				// when you hit space, get the tile selected to do what we wanted
				wasUp = true;
				//selectingTargetLocation = false;
			}
			if (Input.GetButtonDown ("Submit") && wasUp) {
				// when you hit space, get the tile selected to do what we wanted
				coords = map.getTileCoordsFromPos(targetLocation);
				print ("You have selected tile " + coords[0].ToString() + "  " + coords[1].ToString());
				selectingTargetLocation = false;


				// Remove max range indicators
				stopSelectingTargetLocation ();

				// Do the thing that this selection was for
				doAction ();
			}
		} else {
			// Do nothing for now
		}
	}


	/********************************************************************************************/ 
	/**************************************** Initialization ************************************/ 
	/********************************************************************************************/

	// Use this for initialization
	void Start () {
		var sceneMan = gameManager.instance;
		// Number of Ally Participants
		int currentPlayerCharacters = sceneMan.currentPlayerCharacters;
		// Get List of Players
		playerCharacters = new List<CharacterClass>();
		playerCharacters = sceneMan.playerCharacters;
		if (playerCharacters[0] == null) {
			print ("Did not load any characters, ADD END BATTLE HERE");
		}

		List<CharacterClass> targetsToDo = new List<CharacterClass>();
		// Start the Fighting
		StartBattle (currentPlayerCharacters, playerCharacters);
	}

	public void StartBattle(int numPlayers, List<CharacterClass> players){
		//print ("Battle Starting");
		map = GameObject.Find ("Map").GetComponent<Map> ();
		areaRange = 1;
		experienceEarned = 0;
		playerSprites = new GameObject[numPlayers];
		// Set Player characters based on inputs.
		maxPlayerCharacters = numPlayers;
		//playerCharacters = new  List<CharacterClass>();
		playerCharacters = players;
		for (int i = 0; i < maxPlayerCharacters; i++) {
			playerCharacters [i].SetupStats ();
			playerCharacters [i].FullHeal ();
			// Create Player Visuals
			playerSprites[i] = Instantiate(CharacterPrefabs[playerCharacters[i].BattleSprite], new Vector3 (0, 5, 5*i), Quaternion.identity);
			playerCharacters [i].battleAvatar = playerSprites [i];

		}
		selectingTargetLocation = false;

		var gameManager = GameObject.Find ("GameManager").GetComponent<gameManager>();

		// Set Enemies, for now just 2
		maxEnemyCharacters = 2;
		enemySprites = new GameObject[maxEnemyCharacters];
		//Create Enemies Randomly and initialize their stats / HP
		for (int i = 0; i < maxEnemyCharacters; i++) {
			enemyCharacters.Add(new CharacterClass ());
			enemyCharacters [i].Initialize ("Enemy "+ i.ToString(),gameManager.enemyLevel,1,gameManager.enemyTeam);
			enemyCharacters [i].SetupStats ();
			enemyCharacters [i].FullHeal ();
			// Create Enemy Visuals


			enemySprites[i] = Instantiate(gameManager.enemyToFight, new Vector3 (5, 5, 5*i), Quaternion.identity);
			enemyCharacters [i].battleAvatar = enemySprites [i];
			GameObject levelText = enemySprites[i].transform.GetChild(0).gameObject;
			levelText.GetComponent<TextMesh>().text = gameManager.enemyLevel.ToString ();
			// Print Details of Enemy
			string printstats = enemyCharacters [i].printStats ();
			//print("Enemy Spawned: " + printstats);
		}
		// Set initial character positions
		setInitialPositions();
		// Start the Combat with Player Turn
		PlayerTurn ();
	}

	public void setInitialPositions(){
		// Set initial character positions
		Vector3 tempPos = new Vector3(0,0,0);
		for (int i = 0; i < maxPlayerCharacters; i++) {
			setToPosition (playerCharacters [i], 0, i);
			/*
			tempPos = map.getPosFromCoords (0, i);
			tempPos [1] = 2f;
			playerSprites [i].transform.position = tempPos;
			// Set where the character thinks he is on the grid for range calculations and stuff
			playerCharacters[i].battleLocation = new int[2]{0,i};
			*/
		}

		for (int i = 0; i < maxEnemyCharacters; i++) {
			setToPosition (enemyCharacters [i], 1, i + 2);
			/*
			tempPos = map.getPosFromCoords (2, i);
			tempPos [1] = 2f;
			enemySprites [i].transform.position = tempPos;
			enemyCharacters[i].battleLocation = new int[2]{2,i};
			*/
		}
	}

	public void setToPosition(CharacterClass charToMove, int x, int z){
		// Moves character to the center of tile at [x,z].  Also sets that tile as occupied.
		map.tiles[x,z].GetComponent<MapGridUnit>().isOccupied = true;
		charToMove.battleLocation = new int[2]{ x, z };
		charToMove.battleAvatar.transform.position = map.getAbovePosFromCoords (x, z);
	}


	public void selectTargetLocation(int x, int z, int range){
		// Set up the map to start looking for a map location to use whatever ability on
		selectingTargetLocation = true;
		wasUp = false;
		map = GameObject.Find ("Map").GetComponent<Map> ();
		// Default select where Player 0 is
		targetLocation = playerSprites [0].transform.position;
		map.setInRange (x, z, range);
		HideBattleMenu();
		selectingFromX = x;
		selectingFromZ = z;
		selectingRange = range;

	}
	public void selectTargetLocation(int range){
		// Assume we are selecting from where the curent actor is
		int x = actionFrom.battleLocation [0];
		int z = actionFrom.battleLocation [1];
		selectingTargetLocation = true;
		wasUp = false;
		map = GameObject.Find ("Map").GetComponent<Map> ();
		// Default select where Player 0 is
		targetLocation = playerSprites [0].transform.position;
		map.setInRange (x, z, range);
		HideBattleMenu();
		selectingFromX = x;
		selectingFromZ = z;
		selectingRange = range;

	}
	public void stopSelectingTargetLocation(){
		selectingTargetLocation = false;
		//map = GameObject.Find ("Map").GetComponent<Map> ();
		// Default select where Player 0 is
		map.deSelectRange (targetLocation, areaRange);
		map.getTileFromPos (targetLocation).GetComponent<MapGridUnit> ().reColour ();
		targetLocation = playerSprites [0].transform.position;
		map.setOutOfRange (selectingFromX, selectingFromZ, selectingRange);


	}


	/********************************************************************************************/ 
	/**************************************** Turn Management ***********************************/ 
	/********************************************************************************************/


	void EndPlayerTurn (){
		// Placeholder
		EnemyTurn (0);
	}

	void EndBattle(float EXP){
		// End Battle, Load up main map
		var sceneMan = gameManager.instance;
		sceneMan.EndBattle (EXP);
	}


	void EnemyTurn(int target){
		//print ("Start of Enemy Turn");
		// No need to show player options when he has none
		HideBattleMenu();
		areaRange = 1;
		for (int i = 0; i < maxEnemyCharacters; i++) {
			// Only living enemies get a turn
			if (!enemyCharacters [i].isDead) {
				// Update who is doing the next action
				actionFrom = enemyCharacters [i];
				enemyCharacters [i].startTurn ();
				// Enemies only attack player targeted
				string battleMessage = enemyCharacters [i].Attack (playerCharacters [target]);
				// Destroy visual gameobject if enemy dies
				if (playerCharacters [target].checkDead ()) {
					Destroy(enemySprites [target].gameObject);
					print (target.ToString () + " of " + playerCharacters.Count.ToString ());
					playerCharacters.RemoveAt (target);
					maxPlayerCharacters -= 1;
				}
				print (battleMessage);
			}
		}
		// Check Vistory
		if (CheckWin ()) {
			print ("You win");

			EndBattle (experienceEarned);
			// Check Defeat
		} else if (CheckLoss ()) {
			print ("You Lose");
			EndBattle (experienceEarned);
		}
		PlayerTurn ();
	}
	void PlayerTurn (){
		//print ("Start of Player Turn");
		nTurns++;
		areaRange = 1;
		//PrintAllBattleStats ();
		// Show the Battle Menu
		ShowBattleMenu();
		for (int i = 0; i < maxPlayerCharacters; i++) {
			actionFrom = playerCharacters [i];
			playerCharacters [i].startTurn();
		}
	}

	// Checks to see if you have won the game
	bool CheckWin(){
		bool win = true;
		for (int i = 0; i < maxEnemyCharacters; i++) {
			if (!enemyCharacters [i].isDead) {
				win = false;
			}
		}
		return win;
	}
	// Checks to see if you have lost the game
	bool CheckLoss(){
		bool loss = true;
		for (int i = 0; i < maxPlayerCharacters; i++) {
			if (!playerCharacters [i].isDead) {
				loss = false;
			}
		}
		return loss;
	}

	/********************************************************************************************/ 
	/**************************************** Character Actions *********************************/ 
	/********************************************************************************************/

	// This will likely move to one bit ugly filewith every ability
	public void doAction(){
		List<CharacterClass> targetsToDo = null;
		//print ("Not Move, but " + actionToDo);
		if (actionToDo == "Move") {
			int[] tempOldCoords = actionFrom.battleLocation;
			int tempIntDistance = map.getIntDistanceFromCoords (tempOldCoords, coords);
			// If destination is in range
			if (tempIntDistance <= actionFrom.MP) {

				setToPosition (actionFrom, coords [0], coords [1]);
				actionFrom.MP -= map.getIntDistanceFromCoords (tempOldCoords, coords);
				/*
				print ("Moving Time");
				actionFrom.battleLocation = coords;
				actionFrom.battleAvatar.transform.position = map.getAbovePosFromCoords (coords [0], coords [1]);
				actionFrom.MP -= map.getIntDistanceFromCoords (tempOldCoords, coords);
				print ("Used " + tempIntDistance.ToString () + " MP, " + actionFrom.MP.ToString () + " remaining");
				*/
			} else {
				print ("Insufficient MP, " + actionFrom.MP.ToString() + " of " + tempIntDistance.ToString());

			}
			actionToDo = null;
			// Turn is not over, return to battle menu
			ShowBattleMenu();
		}
		if (actionToDo == "Attack") {
			print ("Attacking Time");
			string battleMessage = actionFrom.Attack (actionTo);
			// Check if you killed the target
			if (actionTo.checkDead ()) {
				killCharacter (actionTo);
			}

			// Turn is  over, you attacked
			EnemyTurn (0);
		}
		if (actionToDo == "Fireball") {
			print("Here we would do fireball deeps");
			targetsToDo =  getEnemiesInRange (coords[0],coords[1],areaRange,actionFrom.team);
			for (int i = 0; i < targetsToDo.Count; i++) {
				// Here will be more generalized in the future, I will make a dealElementalDamage() function in characterClass that will be like Attack(target) only with magic stuffs.
				targetsToDo [i].HP -= actionFrom.Intelligence * 2;
				print ("Applying Fireball to " + targetsToDo [i].name);
				if (targetsToDo [i].checkDead ()) {
					killCharacter (targetsToDo [i]);
				}
			}
			// Turn is  over, you attacked
			EnemyTurn (0);
		}
		if (actionToDo == "Sniper Attack") {
			print("Here we would do deeps to someone");
			// Turn is  over, you attacked
			EnemyTurn (0);
		}
		if (actionToDo == "Heal Self") {
			print("Here we would do self healing");
			// Turn is  over, you attacked
			EnemyTurn (0);
		}
	}
	public void killCharacter(CharacterClass toKill){
		// Kills the given character,
		if (enemyCharacters.Contains (toKill)) {
			experienceEarned += toKill.baseExperienceGiven;
			Destroy (toKill.battleAvatar);
			// Remove enemy from list
			enemyCharacters.Remove (toKill);
			maxEnemyCharacters -= 1;
		} else if (playerCharacters.Contains (toKill)) {
			Destroy (toKill.battleAvatar);
			// Remove player from list
			playerCharacters.Remove (toKill);
			maxPlayerCharacters -= 1;
		} else {
			print ("Couldn't find character to kill");
		}
	}

	public void PlayerAttack(int player, int badguy){
		// Given integer value for Player attacking and enemy being attacked, perform attack calculation
		string battleMessage = playerCharacters [player].Attack (enemyCharacters [badguy]);
		// Check if you killed the enemy
		if (enemyCharacters [badguy].checkDead ()) {
			experienceEarned += enemyCharacters [badguy].baseExperienceGiven;
			// Remove enemy from list
			enemyCharacters.RemoveAt (badguy);
			Destroy(enemySprites [badguy].gameObject);
			maxEnemyCharacters -= 1;
		}
		//print (battleMessage);
		EnemyTurn (0);
	}
	public void PlayerItem(){
		// Place Holderf for now
		List<CharacterClass> charsInRange= getAlliesInRange(1,1,3,"Player");
		print (charsInRange.Count.ToString ());
	}
	public void PlayerSpecial(){
		// Placeholder for now
	}
	public void PlayerRun(){
	//print("Gonna load the Main Map back up... wish me luck");
		EndBattle (0);
	}
	public void PlayerEndTurn(){
		EnemyTurn (0);
	}

	List<CharacterClass> getCharactersInRange(int x, int z, int range){
		// Creates a list of every character (enemy and player) insize of the range specified.
		List<CharacterClass> charactersInRange = new List<CharacterClass> ();
		int[] battleLoc = new int[2];
		for (int i = 0; i < maxEnemyCharacters; i++) {
			battleLoc = enemyCharacters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range)){
				charactersInRange.Add(enemyCharacters[i]);
			}
		}
		for (int i = 0; i < maxPlayerCharacters; i++) {
			battleLoc = playerCharacters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range)){
				charactersInRange.Add(playerCharacters[i]);
			}
		}
		return charactersInRange;
	}

	List<CharacterClass> getEnemiesInRange(int x, int z, int range, string team){
		// Creates a list of every enemy character or TEAM insize of the range specified.
		List<CharacterClass> charactersInRange = new List<CharacterClass> ();
		int[] battleLoc = new int[2];
		for (int i = 0; i < maxEnemyCharacters; i++) {
			battleLoc = enemyCharacters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range) && enemyCharacters[i].team != team){
				charactersInRange.Add(enemyCharacters[i]);
			}
		}
		for (int i = 0; i < maxPlayerCharacters; i++) {
			battleLoc = playerCharacters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range) && playerCharacters[i].team != team){
				charactersInRange.Add(playerCharacters[i]);
			}
		}
		return charactersInRange;
	}

	List<CharacterClass> getAlliesInRange(int x, int z, int range, string team){
		// Creates a list of every allied character with TEAM insize of the range specified.
		List<CharacterClass> charactersInRange = new List<CharacterClass> ();
		int[] battleLoc = new int[2];
		for (int i = 0; i < maxEnemyCharacters; i++) {
			battleLoc = enemyCharacters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range) && enemyCharacters[i].team == team){
				charactersInRange.Add(enemyCharacters[i]);
			}
		}
		for (int i = 0; i < maxPlayerCharacters; i++) {
			battleLoc = playerCharacters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range) && playerCharacters[i].team == team){
				charactersInRange.Add(playerCharacters[i]);
			}
		}
		return charactersInRange;
	}

	/********************************************************************************************/ 
	/**************************************** Menus Management **********************************/ 
	/********************************************************************************************/

	public void HideBattleMenu(){
		// Make the Options for battle (Attack, Item...) Invisible
		var BattleMenu = GameObject.Find ("Battle Menu");
		BattleMenu.GetComponent<CanvasGroup>().alpha = 0f;
		BattleMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
		BattleMenu.GetComponent<CanvasGroup>().interactable = false;
	}
	public void ShowBattleMenu(){
		// Make the Options for battle (Attack, Item...) Visible
		var BattleMenu = GameObject.Find ("Battle Menu").GetComponent<CanvasGroup>();
		BattleMenu.alpha = 1f;
		//print ("Show Battle Menu");
		BattleMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
		BattleMenu.GetComponent<CanvasGroup>().interactable = true;
		// Select Attack as default.
		BattleMenu.GetComponent<BattleMenu> ().Attack.Select ();

	}
	public void HideSelectMenu(){
		// Make the Options for battle (Attack, Item...) Invisible
		var SelectMenu = GameObject.Find ("SelectTarget");
		SelectMenu.GetComponent<CanvasGroup>().alpha = 0f;
		SelectMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
		SelectMenu.GetComponent<CanvasGroup>().interactable = false;
		// Destroy old buttons
		SelectMenu.GetComponent<SelectTarget> ().DestroyOptions ();
	}
	public void ShowSelectMenu(int maxSelectCharacters, List<CharacterClass> selectCharacters){
		// Make the Options for battle (Attack, Item...) Visible
		var SelectMenu = GameObject.Find ("SelectTarget").GetComponent<CanvasGroup>();
		SelectMenu.alpha = 1f;
		//print ("Show Select Menu");
		SelectMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
		SelectMenu.GetComponent<CanvasGroup>().interactable = true;
		// Currently all I'm using this for.
		bool attacking = true;
		if (attacking) {
			SelectMenu.GetComponent<SelectTarget> ().CreateAttackOptions (maxSelectCharacters, selectCharacters);
		} else {
			// Do dead character selection (ie for Revive item / spell)
			// Do Team Select
			// Do Area Select.... (After motion?)
		}
		// Default select first option
		SelectMenu.GetComponent<SelectTarget> ().option[0].Select ();
	}

	public void ShowAbilitiesMenu(){
		// Make the Options for battle (Attack, Item...) Visible
		var SelectMenu = GameObject.Find ("SelectTarget").GetComponent<CanvasGroup>();
		SelectMenu.alpha = 1f;
		//print ("Show Select Menu");
		SelectMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
		SelectMenu.GetComponent<CanvasGroup>().interactable = true;
		// Currently all I'm using this for.

		SelectMenu.GetComponent<SelectTarget> ().CreateAbilityOptions (actionFrom);

		// Default select first option
		SelectMenu.GetComponent<SelectTarget> ().option[0].Select ();
	}

	/********************************************************************************************/ 
	/**************************************** Diagnostic Tools **********************************/ 
	/********************************************************************************************/



	void PrintAllBattleStats(){
		// Prints each chraacter's name, level, HP and AP
		string temp;
		for (int i = 0; i < maxPlayerCharacters; i++) {
			temp = playerCharacters [i].printBattleStats ();
			print (temp);
		}
		for (int i = 0; i < maxEnemyCharacters; i++) {
			temp = enemyCharacters [i].printBattleStats ();
			print (temp);
		}
	}




}

