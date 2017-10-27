using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/***********************************************************/
// Custom version of BATTLE MENU that is used to display options in range, abilities / items to use...
// Buttons trigger what is to be done via COMBAT TRACKER calls.
/***********************************************************/


public class SelectTarget : MonoBehaviour {
	public List<Button> option  = new  List<Button>();
	public CombatTracker combat;
	public int maxTargets;

	public Transform TR;
	public GameObject prefabbutton;

	public BattleMenu battlemenu;

	public Ability AbilityToCast;


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
		// Creates single target attack target options for the battle menu.  Will read each character fed in range and add their name as an option.
		int targetNumber = 0;
		for (int i = 0; i < nCharacters; i++) {
			if (!targets [i].isDead) {
				maxTargets = nCharacters;

				// Create generic button option
				GameObject temp = (GameObject)Instantiate (prefabbutton);
				temp.transform.SetParent (TR, false);

				// Set Text on the button to target's name
				Text buttonText = temp.GetComponentInChildren<Text>();
				buttonText.text = targets [i].name;

				// Place Button in unique location on screen
				Vector3 pos = temp.transform.position;
				pos [1] -= targetNumber * 30;
				temp.transform.position = pos;

				// Add button to the list of options
				option.Insert (targetNumber, temp.GetComponent<Button>());

				// Add an   {  AttackTarget (tempTarget);  }  function call to the button for when it is pressed
				CharacterClass tempTarget = targets [i];
				option [targetNumber].GetComponent<Button> ().onClick.AddListener (delegate {
					AttackTarget (tempTarget);
				});
				option [targetNumber].GetComponent<ButtonManager> ().pos = targets[i].battleLocation;

				// Stamp Collecting
				temp = null;
				targetNumber++;
			}
		}
	}

	public void CreateAbilityOptions(CharacterClass caster){
		// Creates a list of casting abilities to use
		List<Ability> abilities = caster.abilities;
		for(int i = 0; i < abilities.Count; i++){
			
			// Create Button Object
			GameObject temp = (GameObject)Instantiate (prefabbutton);
			temp.transform.SetParent (TR, false);

			/// Rename Button
			Text buttonText = temp.GetComponentInChildren<Text>();
			buttonText.text = abilities [i].name;

			// Reposition Buttons
			Vector3 pos = temp.transform.position;
			pos [1] -= i * 30;
			temp.transform.position = pos;

			// Add button to the list of options
			option.Insert (i, temp.GetComponent<Button>());

			// Add a  {  DoAbility (tempAbility);  }   function call to the button for when it is pressed
			Ability tempAbility = abilities [i];
			option [i].GetComponent<Button> ().onClick.AddListener (delegate {
				//UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
				DoAbility (tempAbility);
			});
		};
	}
	/********************************************************************************************/
	/************************************* Act On Target ****************************************/
	/********************************************************************************************/


	public void DestroyOptions(){
		// Clear out the options used for selecting a target last time
		for (int i = 0; i < option.Count; i++) {
			Destroy (option [i].gameObject);
		}
		option.Clear ();
	}
	void AttackTarget(CharacterClass target){
		// Now that we have selected a character, proceed with  the attack
		combat.HideSelectMenu ();
		combat.actionTo = target;
		combat.doAction();
	}
	void DoAbility(Ability ability){
		// Now that we have selected a character, proceed with  the ability location / target select
		AbilityToCast = ability;
		combat.HideSelectMenu ();
		combat.actionToDo = ability;
		int rangeBase = 0;
		combat.map.deSelectAll();
		// Decide on stats to use based on the ability name
		// I might replace the choosing method to a dictionary  or something
		if (ability.targetingType == "Area") {
			combat.areaRange = ability.AoERange;
			combat.selectTargetLocation (ability.baseRange, ability.rangeType);
		} else if (ability.targetingType == "Single") {
			combat.ShowSelectMenu (combat.getEnemiesInRange (ability.baseRange, "Player"));
		}
		combat.selectingRange = ability.baseRange;

	}

}
