using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/***********************************************************/
// Routine to use a special ability in battle.
// Includes constructor and cast function to apply the ability.
/***********************************************************/

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
			baseDamage = 30;
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
	public void doAnimation(CharacterClass target){
		// reserved for solo animations 
	}
	public void doAnimation(Vector3 pos){
		// Do animation at position
		if (name == "Fireball") {
			GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("Basic Explosion"));
			temp.transform.position = pos;
		} else {

		}
	}
	public bool cast(CharacterClass target){
		// What to do damage wise for each ability
		target.takeDamage(baseDamage + caster.Intelligence, damageType);
		if (target.checkDead ()) {
			return true;
		}
		return false;
	}
}
