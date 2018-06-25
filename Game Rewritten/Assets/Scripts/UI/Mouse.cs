using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mouse : MonoBehaviour {

	public const byte LEFT_CLICK = 0;
	public const byte RIGHT_CLICK = 1;

	public static GameObject getGameObject(string tag){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)){
			if(hit.transform.CompareTag(tag))
				return hit.transform.gameObject;
		}
		return null;
	}

	public static GameObject getGameObject(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)){
			return hit.transform.gameObject;
		}
		return null;
	}

	public static bool isOverUI(GameObject o){
		if(o == null) return false;

		PointerEventData pointerData = new PointerEventData(EventSystem.current){
			pointerId = -1
		};
		pointerData.position = Input.mousePosition;

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);
		foreach(RaycastResult r in results){
			if(r.gameObject.name.Equals(o.name)){
				return true;
			}
		}
		return false;
	}

	public static bool isOverUIBoundingBox(GameObject o){
		if(o == null) return false;

		RectTransform rect = o.GetComponent<RectTransform>();

		if(Input.mousePosition.x > o.transform.position.x-(rect.sizeDelta.x/2) &&
			Input.mousePosition.x < o.transform.position.x+(rect.sizeDelta.x/2) && 
			Input.mousePosition.y > o.transform.position.y-(rect.sizeDelta.y/2) &&
			Input.mousePosition.y < o.transform.position.y+(rect.sizeDelta.y/2)){
			return true;
		}

		return false;
	}

	public static bool isOverUI(){
		return EventSystem.current.IsPointerOverGameObject();
	}

	public static bool mouseInAir(){
		return getGameObject() == null && !isOverUI();
	}
}
