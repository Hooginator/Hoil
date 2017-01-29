﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTarget : MonoBehaviour {
	public List<Button> option  = new  List<Button>();
	public CombatTracker combat;
	public int maxTargets;

	public Transform TR;
	public Button prefabbutton;

	public BattleMenu battlemenu;
	// Use this for initialization
	void Start () {
		// Get the instance of Combat Tracker to switch turns and stuff
		combat = combat.GetComponent<CombatTracker> ();
		TR = GetComponent<Transform> ();
		battlemenu = battlemenu.GetComponent<BattleMenu>();
	}
	public void CreateOptions(int nCharacters, List<CharacterClass> targets){//, List<CharacterClass> targets){
		for (int i = 0; i < nCharacters; i++) {
			if (!targets [i].isDead) {
				maxTargets = nCharacters;
				Button temp = (Button)Instantiate (prefabbutton);
				temp.transform.SetParent (TR, false);
				option.Add (temp.GetComponent<Button> ());
				Vector3 pos = temp.transform.position;
				pos [1] -= i * 30;
				print (pos [1].ToString ());
				temp.transform.position = pos;
				//temp.onClick.AddListener (() => test (i));
				print ("Set for enemy" + i.ToString ());
				option.Insert (i, temp);
				// Need a tempi variable here for the int here to make sure each button has a different int (weird isue, I dunno kny really)
				int tempi = i;
				option [tempi].GetComponent<Button> ().onClick.AddListener (delegate {
					testtemp (tempi);
				});
				// This way always has each button store the last value of i in it...
				// option[i].GetComponent<Button>().onClick.AddListener (delegate{testtemp (i);});
				print (option [i].transform.position.ToString () + "   " + i.ToString () + "    " + option.Count.ToString ());
				temp = null;
			}
		}
	}
	public void DestroyOptions(){
		for (int i = 0; i < option.Count; i++) {
			Destroy (option [i].gameObject);
		}
		option.Clear ();
	}
	void testtemp(int num){
		print ("Clicked for enemy" + num.ToString ());
		battlemenu.AttackTarget (num);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
