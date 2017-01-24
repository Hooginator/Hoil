using System.Collections;
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
	public int currentPlayerCharacters;


	public void LoadScene(string sceneName){
		// Used to load different gamef files, IE loading from the battle scene to the world map and back.
		SceneManager.LoadScene (sceneName);
	}
	public void UnLoadScene(string sceneName){
		// Takes all the stuff from a scene out of memory
		this.UnLoadScene (sceneName);
	}
	void Start(){
		// Makes sure that the GameManager this is attached to is always the same one, so we can use it to keep values through scenes.
		if (instance == null) {
			instance = this;
			/****************************************** Start of the game here ***********************************************************/
			// This will be called once at the very start of the game and then never again, good place to set up one time events at the start.
			// Create Main character, probably will be more involved than this later :P
			playerCharacters = new  List<CharacterClass>();
			// Make temp something that should definitely not be Null...
			CharacterClass temp = new CharacterClass();
			temp.Initialize ();
			// Add temp to the list
			playerCharacters.Add(temp);
			playerCharacters.Add(new CharacterClass());// = new CharacterClass[maxPlayerCharacters];
			//playerCharacters[0] = new CharacterClass();
			//playerCharacters [0].Initialize ();
			// Successfully print strength from the list unit that was created from temp
			print(playerCharacters[0].Strength.ToString());
			print (temp.Strength.ToString ());
			currentPlayerCharacters += 1;
			if (temp != null) {
				print ("Player 0 is NOT NULL from the start");
			} else {
				print ("Player 0 is NULL at start");
				// WHY IS THIS NULL!! I LITERALLY JUST MADE IT NOT THAT
				// FUUUUUUUUUUUUUUUUUUUU
				// Found it! Apparently any classes that inherit from MonoDevelop can not be created with 'new' so that messed with everything.
			}
			print ("Built new character with Strength " + playerCharacters[0].Strength.ToString());
			printPlayer0Stats ();



		} else if (instance != this){
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);


	}
	public void printPlayer0Stats(){
		
		print ("Player 0 has Strength " + this.playerCharacters[0].Strength.ToString()); 
	}
	public void StartBattle(){
		printPlayer0Stats ();
		LoadScene ("Battle");
		inBattle = true;
		printPlayer0Stats ();
		//GameObject.Find("Battle Menu").GetComponent<CombatTracker>().StartBattle()
		// Calls StartBattle from the combat tracker
		//CombatTracker battlestatus = new CombatTracker();
		//battlestatus.StartBattle(currentPlayerCharacters,playerCharacters);
		//GameObject.Find("Batle Menu").GetComponent<CombatTracker>().StartBattle(currentPlayerCharacters,playerCharacters);
	}
	public void EndBattle(){
		LoadScene ("Hoil");
		inBattle = false;
	}
}
