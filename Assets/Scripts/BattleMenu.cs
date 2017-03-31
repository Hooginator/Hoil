﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour {
	// Options for Player Combat Main Menu
	public Button Attack;
	public Button Special;
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
		Special = Special.GetComponent<Button> ();
		Item = Item.GetComponent<Button> ();
		Run = Run.GetComponent<Button> ();
		// Get the instance of Combat Tracker to switch turns and stuff
		combat = combat.GetComponent<CombatTracker> ();
	}

	/********************************************************************************************/ 
	/******************************************* Buttons ****************************************/ 
	/********************************************************************************************/


	public void AttackPress(){
		// When you hit that attack button
		//print ("ATTACKKKKK");
		combat.HideBattleMenu ();
		//combat.CreateSelectOptions ();
		combat.ShowSelectMenu (combat.maxEnemyCharacters,combat.enemyCharacters);
		//combat.PlayerAttack(0,0);
	}	
	public void MovePress(){
		// When you hit that attack button
		print ("MOOOVE");
		combat.HideBattleMenu ();
		combat.actionToDo = "Move";
		int[] pos = combat.actionFrom.battleLocation;
		int MP = combat.actionFrom.MP;
		combat.selectTargetLocation (pos[0],pos[1],MP);
	}
	public void AttackTarget(int target){
		//print ("Attacking Target: " + target.ToString ());
		combat.HideSelectMenu ();
		combat.PlayerAttack(0,target);
	}
	public void ItemPress(){
		// When you hit the Item Button
		//print ("Item");
		combat.PlayerItem ();
	}
	public void EndTurnPress(){
		// When you hit the Item Button
		//print ("Item");
		combat.PlayerEndTurn ();
	}
	public void SpecialPress(){
		combat.HideBattleMenu ();
		combat.ShowAbilitiesMenu ();
		//combat.selectTargetLocation (1,1,3);
		// WHen you hit the Special Button
		//print ("Special");
		combat.PlayerSpecial ();
	}
	public void RunPress(){
		// When you hit the Run button
		//print ("Run");
		combat.PlayerRun ();
	}
}
