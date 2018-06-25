using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToGame : MonoBehaviour {

	private RectTransform rect;
	public GameObject anchorObject;
	public int deathTimer;
	private Vector3 vec;
	private float direction;
	private float speed;
	private float yOffset;

	void Start () {
		rect = GetComponent<RectTransform>();
	}

	void Update () {
		if(anchorObject == null) return;
		Vector3 v = Camera.main.WorldToScreenPoint(anchorObject.transform.position);
		transform.position = new Vector3(v.x, v.y+((rect.sizeDelta.y+yOffset)), v.z);
	}

	public void anchor(GameObject o, float yOffset){
		this.anchorObject = o;
		this.vec = o.transform.position;
		this.yOffset = yOffset;
	}

	public void moveRelativeTo(Vector3 vec, float direction, float speed){
		this.vec = vec;
		this.direction = direction;
		this.speed = speed;
	}
}
