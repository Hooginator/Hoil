using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour {
	public Button Attack;
	public Button Special;
	public Button Item;
	public Button Run;

	public CombatTracker combat;
	// Use this for initialization
	void Start () {
		Attack = Attack.GetComponent<Button> ();
		Special = Special.GetComponent<Button> ();
		Item = Item.GetComponent<Button> ();
		Run = Run.GetComponent<Button> ();
		combat = combat.GetComponent<CombatTracker> ();
	}
	public void AttackPress(){
		// When you hit that attack button
		print ("ATTACKKKKK");
		combat.PlayerAttack(0,0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
