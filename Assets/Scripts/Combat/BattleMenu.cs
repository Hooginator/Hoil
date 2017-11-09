using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/***********************************************************/
// Shows the options at the start of one's turn or after completing or cancelling an action.
// Most functions just refer to what's in the Combat Tracker.
/***********************************************************/

public class BattleMenu : MonoBehaviour {
	// Options for Player Combat Main Menu
	public Button Attack;
	//public Button Special;
	public Button Item;
	public Button Run;
	public CombatTracker combat;

	/********************************************************************************************/ 
	/**************************************** Initialization ************************************/ 
	/********************************************************************************************/

	// Use this for initialization
	void Start () {
		// Initialize the Buttons
		Attack = Attack.GetComponent<Button> ();
		//Special = Special.GetComponent<Button> ();
		Item = Item.GetComponent<Button> ();
		Run = Run.GetComponent<Button> ();
		// Get the instance of Combat Tracker to switch turns and stuff
		combat = combat.GetComponent<CombatTracker> ();
	}

	/********************************************************************************************/ 
	/******************************************* Buttons ****************************************/ 
	/********************************************************************************************/


	public void MovePress(){
		// When you hit that move button
		combat.HideBattleMenu ();
		Ability temp = ScriptableObject.CreateInstance ("Ability") as Ability;
		temp.init ("Move", combat.actionFrom);
		combat.actionToDo = temp;

		int[] pos = combat.actionFrom.battleLocation;
		int MP = combat.actionFrom.getMP();
		combat.selectTargetMovableLocation (pos[0],pos[1],MP);
	}
	public void AttackTarget(int target){
		//print ("Attacking Target: " + target.ToString ());
		// Basig attack will be moved into what is currently special / ability use.  That will be the new attack button
		combat.HideSelectMenu ();
		combat.PlayerAttack(0,target);
	}
	public void ItemPress(){
		// When you hit the Item Button
		combat.PlayerItem ();
	}
	public void EndTurnPress(){
		combat.PlayerEndTurn ();
	}
	public void AttackPress(){
		combat.HideBattleMenu ();
		combat.ShowAbilitiesMenu ();
		//combat.selectTargetLocation (1,1,3);
		// WHen you hit the Special Button
		combat.PlayerSpecial ();
	}
	public void RunPress(){
		// When you hit the Run button
		combat.PlayerRun ();
	}
}
