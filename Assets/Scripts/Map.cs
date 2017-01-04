using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
	public int NcellX;
	public int NcellZ;
	//public MapGridUnit[,] MapGrid; 
	public GameObject Prefab;
	// Use this for initialization
	void Start () {
		//MapGrid = new MapGridUnit[NcellX,NcellZ];
		for (int z = 0; z < NcellZ; z++) {
			for (int x = 0; x < NcellX; x++) {
				//print (x * 20);
				Instantiate (Prefab, new Vector3 (x*10, 0, z*10), Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
