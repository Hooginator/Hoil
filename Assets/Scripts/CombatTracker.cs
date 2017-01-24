using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTracker : MonoBehaviour {
	// Good Guys
	public List<CharacterClass> playerCharacters  = new  List<CharacterClass>();
	int maxPlayerCharacters;
	// Bad Guys
	public List<CharacterClass> enemyCharacters  = new  List<CharacterClass>();
	int maxEnemyCharacters;

	GameObject BattleMenu;
	// Number of turns that have gone by, so I can kill the infinite loops witha  failsafe
	private int nTurns = 0;
	public void StartBattle(int numPlayers, List<CharacterClass> players){
		print ("Battle Starting");
		// Set Player characters based on inputs.
		maxPlayerCharacters = numPlayers;
		//playerCharacters = new  List<CharacterClass>();
		playerCharacters = players;
		for (int i = 0; i < maxPlayerCharacters; i++) {
			playerCharacters [i].SetupStats ();
			playerCharacters [i].FullHeal ();
		}
		// Set Enemies, for now just 1 random
		maxEnemyCharacters = 1;
		//enemyCharacters = new List<CharacterClass>();
		for (int i = 0; i < maxEnemyCharacters; i++) {
			enemyCharacters.Add(new CharacterClass ());
			enemyCharacters [i].Initialize ("Enemy");
			enemyCharacters [i].SetupStats ();
			enemyCharacters [i].FullHeal ();
			string printstats = enemyCharacters [i].printStats ();
			print(printstats);
		}

		bool done = false;
		while (!done) {
			nTurns++;
			done = PlayerTurn ();
			if (!done) {
				done = EnemyTurn ();
			}
			if (nTurns > 20) {
				print ("20 Turns went by, aborting battle");
				break;
			}
		}
		print ("Done Fighting");
	}




	bool EnemyTurn(){
		print ("Start of Enemy Turn");
		for (int i = 0; i < maxEnemyCharacters; i++) {
			enemyCharacters [i].AP = enemyCharacters [i].APmax;
			// Enemies only attack Player 1 for now.
			string battleMessage = enemyCharacters [i].Attack (playerCharacters [0]);
			print (battleMessage);
		}
		if (CheckWin ()) {
			print ("You win");
			return true;
		} else if (CheckLoss ()) {
			print ("You Lose");
			return true;
		} else {
			return false;
		}
	}
	bool PlayerTurn (){
		print ("Start of Player Turn");
		// Show the Battle Menu
		ShowBattleMenu();
		// Just attack first enemy for now
		for (int i = 0; i < maxPlayerCharacters; i++) {
			playerCharacters [i].AP = playerCharacters [i].APmax;
			// Enemies only attack Player 1 for now.
			string battleMessage = playerCharacters [i].Attack (enemyCharacters [0]);
			print (battleMessage);
		}
			
		if (CheckWin()) {
			print ("You win");
			return true;
		} else if (CheckLoss()) {
			print ("You Lose");
			return true;
		} else {
			return false;
		}

		// Hide the Battle Menu
		//HideBattleMenu();
		// Once done go to Enemy Turn
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
		playerCharacters = new List<CharacterClass>();
		playerCharacters = sceneMan.playerCharacters;
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
