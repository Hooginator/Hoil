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

	// First constructor for Character Class, just sets everything to something random
	public void Initialize(string newName){
		//print ("Building Character with Random Stats");
		Strength = Random.Range(1,10);
		Constitution = Random.Range(1,10);
		Preception = Random.Range(1,10);
		Dexterity = Random.Range(1,10);
		Charisma = Random.Range(1,10);
		Intelligence = Random.Range(1,10);
		name = newName;
	}
	public string printStats(){
		string stats = name + "  STR: " + Strength.ToString () + "CON: " + Constitution.ToString () + "PRE: " + Preception.ToString () 
							+ "DEX: " + Dexterity.ToString () + "CHA: " + Charisma.ToString () + "INT: " + Intelligence.ToString ();
		return stats;
	}
	// Calculated second level stats
	public void SetupStats(){
		HPmax = Strength + Constitution;
		APmax = Preception*4;
		CarryWeight = Strength;
		Damage = Strength / 2;
		Accuracy = Dexterity;
		Defence = Constitution;
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
				Target.HP = Target.HP - (Damage - Target.Defence);
				battleMessage = "Hit";
			}
		}
		else
		{
			battleMessage = "not enough AP to attack, AP: " + AP.ToString ();;
		}
		return battleMessage;

	}


}
