using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/***********************************************************/
// This is where all of the important stuff that is needed in multiple scenes goes.
// GAME MANAGER is not destroyed on scene load, and a new one is not made.
// Contains scene menegement, world initialization and the effects from one scene to another (combatants to battle, EXP to world..)
/***********************************************************/

public class gameManager : MonoBehaviour {
	public bool inBattle = true;
	public static gameManager instance = null;
	// Characters that will stick around after scenes.
	//public CharacterClass[] playerCharacters;
	public List<CharacterClass> playerCharacters;// = new  List<CharacterClass>();
	public int maxPlayerCharacters = 4;
	public int currentPlayerCharacters = 0;
	public Transform playerMapPosition;
	public Transform teamMapPosition;

	public List<GameObject> teamsPrefabs = new List<GameObject>();
	public List<GameObject> teams;

	public GameObject playerWorldSprite;
	public GameObject worldPlayer;

	// prefab sprite to use.
	public GameObject enemyToFight;
	public int enemyLevel;
	public string enemyTeam;

	// Ground tile to be used to generate battle map
	public float[] groundTileResources;

	public List<CharacterClass> combatants;


	//public Vector3 startPosition = new Vector3(60,2,-10);

	/********************************************************************************************/ 
	/************************************ Initialization ****************************************/ 
	/********************************************************************************************/


	public void LoadScene(string sceneName){
		// Used to load different gamef files, IE loading from the battle scene to the world map and back.
		SceneManager.LoadScene (sceneName);
	}
	public void UnLoadScene(string sceneName){
		// Takes all the stuff from a scene out of memory
		this.UnLoadScene (sceneName);
	}
	void Awake(){
		// Makes sure that the GameManager this is attached to is always the same one, so we can use it to keep values through scenes.
		if (instance == null) {
			instance = this;

			Scene currentScene = SceneManager.GetActiveScene ();
			string sceneName = currentScene.name;
			if (sceneName == "Battle") {
				inBattle = true;
			} else {
				inBattle = false;
			}
			worldPlayer = GameObject.Instantiate (playerWorldSprite, new Vector3 (90, 2, 0), Quaternion.identity);
			worldPlayer.transform.SetParent (gameObject.transform);
			playerMapPosition = worldPlayer.transform;
			//playerMapPosition = new Vector3(0,3,0);
			//playerMapPosition = GameObject.Find("Player").GetComponent<Transform>().position;
			/****************************************** Start of the game here ***********************************************************/
			// This will be called once at the very start of the game and then never again, good place to set up one time events at the start.
			// Create Main character, probably will be more involved than this later :P
			playerCharacters = new  List<CharacterClass> ();
			teams = new  List<GameObject> ();

			// Add Main CharacterPlayer character
			CharacterClass temp = new CharacterClass ();
			// Initialize stats to level 40 so we can beat level 15 generated badguy easily
			temp.Initialize ("Main Character", 40, 0, "Player");
			playerCharacters.Add (temp);
			currentPlayerCharacters += 1;

			// Add second player character
			CharacterClass temp2 = new CharacterClass ();
			temp2.Initialize ("Secondary Character", 25, 0, "Player");
			playerCharacters.Add (temp);
			currentPlayerCharacters += 1;

				// Create the two teams and set their parent transforms to this gameobject (to not be destroyed)
				GameObject tempTeam1 = GameObject.Instantiate (Resources.Load ("Blue Base"), new Vector3 (0, 1, 0), Quaternion.identity) as GameObject;
				tempTeam1.GetComponent<Transform> ().parent = gameObject.transform;
				teams.Add (tempTeam1);
				GameObject tempTeam2 = GameObject.Instantiate (Resources.Load ("Red Base"), new Vector3 (90, 1, 90), Quaternion.identity) as GameObject;
				tempTeam2.GetComponent<Transform> ().parent = gameObject.transform;
				teams.Add (tempTeam2);

				//teams[0].GetComponent<Team>().Initialize();
				//teams[1].GetComponent<Team>().Initialize();
				// Initialize everything that would also be initialized post battle
				// Apply the movement controls for the world map to the player
				WorldMovementControls WMC = worldPlayer.AddComponent<WorldMovementControls> ();
				WMC.moveSpeed = 20;
				WMC.RotationSpeed = 1;

			if (sceneName == "Hoil") {
				InitializeWorld ();
			} else if (sceneName == "Battle") {
				print ("Started in Battle");
				StartBattle ();
			}


		} else if (instance != this){
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);


	}
	void InitializeWorld(){
		// Reinitialize the Player
		worldPlayer.SetActive(true);
		//Reinitialisze the teams bases
		for (int i = 0; i < teams.Count; i++) {
			teams [i].SetActive (true);
			//teams [i].GetComponent<Team> ().Initialize ();
		}
		// Spawn Enemies after world battle
		// Out for now as this references the main base which gets destroyed... might need to add that to the do not destroy list
		//teams [0].spawnEnemies ();
		//teams [1].spawnEnemies ();
	}

	/********************************************************************************************/ 
	/******************************* Battle Management ******************************************/ 
	/********************************************************************************************/
	public void StartBattle(){
		// Load Battle if it is the first scene loaded.  mostly for testing
		// Get the colour of the tile the enemy was on for the battle
		groundTileResources = new float[3]{0.5f,0.5f,0.5f};
		playerMapPosition = worldPlayer.GetComponent<Transform>();
		//print (playerMapPosition.position.ToString ());
		//GameObject.Destroy (worldPlayer);
		// Disable the World version of player
		worldPlayer.SetActive(false);
		// disable the bases for each team on the world map
		for (int i = 0; i < teams.Count; i++) {
			teams [i].SetActive (false);
		}

		combatants = new List<CharacterClass> ();
		CharacterClass tempCharacterClass;
		for (int i = 0; i < playerCharacters.Count; i++) {
			//tempCharacterClass = new CharacterClass ();
			//tempCharacterClass = playerCharacters [i];
			combatants.Add (playerCharacters [i]);
			//combatants [i] = playerCharacters [i];
		}
		// Add four lvl 15 enemies...
		for (int i = 0; i < 4; i++) {
			combatants.Add (new CharacterClass ());
			combatants [i+playerCharacters.Count].Initialize ("Enemy " + i.ToString (), 15, 1, "Red");
		}

		inBattle = true;
	}
	public void StartBattle(GameObject enemyGameObject){
		// Once collided with enemy, starta  fight. 
		// I will need enemy information coming through here
		enemyToFight = enemyGameObject.GetComponent<EnemyBehavior>().prefab;
		enemyLevel = enemyGameObject.GetComponent<EnemyBehavior> ().level;
		enemyTeam = enemyGameObject.GetComponent<EnemyBehavior> ().team;
		// Get the colour of the tile the enemy was on for the battle
		groundTileResources = GameObject.Find ("Map").GetComponent<Map> ().getTileFromPos (enemyGameObject.transform.position).GetComponent<MapGridUnit>().resources;
		playerMapPosition = worldPlayer.GetComponent<Transform>();
		//print (playerMapPosition.position.ToString ());
		//GameObject.Destroy (worldPlayer);
		// Disable the World version of player
		worldPlayer.SetActive(false);
		// disable the bases for each team on the world map
		for (int i = 0; i < teams.Count; i++) {
			teams [i].SetActive (false);
		}

		combatants = new List<CharacterClass> ();
		CharacterClass tempCharacterClass;
		for (int i = 0; i < playerCharacters.Count; i++) {
			tempCharacterClass = playerCharacters [i];
			combatants.Add (tempCharacterClass);
		}
		// for now 2 enemies of collided type
		for (int i = 0; i < 2; i++) {
			tempCharacterClass = new CharacterClass ();
			tempCharacterClass.Initialize ("Enemy " + i.ToString (), enemyLevel, 1, enemyTeam);
			combatants.Add (tempCharacterClass);
		}


		LoadScene ("Battle");
		inBattle = true;
	}
	public void ReduceTeamLevel(int levelAmount, string teamName){
		// Reduce Teams levels based on the losses of the fight
		if (teamName == "Blue") {
			teams [0].GetComponent<Team> ().level -= levelAmount;
			teams [0].GetComponent<Team> ().updateLevelIndicator ();
			//print ("Blue level now: "+teams [0].GetComponent<Team> ().level.ToString ());
		}
		if (teamName == "Red") {
			teams [1].GetComponent<Team> ().level -= levelAmount;
			teams [1].GetComponent<Team> ().updateLevelIndicator ();
			//print ("Red level now: "+teams [1].GetComponent<Team> ().level.ToString ());
		}
	}

	public void EndBattle(float EXP){
		// Load up the world map again. Maybe apply EXP and items here.
		for (int i = 0; i < playerCharacters.Count; i++) {
			print(playerCharacters [i].GainExperience (EXP));
		}

		// Reduce Teams levels based on the losses of the fight
		ReduceTeamLevel(enemyLevel, enemyTeam);
		LoadScene ("Hoil");

		InitializeWorld ();
		inBattle = false;
		//GameObject.Find ("Player").GetComponent<WorldMovementControls> ().Initialize ();
	}
	public bool checkInBattle(){
		return inBattle;
	}

	/********************************************************************************************/ 
	/********************************** Diagnostic Tools ****************************************/ 
	/********************************************************************************************/

	public void printPlayer0Stats(){
		// Small test print fundtion
		print ("Player 0 has Strength " + this.playerCharacters[0].Strength.ToString()); 
	}

}
