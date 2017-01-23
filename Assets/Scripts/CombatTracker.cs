using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTracker : MonoBehaviour {
	// Good Guys
	public CharacterClass[] playerCharacters;
	int maxPlayerCharacters;
	// Bad Guys
	public CharacterClass[] enemyCharacters;
	int maxEnemyCharacters;

	GameObject BattleMenu;

	public void StartBattle(int numPlayers, CharacterClass[] players){
		// Set Player characters based on inputs.
		maxPlayerCharacters = numPlayers;
		playerCharacters = new CharacterClass[numPlayers];
		playerCharacters = players;
		print ("Battle Starting");
		// Set Enemies, for now just 1 random
		maxEnemyCharacters = 1;
		enemyCharacters = new CharacterClass[maxEnemyCharacters];
		for (int i = 0; i < maxEnemyCharacters; i++) {
			enemyCharacters [i] = new CharacterClass ();
			enemyCharacters [i].SetupStats ();
			enemyCharacters [i].FullHeal ();
		}


		PlayerTurn ();
	}




	void EnemyTurn(){
		for (int i = 0; i < maxEnemyCharacters; i++) {
			enemyCharacters [i].AP = enemyCharacters [i].APmax;
			// Enemies only attack Player 1 for now.
			enemyCharacters [i].Attack (playerCharacters [0]);
		}
		if (CheckWin()) {
			print ("You win");
		} else if (CheckLoss()) {
			print ("You Lose");
		} else {
			// Once Done go to player turn
			PlayerTurn ();
		}
	}
	void PlayerTurn (){

		// Show the Battle Menu
		ShowBattleMenu();
		// Hide the Battle Menu
		//HideBattleMenu();
		// Once done go to Enemy Turn
		EnemyTurn ();
	}
	// Checks to see if you have won the game
	bool CheckWin(){
		bool win = true;
		for (int i = 0; i < maxEnemyCharacters; i++) {
			if (enemyCharacters [i].HP > 0) {
				win = false;
			}
		}
		return win;
	}
	// Checks to see if you have lost the game
	bool CheckLoss(){
		bool loss = true;
		for (int i = 0; i < maxPlayerCharacters; i++) {
			if (playerCharacters [i].HP > 0) {
				loss = false;
			}
		}
		return loss;
	}

	// Use this for initialization
	void Start () {
		var sceneMan = gameManager.instance;
		int currentPlayerCharacters = sceneMan.currentPlayerCharacters;
		print (currentPlayerCharacters.ToString ());
		playerCharacters = new CharacterClass[currentPlayerCharacters];
		playerCharacters[0] = sceneMan.playerCharacters[0];
		//sceneMan.printPlayer0Stats ();
		if (playerCharacters[0] == null) {
			print ("Did not load any characters");
		}
		print (sceneMan.playerCharacters[0].Strength.ToString ());
		StartBattle (currentPlayerCharacters, playerCharacters);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void HideBattleMenu(){
		var BattleMenu = GameObject.Find ("Battle Menu");
		BattleMenu.GetComponent<CanvasGroup>().alpha = 0f;
		//BattleMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	void ShowBattleMenu(){
		var BattleMenu = GameObject.Find ("Battle Menu").GetComponent<CanvasGroup>();
		BattleMenu.alpha = 1f;
		print ("Show Battle Menu");
		//BattleMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;

	}

}
