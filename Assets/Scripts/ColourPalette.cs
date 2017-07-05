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
