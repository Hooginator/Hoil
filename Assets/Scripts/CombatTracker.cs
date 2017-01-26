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
		//Create Enemies Randomly and initialize their stats / HP
		for (int i = 0; i < maxEnemyCharacters; i++) {
			enemyCharacters.Add(new CharacterClass ());
			enemyCharacters [i].Initialize ("Enemy");
			enemyCharacters [i].SetupStats ();
			enemyCharacters [i].FullHeal ();
			// Print Details of Enemy
			string printstats = enemyCharacters [i].printStats ();
			print("Enemy Spawned: " + printstats);
		}
		// Start the Combat with Player Turn
		PlayerTurn ();
	}

	void EndPlayerTurn (){
		// Placeholder
		EnemyTurn ();
	}
	void EndBattle(){
		// End Battle, Load up main map
		var sceneMan = gameManager.instance;
		sceneMan.EndBattle ();
	}


	void EnemyTurn(){
		print ("Start of Enemy Turn");
		// No need to show player options when he has none
		HideBattleMenu();
		for (int i = 0; i < maxEnemyCharacters; i++) {
			enemyCharacters [i].startTurn();
			// Enemies only attack Player 1 for now.
			string battleMessage = enemyCharacters [i].Attack (playerCharacters [0]);
			print (battleMessage);
		}
		// Check Vistory
		if (CheckWin ()) {
			print ("You win");
			EndBattle ();
			// Check Defeat
		} else if (CheckLoss ()) {
			print ("You Lose");
			EndBattle ();
		}
		PlayerTurn ();
	}
	void PlayerTurn (){
		print ("Start of Player Turn");
		// Show the Battle Menu
		ShowBattleMenu();
		for (int i = 0; i < maxPlayerCharacters; i++) {
			playerCharacters [i].startTurn();
		}
	}
	public void PlayerAttack(int player, int badguy){
		string battleMessage = playerCharacters [player].Attack (enemyCharacters [badguy]);
		print (battleMessage);
		EnemyTurn ();
	}
	public void PlayerItem(){
		// Place Holderf for now
	}
	public void PlayerSpecial(){
		// Placeholder for now
	}
	public void PlayerRun(){
	print("Gonna load the Main Map back up... wish me luck");
		EndBattle ();
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
		// Number of Ally Participants
		int currentPlayerCharacters = sceneMan.currentPlayerCharacters;
		// Get List of Players
		playerCharacters = new List<CharacterClass>();
		playerCharacters = sceneMan.playerCharacters;
		if (playerCharacters[0] == null) {
			print ("Did not load any characters, ADD END BATTLE HERE");
		}
		// Start the Fighting
		StartBattle (currentPlayerCharacters, playerCharacters);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void HideBattleMenu(){
		// Make the Options for battle (Attack, Item...) Invisible
		var BattleMenu = GameObject.Find ("Battle Menu");
		BattleMenu.GetComponent<CanvasGroup>().alpha = 0f;
		BattleMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	void ShowBattleMenu(){
		// Make the Options for battle (Attack, Item...) Visible
		var BattleMenu = GameObject.Find ("Battle Menu").GetComponent<CanvasGroup>();
		BattleMenu.alpha = 1f;
		print ("Show Battle Menu");
		BattleMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;

	}

}
