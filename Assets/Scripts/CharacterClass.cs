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
	public int HPmax;
	public int APmax;
	public int HP;
	public int AP;
	public double CarryWeight;
	public double Damage;
	public double Accuracy;
	public double Defence;
	public double Dodge;
	public double Range;
	public StatusEffect Inflicted;


	void Attack ()
	{
		double AttackCost = 3 - Dexterity / 4;
		int intAttackCost = (int)AttackCost;
		CharacterClass Target;
		int HitChance = 100+Accuracy-Target.Dodge;
		Random RandomHit = new Random();


		if (AP>=intAttackCost)
		{
			AP = AP - intAttackCost;

			//roll to see if the attack hits or misses
			RandomHit.Next (1, 101);
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
