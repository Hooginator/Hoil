using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour {

	// these are the different Status effects
	public bool Bleed;
	public bool Poison;
	public bool Stun;
	public bool Blind;
	public bool Drunk;
	public bool HeatStroke;
	public bool FrostBite;

	// these are the modifers that are changed by statuseffects
	double AttckCostStatusModifier;

}
