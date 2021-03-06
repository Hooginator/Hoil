using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***********************************************************/
// Stats and routines for individual characters (ally or enemy)
// Includes initialization, leveling, battle management and basic attacks
/***********************************************************/

public class CharacterClass : ScriptableObject // Removed inheritance from MonoBehaviour.  Now I can create these easier dynamically but you lost print.  I'll find a replacement.
{
	public string name;
	//Character Atributes
	public int Strength;
	public int Constitution;
	public int Preception;
	public int Dexterity;
	public int Charisma;
	public int Intelligence;


	//Character Stats
	public double HPmax;
	public int APmax;
	public int MPmax;
	public double HP;
	public int AP;
	public int MP;
	public double CarryWeight;
	public double Damage;
	public double Accuracy;
	public double Defence;
	public double Dodge;
	public double Range;
	double AttackCost;
	int intAttackCost;
	double HitChance;
	double RandomHit;
	public StatusEffect Inflicted;
	public List<Ability> abilities;

	public string team;

	// Level and EXperience
	public int level;
	public float experience;
	public float baseExperienceGiven;
	public float experienceToLevel;
	public bool turnTaken;

	// Are you dead?
	public bool isDead;
	// Prefab number.  There will be a list of sprites for combat and this number corresponds to which one to load for this character.
	public int BattleSprite;

	public int[] battleLocation;
	public GameObject battleAvatar;

	// First constructor for Character Class, just sets everything to something random
	public void Initialize(string newName, int startlevel, int BattleSpriteNumber, string teamName){
		//print ("Building Character with Random Stats");
		Strength = Random.Range(1+startlevel/2,10+startlevel);
		Constitution = Random.Range(1+startlevel/2,10+startlevel);
		Preception = Random.Range(1+startlevel/2,10+startlevel);
		Dexterity = Random.Range(1+startlevel/2,10+startlevel);
		Charisma = Random.Range(1+startlevel/2,10+startlevel);
		Intelligence = Random.Range(1+startlevel/2,10+startlevel);
		name = newName;
		level = startlevel;
		BattleSprite = BattleSpriteNumber;
		isDead = false;
		// Experience given if you kill this guy
		baseExperienceGiven = 100f*startlevel;
		experienceToLevel = 100f * startlevel;
		battleLocation = new int[2];
		team = teamName;


		abilities = new List<Ability> ();
		List<string> abilityNames = new List<string>(new string[] {"Fireball","Iceball","Sniper Attack","Healing"});
		for (int i = 0; i < abilityNames.Count; i++) {
			Ability temp = ScriptableObject.CreateInstance ("Ability") as Ability;
			temp.init (abilityNames [i], this);
			abilities.Add (temp);
		}
		/*abilities = new List<Ability> ();
		abilities.Add (ScriptableObject.CreateInstance("Ability")  ("Fireball",this));
		abilities.Add (new Ability("Sniper Attack",this));
		abilities.Add (new Ability("Healing",this));*/
	}


	/********************************************************************************************/ 
	/**************************************** Battle Management *********************************/ 
	/********************************************************************************************/

	public string GainExperience(float EXP){ 
		experience += EXP;
		string message = "Experience: " + experience.ToString() +" of " +experienceToLevel.ToString() + " needed";
		while (experience > experienceToLevel) {
			experience -= experienceToLevel;
			experienceToLevel += 100f;
			message = message + LevelUp ();
		}
		return message;
	}
	public string LevelUp(){
		level++;
		string message = name + " has increased to level " + level;
		// Currently, give each stat a 75% change to increase
		if (75 > Random.Range (1, 100)) {
			Strength++;
			message += " STR+";
		} else if (75 > Random.Range (1, 100)) {
			Constitution++;
			message += " CON+";
		} else if (75 > Random.Range (1, 100)) {
			Preception++;
			message += " PRE+";
		} else if (75 > Random.Range (1, 100)) {
			Dexterity++;
			message += " DEX+";
		} else if (75 > Random.Range (1, 100)) {
			Charisma++;
			message += " CHA+";
		} else if (75 > Random.Range (1, 100)) {
			Intelligence++;
			message += " INT+";
		}
		return message;
	}

	// Calculated second level stats
	public void SetupStats(){
		// Calculate battlestats from base stats for starting a battle
		HPmax = 4*Strength + 6*Constitution;
		APmax = Preception*4;
		MPmax = 5;
		CarryWeight = Strength;
		Damage = Strength;
		Accuracy = Dexterity;
		Defence = Constitution/4;
		Dodge = Dexterity;
		Range = Preception;
		AttackCost = Preception;
		intAttackCost = (int)AttackCost;
		HitChance = 100 + Accuracy;
	}
	public void FullHeal()
	{// Max out HP, for initialization
		HP = HPmax;
	}
	public bool takeDamage(float damageIn, string type, float time){
		//Reduce character's health accordingly.
		HP -= damageIn;
		// Create the damge taking animation default of 0.5 second lag on aniumation start
		gameManager.instance.StartCoroutine( displayDamageIn(damageIn, type, time));
		// returns true if if kills
		if (checkDead ()) {
			killCharacterIn (time);
			return true;
		}
		return false;
	}

	IEnumerator displayDamageIn(float damageIn, string type, float t){
		// Gives animations a second to go off before starting next turn
		yield return new WaitForSeconds (t);
		displayDamage(damageIn, type);
	}

	IEnumerator killCharacterIn(float t){
		// Gives animations a second to go off before starting next turn
		yield return new WaitForSeconds (t);
		killCharacter ();
	}
	void displayDamage(float damageIn, string type){
		// Create the damge taking animation
		if (battleAvatar != null) {
			GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("FloatingText"));
			temp.GetComponent<FloatingText> ().setPosition (battleAvatar.transform);
			temp.GetComponent<FloatingText> ().setText (damageIn, type);
		}
	}

	public bool checkDead(){
		// Check if character is dead
		if (HP < 0) {
			return true;
		}
		return false;
	}

	public void killCharacter(){
		// Kill this instance of character.  
		isDead = true;
	}
	public void startTurn(){
		// Update stats for start of turn
		// will do poison, debuffs ... here
		AP = APmax;
		MP = MPmax;
		Debug.Log ("STARTINGTURN< MP = MAX");
		if (team != "Player") {
			MP = 3;
		}
		turnTaken = false;
	}
	public void useMP(int toUse){
		Debug.Log ("Reducing character MP by " + toUse.ToString());
		MP -= toUse;
	}
	public int getMP(){
		return MP;
	}
	// This needs to be fixed....
	public string Attack (CharacterClass Target)
	{
		string battleMessage;
		// Update Stats
		SetupStats();
		Target.SetupStats ();
		// Update stats based on Target
		HitChance -= Target.Dodge;

		if (AP>=intAttackCost)
		{
			AP = AP - intAttackCost;

			//roll to see if the attack hits or misses
			RandomHit =  Random.Range(1, 2);
			if (RandomHit > HitChance) 
			{
				battleMessage = "miss";
			}
			else
			{
				double tempDamage = (Damage - Target.Defence);
				if (tempDamage < 0) {
					tempDamage = 0;
				}
				Target.takeDamage( (float)tempDamage,"normal",0);
				battleMessage = name + " hit " + Target.name + " for " + tempDamage.ToString() + " HP, " + Target.HP .ToString() + " remaining" ;
			}
		}
		else
		{
			battleMessage = "not enough AP to attack, AP: " + AP.ToString ();;
		}
		return battleMessage;

	}


	/********************************************************************************************/ 
	/**************************************** Diagnostic Tools **********************************/ 
	/********************************************************************************************/

	public string printStats(){
		// Return string of player anme and base stats
		string stats = name + "  STR: " + Strength.ToString () + " CON: " + Constitution.ToString () + " PRE: " + Preception.ToString () 
			+ " DEX: " + Dexterity.ToString () + " CHA: " + Charisma.ToString () + " INT: " + Intelligence.ToString ();
		return stats;
	}
	public string printBattleStats(){
		// Returns a string of character's name and some battle stats
		string stats = name + " level " + level.ToString() + ": " + HP.ToString () + "/" + HPmax.ToString () + " HP " + AP.ToString () + "/" + APmax.ToString () + " AP";
		return stats;
	}
}
