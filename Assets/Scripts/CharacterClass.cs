using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass // Removed inheritance from MonoBehaviour.  Now I can create these easier dynamically but you lost print.  I'll find a replacement.
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
	public double HP;
	public int AP;
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

	// Level and EXperience
	public int level;
	public float experience;

	// Prefab number.  There will be a list of sprites for combat and this number corresponds to which one to load for this character.
	public int BattleSprite;

	// First constructor for Character Class, just sets everything to something random
	public void Initialize(string newName, int startlevel, int BattleSpriteNumber){
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
	public string printStats(){
		string stats = name + "  STR: " + Strength.ToString () + " CON: " + Constitution.ToString () + " PRE: " + Preception.ToString () 
							+ " DEX: " + Dexterity.ToString () + " CHA: " + Charisma.ToString () + " INT: " + Intelligence.ToString ();
		return stats;
	}
	public string printBattleStats(){
		string stats = name + " level " + level.ToString() + ": " + HP.ToString () + "/" + HPmax.ToString () + " HP " + AP.ToString () + "/" + APmax.ToString () + " AP";
		return stats;
	}
	// Calculated second level stats
	public void SetupStats(){
		HPmax = 4*Strength + 6*Constitution;
		APmax = Preception*4;
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
	{
		HP = HPmax;
	}
	public void startTurn(){
		// Update stats for start of turn
		AP = APmax;
	}

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
				Target.HP = Target.HP - tempDamage;
				battleMessage = name + " hit " + Target.name + " for " + tempDamage.ToString() + " HP, " + Target.HP .ToString() + " remaining" ;
			}
		}
		else
		{
			battleMessage = "not enough AP to attack, AP: " + AP.ToString ();;
		}
		return battleMessage;

	}


}
