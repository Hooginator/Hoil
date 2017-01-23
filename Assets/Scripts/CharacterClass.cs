using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass : MonoBehaviour
{
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
	public StatusEffect Inflicted;


	void Attack (CharacterClass Target)
	{
		
		double AttackCost = 3 - Dexterity / 4;
		int intAttackCost = (int)AttackCost;
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
