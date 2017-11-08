using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler {

	// class used to highlight selections from button.
	public int [] pos;
	Map map;
	CombatTracker CT;
	void Awake(){
		map = GameObject.Find ("Map").GetComponent<Map> ();
		CT = GameObject.Find ("Battle Menu").GetComponent<CombatTracker> ();
		pos = null;
	}

	public void OnPointerEnter(PointerEventData eventData){

	}
	public void OnSelect(BaseEventData eventData){
		// Light up the square under the target you might select
		if (CT.actionToDo != null && CT.actionToDo.targetingType == "Single") {
			map.selectCentralUnit (pos);
		}
	}
	public void OnDeselect(BaseEventData eventData){
		if (CT.actionToDo != null && pos != null) {
			map.deSelectUnit (pos [0], pos [1]);
		}
	}
}
