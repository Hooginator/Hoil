using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass : MonoBehaviour
{
	//Character Atributes
	public int Strength;
	public int Constitution;
	public int Perception;
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
	public StatusEffect Inflicted;
	public double AttackCost;
	public int intAttackCost;

	// these are the different Status effects
	public bool Bleed;
	public bool Poison;
	public bool Stun;
	public bool Blind;
	public bool Drunk;
	public bool HeatStroke;
	public bool FrostBite;
	public bool Haste;
	public bool Slow;



	// these are the modifers that are changed by statuseffects
	public double AttckCostModifier;
	public double AccuracyModifier;



	//this function will generate all character stats that are used in actions and therefore they will be dubbed action stats
	void GenStats ()
	{

		double AttackCost = 3 - Dexterity / 4 + AttckCostModifier;
		int intAttackCost = (int)AttackCost;
		double Accuracy = (Perception * 5) + (Dexterity * 3) 


	}



	//this is the function for this character to do an attack
	void Attack (CharacterClass Target)
	{

		double HitChance = 100+Accuracy-Target.Dodge;
		double RandomHit;
	


		if (AP>=intAttackCost)
		{
			AP = AP - intAttackCost;

			//roll to see if the attack hits or misses
			RandomHit =  Random.Range(1, 2);
			if (RandomHit > HitChance) 
			{
				print ("miss");
			}
			else
			{
				Target.HP = Target.HP - (Damage - Target.Defence);
			}
		}
		else
		{
			print ("not enough AP to attack");
		}


	}


}
