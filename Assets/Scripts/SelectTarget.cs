using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTarget : MonoBehaviour {
	public List<Button> option  = new  List<Button>();
	public CombatTracker combat;
	public int maxTargets;

	public Transform TR;
	public GameObject prefabbutton;

	public BattleMenu battlemenu;

	// Update is called once per frame
	void Update () {

	}


	/********************************************************************************************/
	/************************************* Initialization ***************************************/
	/********************************************************************************************/

	// Use this for initialization
	void Start () {
		// Get the instance of Combat Tracker to switch turns and stuff
		combat = combat.GetComponent<CombatTracker> ();
		TR = GetComponent<Transform> ();
		battlemenu = battlemenu.GetComponent<BattleMenu>();
	}
	public void CreateAttackOptions(int nCharacters, List<CharacterClass> targets){//, List<CharacterClass> targets){
		int targetNumber = 0;
		for (int i = 0; i < nCharacters; i++) {
			if (!targets [i].isDead) {
				maxTargets = nCharacters;
				GameObject temp = (GameObject)Instantiate (prefabbutton);
				temp.transform.SetParent (TR, false);
				Text buttonText = temp.GetComponentInChildren<Text>();
				buttonText.text = targets [i].name;
				//option.Add (temp.GetComponent<Button> ());
				Vector3 pos = temp.transform.position;
				pos [1] -= targetNumber * 30;
				print (pos [1].ToString ());
				temp.transform.position = pos;
				//temp.onClick.AddListener (() => test (i));
				//print ("Set for enemy" + i.ToString ());
				option.Insert (targetNumber, temp.GetComponent<Button>());
				// Need a tempi variable here for the int here to make sure each button has a different int (weird isue, I dunno kny really)
				int tempi = i;
				// Add a function to the button for when it's pressed
				option [targetNumber].GetComponent<Button> ().onClick.AddListener (delegate {
					AttackTarget (tempi);
				});
				// This way always has each button store the last value of i in it...
				// option[i].GetComponent<Button>().onClick.AddListener (delegate{testtemp (i);});
				//print (option [i].transform.position.ToString () + "   " + i.ToString () + "    " + option.Count.ToString ());
				temp = null;
				targetNumber++;
			}
		}
	}

	public void CreateAbilityOptions(CharacterClass caster){
		// Creates a list of casting abilities to use
		List<string> abilities = caster.abilities;
		for(int i = 0; i < abilities.Count; i++){
			// Create Button Object
			GameObject temp = (GameObject)Instantiate (prefabbutton);
			temp.transform.SetParent (TR, false);
			/// Reneme Button
			Text buttonText = temp.GetComponentInChildren<Text>();
			buttonText.text = abilities [i];

			// Reposition Buttons
			Vector3 pos = temp.transform.position;
			pos [1] -= i * 30;
			print (pos [1].ToString ());
			temp.transform.position = pos;


			option.Insert (i, temp.GetComponent<Button>());
			print (i.ToString () + "  " + combat.actionFrom.abilities [i]);
			string tempAbility = abilities [i];
			option [i].GetComponent<Button> ().onClick.AddListener (delegate {
				DoAbility (tempAbility);
			});
		};
	}
	/********************************************************************************************/
	/************************************* Act On Target ****************************************/
	/********************************************************************************************/


	public void DestroyOptions(){
		for (int i = 0; i < option.Count; i++) {
			Destroy (option [i].gameObject);
		}
		option.Clear ();
	}
	void AttackTarget(int num){
		//print ("Clicked for enemy" + num.ToString ());
		battlemenu.AttackTarget (num);
	}
	void DoAbility(string ability){

		combat.HideSelectMenu ();
		print ("Doing ability " + ability);
		combat.actionToDo = ability;
		int rangeBase = 0;
		if (ability == "Fireball") {
			rangeBase = 4;
			combat.areaRange = 3;
			combat.selectTargetLocation (rangeBase);
		}else if(ability == "Sniper Attack"){
			// Change to individual target
			rangeBase = 8;
			combat.selectTargetLocation (rangeBase);
			combat.areaRange = 1;
		}else if(ability == "Heal Self"){
			// Change select method to individual ally, or just fuck it
			rangeBase = 0;
			combat.selectTargetLocation (rangeBase);
		}
		combat.selectingRange = rangeBase;
		//combat.selectTargetLocation (rangeBase);
	}

}
