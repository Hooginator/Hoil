using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***********************************************************/
// COMBAT TRACKER is the bread and butter of the Battle scene.
// Initializes needed battle parameters from what GAME MANAGER provides
// Has location selection and some menu management (back button)
// Turnmanagement, map ranges and camera directions
// Menu management between BATTLE MENU and SELECT MENU
// the doAction() function that is the how all interactions with the battle are carried out:
// -> Currently movement, abilities and basic attacks go through, will add items, run...
/***********************************************************/


public class CombatTracker : MonoBehaviour {
	// Management Functions for the combat flow.

	// General Turns
	public int numCharacters;
	public List<CharacterClass> characters  = new  List<CharacterClass>();
	public GameObject[] sprites;

	// Prefabs of the visuals used for characters in battle
	public GameObject[] CharacterPrefabs;// = new GameObject[3];

	public float experienceEarned;

	public Vector3 targetLocation;
	bool selectingTargetLocation;

	GameObject BattleMenu;
	// Shows which menu we're in.  likely to be used for going back a manu
	public string windowStatus;
	public string previousWindowStatus;

	public Map map;
	public Vector3 temppos;
	// Number of turns that have gone by, so I can kill the infinite loops witha  failsafe
	private int nTurns = 0;

	// Temp boolean to stop from pressing enter, ie "Submit" once and blasting through all the menus and target selection. 
	public bool wasUp;
	public bool cancelWasUp;
	// Map coords for where a selection of a map tile originates from
	public int selectingFromX;
	public int selectingFromZ;
	public int selectingRange;
	public int areaRange;

	// Name of the action to be performed.  For when selecting a target, after we have chosen Attack, Item, Move etc..
	public Ability actionToDo;
	// Character doing the action
	public CharacterClass actionFrom;
	// Character taking the action, thgis could probably be a temp variable
	public CharacterClass actionTo;
	public List<CharacterClass> targetsToDo;
	// Coordinates selected
	int[] coords = new int[2];

	public BattleCameraControls battleCam;

	public Vector3 moveTarget;
	public Vector3 currentPos;
	public bool isMoving;
	public float moveSpeed;


	public List<CharacterClass> currentTurnCharacters;
	public string currentTeam;
	public List<string> teams;
	// boolean for whether we are locked into the selection for who is going (say after they have done an action)
	public bool actionFromLocked;

	public List<ColourPalette> colourPalettes = new List<ColourPalette>();

	/********************************************************************************************/ 
	/**************************************** Upkeep ********************************************/ 
	/********************************************************************************************/

	// Update is called once per frame
	void Update () {
		// Check if our character is moving
		if (isMoving) {
			currentPos = Vector3.MoveTowards (currentPos, moveTarget, moveSpeed);
			actionFrom.battleAvatar.gameObject.transform.position = currentPos;
			if (currentPos == moveTarget) {
				isMoving = false;
				print ("Done Moving");
				// Turn is not over, return to battle menu
				setToPosition (actionFrom, coords [0], coords [1]);
				continueTurn ();
			}
		}

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
				updateCameraTarget (targetLocation);
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
				//print ("You have selected tile " + coords[0].ToString() + "  " + coords[1].ToString());


				// Check if we're actually in range
				if(map.isInRange(coords[0],coords[1],selectingFromX,selectingFromZ,selectingRange)){
					selectingTargetLocation = false;
					// Remove max range indicators
					stopSelectingTargetLocation ();
					// Do the thing that this selection was for
					doAction ();
				}else{
					print("Not in range");
				}
			}
		} else {
			// Do nothing for now
		}
		// Going back menus
		if (Input.GetButtonDown ("Cancel")) {
			print ("Go Back!!");
			goBackMenu ();
			cancelWasUp = false;
		}else if (Input.GetButtonUp ("Cancel")) {
			cancelWasUp = true;
		}
	}
	public void goBackMenu (){
		print ("Windows: " + windowStatus + "  " + previousWindowStatus);
		// For now every time you go back it goes right back to the start.  I will need to record the difference between selecting a target for abilities / items / move etc...
		switch (windowStatus) {

		case "Abilities Menu":
		case "Item Menu":
		case "Select Character Menu":
			HideSelectMenu ();
			ShowBattleMenu ();
			break;
		case "Selecting Location":
			stopSelectingTargetLocation ();
			switch (previousWindowStatus) {
			case "Abilities Menu":
				ShowAbilitiesMenu ();
				break;
			case "Battle Manu":
			default:
				ShowBattleMenu ();
				break;
			}
			break;
		case "Battle Menu":
		case "None":
		default:
			print ("Can't go back");
			break;
		}
	}
	void updateCameraTarget(Vector3 newTarget){
		// Update where the battle camera will look to
		battleCam.updateTarget (newTarget);
	}

	void updateCameraTarget(int x, int z){
		// Updates where camera looks based on coords
		Vector3 newTarget = map.getPosFromCoords (x, z);
		battleCam.updateTarget (newTarget);
	}
	void updateCameraTarget(int[] pos){
		// Updates where the camera looks based on INT array coordinates
		Vector3 newTarget = map.getPosFromCoords (pos[0], pos[1]);
		battleCam.updateTarget (newTarget);
	}
	/********************************************************************************************/ 
	/**************************************** Initialization ************************************/ 
	/********************************************************************************************/

	// Use this for initialization
	void Start () {
		isMoving = false;
		cancelWasUp = true;
		moveSpeed = 0.4f;
		var gameMan = gameManager.instance;
		// Get List of Players
		characters = new List<CharacterClass>();
		for (int i = 0; i < gameMan.combatants.Count; i++) {
			print (gameMan.combatants.Count.ToString ());
			characters.Add(gameMan.combatants [i]);
		}
		if (characters[0] == null) {
			print ("Did not load any characters, ADD END BATTLE HERE");
		}

		List<CharacterClass> targetsToDo = new List<CharacterClass>();
		// Start the Fighting
		StartBattle ();
	}

	public void StartBattle(){
		//print ("Battle Starting");
		map = GameObject.Find ("Map").GetComponent<Map> ();
		battleCam = GameObject.Find ("Camera").GetComponent<BattleCameraControls> ();
		areaRange = 0;
		experienceEarned = 0;
		selectingTargetLocation = false;
		windowStatus = "None";
		previousWindowStatus = "None";
		var gameManager = GameObject.Find ("GameManager").GetComponent<gameManager>();
		numCharacters = characters.Count;

		sprites = new GameObject[numCharacters];

		for (int i = 0; i < numCharacters; i++) {
			characters [i].SetupStats ();
			characters [i].FullHeal ();
			// Create Player Visuals
			sprites[i] = Instantiate(CharacterPrefabs[characters[i].BattleSprite], new Vector3 (0, 5, 5*i), Quaternion.identity);
			characters [i].battleAvatar = sprites [i];
			if (characters [i].team == "Player") {
				print ("Player Detected");
			} else {
				// Update level indicator above enemy head
				//GameObject levelText = sprites[i].transform.GetChild(0).gameObject;
				//levelText.GetComponent<TextMesh>().text = gameManager.enemyLevel.ToString ();
			}

		}
		colourPalettes = gameManager.colourPalettes;	
		// Set initial character positions
		setInitialPositions();

		getTeamNames ();


		// set general turn stuff up
		// Start with character[0] turn
		currentTeam = characters[0].team;

		// Start the Combat with Player Turn
		startTurn ();

	}
	public void getTeamNames(){
		teams = new List<string>();
		string tempTeam;
		for (int i = 0; i < numCharacters; i++) {
			tempTeam = characters [i].team;
			if (teams.IndexOf (tempTeam) < 0) {
				teams.Add (tempTeam);
			}
		}
	}
	public void continueTurn(){
		// Used after movement, for player it will bring back the menu after the movement is done
		if (currentTeam == "Player") {
			ShowBattleMenu ();
		} else {
			print ("Continue enemy turn");
			continueComputerCharacterTurn ();
		}
	}

	public void setInitialPositions(){
		// Set initial character positions
		Vector3 tempPos = new Vector3(0,0,0);
		for (int i = 0; i < numCharacters; i++) {
			print ("Placing Character " + i.ToString () + "  " + characters [i].team);

			// This is super poorly done but I'm too tired to fix now.  TODO TOOOODOOOOOOOOOOOOOOOOOOOOOOO
			if (characters [i].team == "Player") {
				print ("Player Placement");
				setToPosition (characters [i], 0, i);
			} else if(characters [i].team == "Red"){
				print (characters [i].name.ToString()+" Placement");
				setToPosition (characters [i], 2, i );
			} else if(characters [i].team == "Blue"){
				print (characters [i].name.ToString()+" Placement");
				setToPosition (characters [i], 8, i );
			}
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
		targetLocation = map.getAbovePosFromCoords(x,z);
		map.setInRange (x, z, range);
		HideBattleMenu();
		selectingFromX = x;
		selectingFromZ = z;
		selectingRange = range;
		windowStatus = "Selecting Location";
	}
	public void selectTargetLocation(int range){
		// Assume we are selecting from where the curent actor is
		int x = actionFrom.battleLocation [0];
		int z = actionFrom.battleLocation [1];
		selectTargetLocation (x, z, range);
	}
	public void stopSelectingTargetLocation(){
		selectingTargetLocation = false;
		//map = GameObject.Find ("Map").GetComponent<Map> ();
		// Default select where Player 0 is
		map.deSelectRange (targetLocation, areaRange);
		map.getTileFromPos (targetLocation).GetComponent<MapGridUnit> ().reColour ();
		targetLocation = sprites [0].transform.position;
		map.setOutOfRange (selectingFromX, selectingFromZ, selectingRange);
		windowStatus = "None";


	}


	/********************************************************************************************/ 
	/**************************************** Turn Management ***********************************/ 
	/********************************************************************************************/


	void endCharacterTurn(){
		//currentTurnCharacters.Remove (actionFrom);

		map.deSelectAll();
		actionFrom.turnTaken = true;
		areaRange = 0;
		print (currentTurnCharacters.ToString ());
		print (actionFrom.name + " has ended his turn");
		actionFrom = null;
		actionFromLocked = false;
		for (int i = 0; i < currentTurnCharacters.Count; i++) {
			if (!currentTurnCharacters [i].turnTaken) {
				actionFrom = currentTurnCharacters [i];
				break;
			}
		}
		checkDead ();
		if (checkEndBattle ()) {
			EndBattle (experienceEarned);
		}
		if(actionFrom == null){
			endTurn ();
		} else {
			StartCoroutine(startCharacterTurnIn (1.0f));
		}
	}

	IEnumerator startCharacterTurnIn(float t){
		// Gives animations a second to go off before starting next turn
		yield return new WaitForSeconds (t);
		startCharacterTurn ();
	}

	void startCharacterTurn(){
		if (currentTeam == "Player") {
			ShowBattleMenu ();
		}else{
			startComputerCharacterTurn ();
		}
	}



	void endTurn(){
		print ("ACTUALLY ENDING A TURN");
		checkDead ();
		if (checkEndBattle ()) {
			EndBattle (experienceEarned);
		} else {
			// Setup for next turn
			int teamInt = teams.IndexOf (currentTeam);
			teamInt = (teamInt + 1);
			if (teamInt == teams.Count) {
				teamInt = 0;
				print ("TEAMS COUNT " + teams.Count.ToString ());
			}
			currentTeam = teams [teamInt];

			currentTurnCharacters = null;
			// Remove finished player from list
			// Check for list empty to either to go next team's turn or next character's
			// reset current character to null
			startTurn ();
		}
	}

	void startTurn(){
		print ("Start of Turn");
		currentTurnCharacters = getCurrentTurnCharacters ();
		actionFrom = currentTurnCharacters [0];
		for (int i = 0; i < currentTurnCharacters.Count; i++) {
			// Initialize for turns
			currentTurnCharacters [i].startTurn ();
		}
		if (currentTeam == "Player") {
			ShowBattleMenu ();
		} else {
			startComputerTurn ();
		}

	}

	List<CharacterClass> getCurrentTurnCharacters (){
		List<CharacterClass> tempList = new List<CharacterClass>();
		for(int i = 0;i<characters.Count;i++){
			print ("Adding character to " + currentTeam);
			if(characters[i].team == currentTeam){
				tempList.Add(characters[i]);
			}
		}
		return tempList;
		
	}
	void startComputerTurn(){
		// Do computer controlled turn
		print ("General Computer Turn");
		startComputerCharacterTurn ();

	}
	void startComputerCharacterTurn(){
		// Do One computer character's turn
		Ability temp = ScriptableObject.CreateInstance ("Ability") as Ability;
		temp.init ("Move", actionFrom);
		actionToDo = temp;
		// Move to a random location
		bool foundTarget = getRandomTarget(5,actionFrom.MP);
		if (map.isIntInBoundaries (coords [0], coords [1]) && !map.isOccupied (coords [0], coords [1])) {
			doAction ();
		} else {
			// Do post movement part of turn
			continueComputerCharacterTurn ();
		}
	}
	bool getRandomTarget( int maxAttempts, int range){
		// set coords to a Target for actionFrom ability coming in
		int tempRandomInt;
		bool foundTarget = false;
		for (int tempAttempt = 0; tempAttempt < 5;tempAttempt ++) {
			tempRandomInt = Random.Range(-range,range);
			coords [0] = actionFrom.battleLocation [0] + tempRandomInt;
			coords [1] = actionFrom.battleLocation [1] + Random.Range(-range + Mathf.Abs(tempRandomInt),range - Mathf.Abs(tempRandomInt));
			coords = map.MirrorInsideBoundaries (coords);
			if (map.isIntInBoundaries (coords [0], coords [1])) {
				foundTarget = true;
				break;
			}
			tempAttempt++;
		}
		return foundTarget;
	}
	void continueComputerCharacterTurn(){
		// The part of the turn after computer moves.
		Ability temp = ScriptableObject.CreateInstance ("Ability") as Ability;
		string abilityName = getAbilityToUse ();
		temp.init (abilityName, actionFrom);
		actionToDo = temp;
		int tempRandomInt;
		bool foundTarget = getRandomTarget(5,actionToDo.baseRange);
		if (foundTarget) {
			doAction ();
		} else {
			// note I only end character turn IF I didn't fgind a fireball target.  the fireball actions ends turn on its own after casting.
			endCharacterTurn ();
		}
	}
	string getAbilityToUse(){
		// generates a random ability string to use for computer turns
		int rand = Random.Range(1,4);
		switch (rand) {
		case 1:
			return "Fireball";
			break;
		case 2:
			return "Iceball";
			break;
		case 3:
			return "Acidball";
			break;

		}


		/// default
		return "Fireball";
	}
	/// END OF GENERALIZED TURN TEST

	void EndBattle(float EXP){
		// End Battle, Load up main map
		characters = null;
		var sceneMan = gameManager.instance;
		sceneMan.EndBattle (EXP);
	}

	bool checkEndBattle (){
		// Currently checks for two teams to be alive
		bool livingCharacter = false;
		string livingTeam = null;
		for (int i = 0; i < numCharacters; i++) {
			if (!characters [i].isDead) {
				if (livingCharacter) {
					if (livingTeam != characters [i].team) {
						return false;
					}
				}else{
					livingCharacter = true;
					livingTeam = characters [i].team;
				}


			}
		}
		return true;

		/*if (!livingPlayer) {
			experienceEarned = 0;
			print ("Player Died");
		}
		if (!livingEnemy) {
			print ("Enemies Died");
		}
		return !(livingEnemy && livingPlayer);*/
	}
	void checkDead(){
		// Checks all characters and kills them if dead
		for (int i = 0; i < numCharacters; i++) {
			if (characters [i] != null && characters [i].checkDead ()) {
				killCharacter (characters [i]);
			}
		}
	}

	/********************************************************************************************/ 
	/**************************************** Character Actions *********************************/ 
	/********************************************************************************************/

	// This will likely move to one big ugly filewith every ability
	public void doAction(){
		List<CharacterClass> targetsToDo = null;
		areaRange = actionToDo.AoERange;

		/************************************************** MOVING ******************************/

		if (actionToDo.name == "Move") {
			int[] tempOldCoords = actionFrom.battleLocation;
			int tempIntDistance = map.getIntDistanceFromCoords (tempOldCoords, coords);

			List<int[]> path = map.getPath (tempOldCoords, coords, actionFrom.MP);
			// If destination is in range
			if (tempIntDistance <= actionFrom.MP && path != null) {

				// Immediately set position to where you chose to move to
				//setToPosition (actionFrom, coords [0], coords [1]);
				currentPos = actionFrom.battleAvatar.transform.position;
				moveTarget = map.getAbovePosFromCoords(coords[0],coords[1]);
				isMoving = true;
				actionFrom.MP -= map.getIntDistanceFromCoords (tempOldCoords, coords);

			} else {
				print ("Insufficient MP, " + actionFrom.MP.ToString () + " of " + tempIntDistance.ToString ());
				continueTurn ();
			}
			actionToDo = null;

		/************************************************ ATTACKING *****************************/

		} else if (actionToDo.name == "Basic Attack") {
			print ("Attacking Time");
			string battleMessage = actionFrom.Attack (actionTo);
			// Check if you killed the target
			if (actionTo.checkDead ()) {
				killCharacter (actionTo);
			}

			actionToDo = null;
			endCharacterTurn ();

		/************************************************ SPECIAL *******************************/

		} else if (actionToDo != null){
			if (actionToDo.targetingType == "Single") {
				if (actionToDo.cast (actionTo)) {
					killCharacter (actionTo);
				}

			} else if (actionToDo.targetingType == "Area") {
				// If we've selected an area to cast on, cycle through the appropriate targets and apply the ability
				if (actionToDo.targets == "Enemy") {
					targetsToDo =  getEnemiesInRange (coords[0],coords[1],areaRange,actionFrom.team);
				} else if (actionToDo.targets == "Ally") {
					targetsToDo =  getAlliesInRange (coords[0],coords[1],areaRange,actionFrom.team);
				} else if (actionToDo.targets == "All") {
					targetsToDo =  getCharactersInRange (coords[0],coords[1],areaRange);
				}
				targetsToDo =  getEnemiesInRange (coords[0],coords[1],areaRange,actionFrom.team);
				for (int i = 0; i < targetsToDo.Count; i++) {
					if (actionToDo.cast (targetsToDo [i])) {
						// If it kills
						killCharacter(targetsToDo [i]);
					};
				}
				actionToDo.doAnimation (actionFrom.battleAvatar.transform.position,  map.getAbovePosFromCoords (coords [0], coords [1]),colourPalettes);
			}

			actionToDo = null;
			endCharacterTurn ();
		}
	}
	public void killCharacter(CharacterClass toKill){
		// Kills the given character,
		if (characters.Contains (toKill)) {
			print ("Killing Character on " + toKill.team);
			if (toKill.team != "Player") {
				experienceEarned += toKill.baseExperienceGiven;
			}
			StartCoroutine (DestroyCharacter (toKill.battleAvatar));
			toKill.battleAvatar.GetComponent<BasicEnemyAnimations> ().animationType = "dying";
			//Destroy (toKill.battleAvatar);
			// Remove enemy from list
			characters.Remove (toKill);
			numCharacters -= 1;
		} else {
			print ("Couldn't find character to kill");
		}
	}

	IEnumerator DestroyCharacter(GameObject avatar){
		// Coroutine to let the enemy model exist for a second after it is killed.
		yield return new WaitForSeconds(2.2f);
		Destroy (avatar);
	}

	public void PlayerAttack(int player, int badguy){
		// Given integer value for Player attacking and enemy being attacked, perform attack calculation
		string battleMessage = characters [player].Attack (characters [badguy]);
		// Check if you killed the enemy
		//print (battleMessage);
		endCharacterTurn ();
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
		endCharacterTurn ();
	}

	List<CharacterClass> getCharactersInRange(int x, int z, int range){
		// Creates a list of every character (enemy and player) insize of the range specified.
		List<CharacterClass> charactersInRange = new List<CharacterClass> ();
		int[] battleLoc = new int[2];
		for (int i = 0; i < numCharacters; i++) {
			battleLoc = characters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range)){
				charactersInRange.Add(characters[i]);
			}
		}
		return charactersInRange;
	}

	public List<CharacterClass> getEnemiesInRange(int x, int z, int range, string team){
		// Creates a list of every enemy character or TEAM insize of the range specified.
		List<CharacterClass> charactersInRange = new List<CharacterClass> ();
		int[] battleLoc = new int[2];
		for (int i = 0; i < numCharacters; i++) {
			battleLoc = characters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range) && characters[i].team != team){
				charactersInRange.Add(characters[i]);
			}
		}
		return charactersInRange;
	}

	public List<CharacterClass> getEnemiesInRange(int range, string team){
		// Assume range is measure from acting character
		return getEnemiesInRange (actionFrom.battleLocation[0],actionFrom.battleLocation[1],range, team);

	}

	public List<CharacterClass> getAlliesInRange(int x, int z, int range, string team){
		// Creates a list of every allied character with TEAM insize of the range specified.
		List<CharacterClass> charactersInRange = new List<CharacterClass> ();
		int[] battleLoc = new int[2];
		for (int i = 0; i < numCharacters; i++) {
			battleLoc = characters [i].battleLocation;
			if(map.isInRange(battleLoc[0],battleLoc[1],x,z,range) && characters[i].team == team){
				charactersInRange.Add(characters[i]);
			}
		}
		return charactersInRange;
	}

	/********************************************************************************************/ 
	/**************************************** Menus Management **********************************/ 
	/********************************************************************************************/

	// NOTE: Hide menus first! They reset which state you're in to "None"

	public void HideBattleMenu(){
		// Make the Options for battle (Attack, Item...) Invisible
		var BattleMenu = GameObject.Find ("Battle Menu");
		BattleMenu.GetComponent<CanvasGroup>().alpha = 0f;
		BattleMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
		BattleMenu.GetComponent<CanvasGroup>().interactable = false;
		windowStatus = "None";
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
		windowStatus = "Battle Menu";

	}
	public void HideSelectMenu(){
		// Make the Options for battle (Attack, Item...) Invisible
		var SelectMenu = GameObject.Find ("SelectTarget");
		SelectMenu.GetComponent<CanvasGroup>().alpha = 0f;
		SelectMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
		SelectMenu.GetComponent<CanvasGroup>().interactable = false;
		// Destroy old buttons
		SelectMenu.GetComponent<SelectTarget> ().DestroyOptions ();

		previousWindowStatus = windowStatus;
		windowStatus = "None";

	}
	public void ShowSelectMenu(List<CharacterClass> selectCharacters){
		if(selectCharacters.Count > 0){
			// Make the Options for battle (Attack, Item...) Visible
			var SelectMenu = GameObject.Find ("SelectTarget").GetComponent<CanvasGroup>();
			SelectMenu.alpha = 1f;
			//print ("Show Select Menu");
			SelectMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
			SelectMenu.GetComponent<CanvasGroup>().interactable = true;
			// Currently all I'm using this for.
			bool attacking = true;
			int maxSelectCharacters = selectCharacters.Count;
			if (attacking) {
				SelectMenu.GetComponent<SelectTarget> ().CreateAttackOptions (maxSelectCharacters, selectCharacters);
			} else {
				// Do dead character selection (ie for Revive item / spell)
				// Do Team Select
				// Do Area Select.... (After motion?)
			}
			// Default select first option
				SelectMenu.GetComponent<SelectTarget> ().option [0].Select ();
				windowStatus = "Select Character Menu";
		} else {
			continueTurn ();
		}

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

		previousWindowStatus = windowStatus;
		windowStatus = "Abilities Menu";
	}

	/********************************************************************************************/ 
	/**************************************** Diagnostic Tools **********************************/ 
	/********************************************************************************************/



	void PrintAllBattleStats(){
		// Prints each chraacter's name, level, HP and AP
		string temp;
		for (int i = 0; i < numCharacters; i++) {
			temp = characters [i].printBattleStats ();
			print (temp);
		}
		for (int i = 0; i < numCharacters; i++) {
			temp = characters [i].printBattleStats ();
			print (temp);
		}
	}




}

