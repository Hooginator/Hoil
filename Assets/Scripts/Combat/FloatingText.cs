using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***********************************************************/
// Used to generate floating text that will stay for a second or so.
// For displaying the amount of damage taken. 
/***********************************************************/

public class FloatingText : MonoBehaviour {
	// Information on what the text will look like
	public int damage;
	public string text;
	public string damageType;
	public Color textColor;
	public Vector3 position;
	public int totalTime;
	public int halfTime;
	public float movespeed;
	// Time the text has existed for
	private int t;

	// Use this for initialization
	void Start () {
		totalTime = 60;
		halfTime = totalTime / 15;
		movespeed = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		// Timekeeping
		t++;
		if (t > halfTime) {
			movespeed = 0;
			// no longer want this to activate
			halfTime = totalTime + 1;
		}
		// 5 second hard maximum ATM
		if (t > totalTime) {
			destroyText ();
		}
		gameObject.transform.position += new Vector3 (0, movespeed, movespeed);
	}
	void destroyText(){
		Destroy (this.gameObject);
	}

	// Setup
	public void setText(float num, string type){
		// Set what the text is 
		damage = (int)num;
		text = num.ToString ();
		setSize ();
		setText (text,type);
	}
	public void setText(string str, string type){
		// Set what the text is 
		text = str;
		damageType = type;
		checkColor ();
		updateText ();
	}
	public void setSize (){
		gameObject.GetComponent<TextMesh> ().fontSize = 18 + damage / 10;
	}
	public void setPosition(Transform TR){
		position = TR.position;
		updatePosition();
	}

	// Update
	void updateText(){
		gameObject.GetComponent<TextMesh> ().text = text;
		gameObject.GetComponent<TextMesh> ().color = textColor;
	}
		
	void updatePosition(){
		this.gameObject.transform.position = position;
	}
	void checkColor(){
		// Dictates which colour to use based on the damage type
		if (damageType == "Fire") {
			textColor = new Color (0.9f, 0.2f, 0.2f, 1);
		} else if (damageType == "Ice") {
			textColor = new Color (0.5f, 0.5f, 0.9f, 1);
		} else if (damageType == "Toxic") {
			textColor = new Color (0.2f, 0.9f, 0.2f, 1);
		} else if (damageType == "Electric") {
			textColor = new Color (0.1f, 0.1f, 0.9f, 1);
		} else {
			textColor = new Color (0.9f, 0.9f, 0.9f, 1);
		}
	}
}
