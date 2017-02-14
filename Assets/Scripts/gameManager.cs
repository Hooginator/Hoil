﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour {
	public bool inBattle = false;
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

	//public Vector3 startPosition = new Vector3(60,2,-10);

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
			playerMapPosition = GetComponent<Transform> ();
			playerMapPosition.position = new Vector3(90,2,0);
			worldPlayer = GameObject.Instantiate (playerWorldSprite, playerMapPosition);
			//playerMapPosition = new Vector3(0,3,0);
			//playerMapPosition = GameObject.Find("Player").GetComponent<Transform>().position;
			/****************************************** Start of the game here ***********************************************************/
			// This will be called once at the very start of the game and then never again, good place to set up one time events at the start.
			// Create Main character, probably will be more involved than this later :P
			playerCharacters = new  List<CharacterClass>();
			teams = new  List<GameObject>();
			// Make temp something that should definitely not be Null...
			CharacterClass temp = new CharacterClass();
			// Initialize stats to level 5 so we can beat level 1 generated badguy easily
			temp.Initialize ("GoodGuy",10,0);
			string printstats = temp.printStats ();
			print (printstats);
			// Add temp to the list
			playerCharacters.Add(temp);
			//playerCharacters.Add(new CharacterClass());
			currentPlayerCharacters += 1;

			// Create the two teams and set their parent transforms to this gameobject (to not be destroyed)
			GameObject tempTeam1 = GameObject.Instantiate(Resources.Load("Blue Base"),new Vector3(0,1,0),Quaternion.identity) as GameObject;
			tempTeam1.GetComponent<Transform> ().parent = gameObject.transform;
			teams.Add(tempTeam1);
			GameObject tempTeam2 = GameObject.Instantiate(Resources.Load("Red Base"),new Vector3(90,1,90),Quaternion.identity) as GameObject;
			tempTeam2.GetComponent<Transform> ().parent = gameObject.transform;
			teams.Add(tempTeam2);

			teams[0].GetComponent<Team>().Initialize(10);
			teams[1].GetComponent<Team>().Initialize(10);
			// Initialize everything that would also be initialized post battle
			InitializeWorld ();
			// Apply the movement controls for the world map to the player
			WorldMovementControls WMC = worldPlayer.AddComponent<WorldMovementControls> ();
			WMC.moveSpeed = 20;
			WMC.RotationSpeed = 1;

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
			teams [i].GetComponent<Team> ().Initialize (5);
		}
		// Spawn Enemies after world battle
		// Out for now as this references the main base which gets destroyed... might need to add that to the do not destroy list
		//teams [0].spawnEnemies ();
		//teams [1].spawnEnemies ();
	}
	public void printPlayer0Stats(){
		// Small test print fundtion
		print ("Player 0 has Strength " + this.playerCharacters[0].Strength.ToString()); 
	}
	public void StartBattle(){
		// Once collided with enemy, starta  fight. 
		// I will need enemy information coming through here
		playerMapPosition = worldPlayer.GetComponent<Transform>();
		print (playerMapPosition.position.ToString ());
		//GameObject.Destroy (worldPlayer);
		// Disable the World version of player
		worldPlayer.SetActive(false);
		// disable the bases for each team on the world map
		for (int i = 0; i < teams.Count; i++) {
			teams [i].SetActive (false);
		}

		LoadScene ("Battle");
		inBattle = true;
	}
	public void EndBattle(float EXP){
		// Load up the world map again. Maybe apply EXP and items here.
		for (int i = 0; i < playerCharacters.Count; i++) {
			print(playerCharacters [i].GainExperience (EXP));
		}
		LoadScene ("Hoil");

		InitializeWorld ();
		inBattle = false;
		//GameObject.Find ("Player").GetComponent<WorldMovementControls> ().Initialize ();
	}
}
