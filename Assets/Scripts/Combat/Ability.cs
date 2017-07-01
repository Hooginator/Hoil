using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/***********************************************************/
// Routine to use a special ability in battle.
// Includes constructor and cast function to apply the ability.
/***********************************************************/

public class Ability : ScriptableObject{

	public string name;
	public int baseRange;
	public int AoERange;
	public float baseDamage;
	public string damageType;
	public string targetingType;
	public string targets;
	public CharacterClass caster;

	public void init(string nameIn, int baseRangeIn, int AoERangeIn, float baseDamageIn, string damageTypeIn,string targetingTypeIn, string targetsIn, CharacterClass casterIn){
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

	public void init(string nameIn, CharacterClass casterIn){
		// Create an ability
		name = nameIn;
		caster = casterIn;
		switch (nameIn) {
		case "Fireball":
			baseRange = 6;
			AoERange = 3;
			baseDamage = 30;
			damageType = "Fire";
			targets = "Enemy";
			targetingType = "Area";
			break;
		case "Sniper Attack":
			baseRange = 7;
			AoERange = 0;
			baseDamage = 80;
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
			baseDamage = 20;
			damageType = "Normal";
			targets = "Enemy";
			targetingType = "Single";
			break;
		}
	}
	public void doAnimation(CharacterClass target){
		// reserved for solo animations 
	}
	public void doAnimation(Vector3 startPos, Vector3 stopPos){
		// Do animation at position
		if (name == "Fireball") {
			//GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("Basic Explosion"));
			GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("FireBallCastAnimation"));
			temp.transform.position = startPos;
			temp.GetComponent<FireBallCastAnimation> ().init (startPos,stopPos);
			temp.GetComponent<FireBallCastAnimation> ().reColour(new Color (1, 0.5f, 0.5f, 1));
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
