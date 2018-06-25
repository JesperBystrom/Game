using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMover : MonoBehaviour {

	private GameObject attachedObject;
	public static GridMover instance;

	void Start(){
		instance = this;
	}

	void Update () {
		GameObject o = Mouse.getGameObject("Territory");
		if(o != null){
			transform.position = new Vector3(o.transform.position.x, transform.position.y, o.transform.position.z);
		}
		if(attachedObject != null){
			attachedObject.transform.position = transform.position;
		}
	}

	public void attachObject(GameObject o){
		this.attachedObject = o;
	}

	public void detachObject(){
		this.attachedObject = null;
	}

	public Territory getTerritory(){
		return Map.instance.findTerritory((int)transform.position.x, (int)transform.position.z);
	}
}