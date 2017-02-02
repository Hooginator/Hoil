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
	public double XPmax;
	public double XPcurrent;
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
	public double CritChance;

	//these are hidden stats that will not be directly used by the player
	public double XPReward;
	// double XPReward = Strenght + Constitution + Perception + Dexterity + Charisma + Intelligence;


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



	// these are the modifers that are changed by statuseffects, items, and mapeffects;
	public double HPmaxModifier;
	public int APmaxModifier;
	public double CarryWeightModifier;
	public double DamageModifier;
	public double AccuracyModifier;
	public double DefenceModifier;
	public double DodgeModifier;
	public double RangeModifier;
	public double AttckCostModifier;





	//this function will generate all character stats that are used in actions and therefore they will be dubbed action stats
	void GenStats ()
	{
		
		double HPmax = 50 + (10 * Constitution) + HPmaxModifier;
		int APmax = 3 + (Dexterity / 2) + (Intelligence / 5) + APmaxModifier;
		double CarryWeight = 100 + (10 * Strength) + (2 * Constitution) +CarryWeightModifier;
		double Accuracy = (Perception * 5) + (Dexterity * 3) + AccuracyModifier;
		double Defence = (Constitution * 5) + (Dexterity) + DefenceModifier;
		double Dodge = (Dexterity * 3) + (Perception * 2) + DodgeModifier;
		double AttackCost = 3 - (Dexterity / 4) + AttckCostModifier;
		int intAttackCost = (int)AttackCost;
		double CritChance = (Perception * 3) + (Dexterity) + (Intelligence);


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
				if (Target.HP <= 0)
				{
				/*
				need to add the code for level up here
				//this makes leveling faster the stronger the enemies are
				XP = XP + (Target.XPReward / XPReward) * 10
				*/

				}


			}
		}
		else
		{
			print ("not enough AP to attack");
		}


	}


}
