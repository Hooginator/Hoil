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
	public Vector3 setectedTarget;
	public float timeToLand;
	public float timeToCast;
	public CharacterClass caster;
	public string rangeType;

	public void init(string nameIn, int baseRangeIn, int AoERangeIn, float baseDamageIn, string damageTypeIn,
		string targetingTypeIn, string targetsIn, CharacterClass casterIn, float timeToCastIn, float timeToLandIn, string rangeTypeIn){

		name = nameIn; // For case switch
		baseRange = baseRangeIn; // Max distance you can cast away from caster
		AoERange = AoERangeIn; // max distance from centre of cast that someone will be hit
		baseDamage = baseDamageIn; 
		damageType = damageTypeIn;
		targetingType = targetingTypeIn; // area, single target..
		targets = targetsIn; // allies only, all flying characterrs...
		caster = casterIn;
		timeToCast = timeToCastIn;
		timeToLand = timeToLandIn;
		rangeType = rangeTypeIn; // Line of sight, all in range...
	}

	public void init(string nameIn, CharacterClass casterIn){
		// Create an ability, this will be the ugly part that mightg be replaced with a nice DB some day
		name = nameIn;
		caster = casterIn;
		switch (nameIn) {
		case "Fireball":
			init (nameIn, 8, 3, 30, "Fire", "Area", "Enemy", casterIn, 1.0f, 0.5f, "LOS"); 
			break;
		case "Iceball":
			init (nameIn, 5, 5, 20, "Ice", "Area", "Enemy", casterIn, 1.0f, 0.5f, "LOS"); 
			break;
		case "Acidball":
			init (nameIn, 7, 2, 30, "Toxic", "Area", "Enemy", casterIn, 1.0f, 0.5f, "LOS"); 
			break;
		case "Sniper Attack":
			init (nameIn, 8, 3, 30, "Pierce", "Single", "Enemy", casterIn, 1.0f, 0.1f, "LOS"); 
			break;
		case "Healing":
			init (nameIn, 4, 2, -30, "Heal", "Area", "Ally", casterIn, 1.0f, 0.1f, "LOS"); 
			break;
		case "Basic Attack":
			init (nameIn, 0, 0, 20, "Normal", "Single", "Enemy", casterIn, 0.4f, 0.1f, "LOS"); 
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
			temp.GetComponent<FireBallCastAnimation> ().init (startPos,stopPos, colourPalettes[0] ,AoERange,timeToCast, timeToLand);
		} else if (name == "Iceball") {
			// recolour of fireball
			GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("FireBallCastAnimation"));
			temp.GetComponent<FireBallCastAnimation> ().init (startPos,stopPos, colourPalettes[1] ,AoERange,timeToCast, timeToLand);
		} else if (name == "Acidball") {
			// recolour of fireball
			GameObject temp = (GameObject)GameObject.Instantiate (Resources.Load ("FireBallCastAnimation"));
			temp.GetComponent<FireBallCastAnimation> ().init (startPos,stopPos, colourPalettes[2] ,AoERange,timeToCast, timeToLand);
		}

		// Stuff we're doing no matter what ability it is
		caster.battleAvatar.GetComponent<BasicEnemyAnimations> ().castTowards (stopPos - startPos, timeToCast);
	}

	public bool cast(CharacterClass target){
		// What to do damage wise for each ability
		target.takeDamage(baseDamage + caster.Intelligence, damageType,timeToLand + timeToCast);
		target.battleAvatar.GetComponent<BasicEnemyAnimations> ().recoilFromIn (0.4f*(target.battleAvatar.transform.position - setectedTarget),timeToLand + timeToCast);
		if (target.checkDead ()) {
			return true;
		}
		return false;
	}
}
