﻿using System.Collections;
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
	public Vector3 setectedTarget;
	public float timeToLand;
	public float timeToCast;
	public CharacterClass caster;
	public string rangeType;

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
		// Create an ability, this will be the ugly part that mightg be replaced with a nice DB some day
		name = nameIn;
		caster = casterIn;
		switch (nameIn) {
		case "Fireball":
			baseRange = 2;
			AoERange = 3;
			baseDamage = 30;
			damageType = "Fire";
			targets = "Enemy";
			targetingType = "Area";
			timeToLand = 1.5f;
			timeToCast = 1.0f;
			rangeType = "LOS";
			break;
		case "Iceball":
			baseRange = 4;
			AoERange = 7;
			baseDamage = 20;
			damageType = "Ice";
			targets = "Enemy";
			targetingType = "Area";
			timeToLand = 1.5f;
			timeToCast = 1.0f;
			rangeType = "LOS";
			break;
		case "Acidball":
			baseRange = 3;
			AoERange = 2;
			baseDamage = 40;
			damageType = "Toxic";
			targets = "Enemy";
			targetingType = "Area";
			timeToLand = 1.5f;
			timeToCast = 1.0f;
			rangeType = "LOS";
			break;
		case "Sniper Attack":
			baseRange = 20;
			AoERange = 0;
			baseDamage = 80;
			damageType = "Pierce";
			targets = "Enemy";
			targetingType = "Single";
			rangeType = "LOS";
			break;
		case "Healing":
			baseRange = 3;
			AoERange = 2;
			baseDamage = -20;
			damageType = "Heal";
			targets = "Ally";
			targetingType = "Area";
			rangeType = "LOS";
			break;
		case "Basic Attack":
			baseRange = 1;
			AoERange = 0;
			baseDamage = 20;
			damageType = "Normal";
			targets = "Enemy";
			targetingType = "Single";
			rangeType = "LOS";
			break;
		}
	}
	public void doAnimation(CharacterClass target){
		// reserved for solo animations 
	}
	public void doAnimation(Vector3 startPos, Vector3 stopPos, List<ColourPalette> colourPalettes){
		setectedTarget = stopPos;
		int colourPal;
		// Do animation at position indicated.
		if (name == "Fireball") {
			GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("FireBallCastAnimation"));
			temp.GetComponent<FireBallCastAnimation> ().init (startPos,stopPos, colourPalettes[0] ,AoERange);
		} else if (name == "Iceball") {
			// recolour of fireball
			GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("FireBallCastAnimation"));
			temp.GetComponent<FireBallCastAnimation> ().init (startPos,stopPos, colourPalettes[1] ,AoERange);
		} else if (name == "Acidball") {
			// recolour of fireball
			GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("FireBallCastAnimation"));
			temp.GetComponent<FireBallCastAnimation> ().init (startPos,stopPos, colourPalettes[2] ,AoERange);
		}

		// Stuff we're doing no matter what ability it is
		caster.battleAvatar.GetComponent<BasicEnemyAnimations> ().castTowards (stopPos - startPos, timeToCast);
	}

	public bool cast(CharacterClass target){
		// What to do damage wise for each ability
		target.takeDamage(baseDamage + caster.Intelligence, damageType,timeToLand);
		target.battleAvatar.GetComponent<BasicEnemyAnimations> ().recoilFromIn (0.4f*(target.battleAvatar.transform.position - setectedTarget),timeToLand);
		if (target.checkDead ()) {
			return true;
		}
		return false;
	}
}
