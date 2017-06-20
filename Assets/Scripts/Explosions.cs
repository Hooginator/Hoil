using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***********************************************************/
// Class that will have some useful functions for all explosions / combat effects
/***********************************************************/


public class Explosions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(DestroyAfterTime (this.gameObject, 1.2f));
	}


	IEnumerator DestroyAfterTime(GameObject toDestroy, float t){
		// Coroutine to let an animation start t seconds after current time.
		yield return new WaitForSeconds(t);
		GameObject.Destroy (toDestroy.gameObject);
	}
}
