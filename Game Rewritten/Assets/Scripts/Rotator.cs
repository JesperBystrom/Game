using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public GameObject pivot;
	public float xSpeed;
	public float ySpeed;
	public float zSpeed;

	void Start () {
		if(pivot == null)
			pivot = gameObject;
	}

	void Update () {
		pivot.transform.Rotate(new Vector3(xSpeed, ySpeed, zSpeed));
	}

	public void rotate(float xSpeed, float ySpeed, float zSpeed){
		this.xSpeed = xSpeed;
		this.ySpeed = ySpeed;
		this.zSpeed = zSpeed;
	}
}
