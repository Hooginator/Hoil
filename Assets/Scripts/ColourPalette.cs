using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPalette : ScriptableObject {

	public string name;
	public List<Color> colourList;
	int count;
	// Use this for initialization
	void Start () {
		
	}

	public static List<ColourPalette> initializeColourPalette(List<ColourPalette> colourPalettes){
		// Here I will have all of the colour sets for the palettes 
		// Red fireball stuff
		ColourPalette temp = new ColourPalette();
		List<Color> tempColour = new List<Color>();
		tempColour.Add (new Color (0.6f, 0.1f, 0.1f));
		tempColour.Add (new Color (0.7f,0.5f,0.5f));
		tempColour.Add (new Color (0.8f,0.3f,0.3f));
		temp.init (tempColour,"Fire");
		colourPalettes.Add (temp);
		// Blue ice stuff
		temp = null;
		tempColour = null;
		temp = new ColourPalette();
		tempColour = new List<Color>();
		tempColour.Add (new Color (0.1f, 0.1f, 0.9f));
		tempColour.Add (new Color (0.2f,0.4f,0.8f));
		tempColour.Add (new Color (0.0f,0.3f,0.7f));
		temp.init (tempColour,"Ice");
		colourPalettes.Add (temp);
		// Green toxic stuff
		temp = null;
		tempColour = null;
		temp = new ColourPalette();
		tempColour = new List<Color>();
		tempColour.Add (new Color (0.0f, 0.9f, 0.1f));
		tempColour.Add (new Color (0.6f,0.9f,0.4f));
		tempColour.Add (new Color (0.2f,0.6f,0.1f));
		temp.init (tempColour,"Toxic");
		colourPalettes.Add (temp);
		return colourPalettes;
	}

	public void init(List<Color> coloursIn,string nameIn){
		colourList = coloursIn;
		count = colourList.Count;
		name = nameIn;
	}

	public Color getColour(int i){
		// Gives out colours in order
		return colourList [i % count];
	}

	public void addColour(Color colourIn){
		colourList.Add (colourIn);
		count = colourList.Count;
	}

}
