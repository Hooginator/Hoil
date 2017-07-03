using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler {

	// position of terget
	public int [] pos;
	Map map;
	CombatTracker CT;
	void Awake(){
		map = GameObject.Find ("Map").GetComponent<Map> ();
		CT = GameObject.Find ("Battle Menu").GetComponent<CombatTracker> ();
	}

	public void OnPointerEnter(PointerEventData eventData){

	}
	public void OnSelect(BaseEventData eventData){
		if (CT.actionToDo != null && CT.actionToDo.targetingType == "Single") {
			map.selectCentralUnit (pos [0], pos [1]);
		}
	}
	public void OnDeselect(BaseEventData eventData){
		if (CT.actionToDo != null) {
			map.deSelectUnit (pos [0], pos [1]);
		}
	}
}
