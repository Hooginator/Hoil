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

	// Prefabs of the visuals used for characters in battle
	public GameObject[] CharacterPrefabs;// = new GameObject[3];

	public GameObject[] playerSprites;
	public GameObject[] enemySprites;


	GameObject BattleMenu;
	// Number of turns that have gone by, so I can kill the infinite loops witha  failsafe
	private int nTurns = 0;
	public void StartBattle(int numPlayers, List<CharacterClass> players){
		print ("Battle Starting");
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

		}
		// Set Enemies, for now just 1 random
		maxEnemyCharacters = 2;
		enemySprites = new GameObject[maxEnemyCharacters];
		//Create Enemies Randomly and initialize their stats / HP
		for (int i = 0; i < maxEnemyCharacters; i++) {
			enemyCharacters.Add(new CharacterClass ());
			enemyCharacters [i].Initialize ("Enemy",1,1);
			enemyCharacters [i].SetupStats ();
			enemyCharacters [i].FullHeal ();
			// Create Enemy Visuals
			enemySprites[i] = Instantiate(CharacterPrefabs[enemyCharacters[i].BattleSprite], new Vector3 (5, 5, 5*i), Quaternion.identity);
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
		PrintAllBattleStats ();
		// Show the Battle Menu
		ShowBattleMenu();
		for (int i = 0; i < maxPlayerCharacters; i++) {
			playerCharacters [i].startTurn();

		}
	}
	void PrintAllBattleStats(){
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
		print ("Show Battle Menu");
		BattleMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
		BattleMenu.GetComponent<CanvasGroup>().interactable = true;
		BattleMenu.GetComponent<BattleMenu> ().Attack.Select ();

	}
	public void HideSelectMenu(){
		// Make the Options for battle (Attack, Item...) Invisible
		var SelectMenu = GameObject.Find ("SelectTarget");
		SelectMenu.GetComponent<CanvasGroup>().alpha = 0f;
		SelectMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
		SelectMenu.GetComponent<CanvasGroup>().interactable = false;
		SelectMenu.GetComponent<SelectTarget> ().DestroyOptions ();
	}
	public void ShowSelectMenu(){
		// Make the Options for battle (Attack, Item...) Visible
		var SelectMenu = GameObject.Find ("SelectTarget").GetComponent<CanvasGroup>();
		SelectMenu.alpha = 1f;
		print ("Show Select Menu");
		SelectMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
		SelectMenu.GetComponent<CanvasGroup>().interactable = true;
		SelectMenu.GetComponent<SelectTarget> ().CreateOptions (3);
		SelectMenu.GetComponent<SelectTarget> ().option[0].Select ();
	}
	public void CreateSelectOptions()
	{
		var SelectMenu = GameObject.Find ("SelectTarget").GetComponent<SelectTarget>();
		// Just gives number of enemies for now, eventually it will be alist of enemies
		SelectMenu.CreateOptions (maxEnemyCharacters);
	}
}
