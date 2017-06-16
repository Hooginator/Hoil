using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***********************************************************/
// Used to generate floating text that will stay for a second or so.
// For displaying the amount of damage taken. 
/***********************************************************/

public class FloatingText : MonoBehaviour {
	// Information onf what the text will look like
	public int damage;
	public string text;
	public string damageType;
	public Color textColor;
	public Vector3 position;
	public int totalTime = 100;
	public float movespeed = 0.05f;
	// Time the text has existed for
	private int t;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Timekeeping
		t++;
		// 5 second hard maximum ATM
		if (t > totalTime) {
			destroyText ();
		}
		gameObject.transform.position += new Vector3 (0, movespeed, movespeed);
		//gameObject.transform.parent = null;
		//gameObject.transform.lossyScale += new Vector3(0.1f,0.1f,0.1f);
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
		gameObject.GetComponent<TextMesh> ().fontSize = 10 + damage / 100;
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
			textColor = new Color (255, 100, 100, 1);
		} else if (damageType == "Ice") {
			textColor = new Color (200, 200, 255, 1);
		} else if (damageType == "Toxic") {
			textColor = new Color (100, 255, 100, 1);
		} else if (damageType == "Electric") {
			textColor = new Color (100, 100, 255, 1);
		} else {
			textColor = new Color (255, 255, 255, 1);
		}
	}
}
