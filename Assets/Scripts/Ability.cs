using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability {

	public string name;
	public int baseRange;
	public int AoERange;
	public float baseDamage;
	public string damageType;
	public string targetingType;
	public string targets;
	public CharacterClass caster;

	public Ability(string nameIn, int baseRangeIn, int AoERangeIn, float baseDamageIn, string damageTypeIn,string targetingTypeIn, string targetsIn, CharacterClass casterIn){
		// Create an ability
		name = nameIn;
		baseRange = baseRangeIn;
		AoERange = AoERangeIn;
		baseDamage = baseDamageIn;
		damageType = damageTypeIn;
		targetingType = targetingTypeIn;
		targets = targetsIn;
		caster = casterIn;
	}

	public Ability(string nameIn, CharacterClass casterIn){
		// Create an ability
		name = nameIn;
		caster = casterIn;
		switch (nameIn) {
		case "Fireball":
			baseRange = 4;
			AoERange = 3;
			baseDamage = 10;
			damageType = "Fire";
			targets = "Enemy";
			targetingType = "Area";
			break;
		case "Sniper Attack":
			baseRange = 7;
			AoERange = 0;
			baseDamage = 20;
			damageType = "Pierce";
			targets = "Enemy";
			targetingType = "Single";
			break;
		case "Healing":
			baseRange = 3;
			AoERange = 2;
			baseDamage = -20;
			damageType = "Heal";
			targets = "Ally";
			targetingType = "Area";
			break;
		case "Basic Attack":
			baseRange = 1;
			AoERange = 0;
			baseDamage = 10;
			damageType = "Normal";
			targets = "Enemy";
			targetingType = "Single";
			break;
		}
	}
	public void cast(CharacterClass target){
		// What to cast for each ability
		target.takeDamage(baseDamage + caster.Intelligence);
	}
}
