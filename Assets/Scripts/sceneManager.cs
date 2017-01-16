using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour {
	public bool inBattle = false;
	public static sceneManager instance = null;
	public void LoadScene(string sceneName){
		SceneManager.LoadScene (sceneName);
	}
	public void UnLoadScene(string sceneName){
		this.UnLoadScene (sceneName);
	}
	void Start(){
		// Makes sure that the GameManager this is attached to is always the same one, so we can use it to keep values through scenes.
		if (instance == null) {
			instance = this;
		} else if (instance != this){
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}
}
