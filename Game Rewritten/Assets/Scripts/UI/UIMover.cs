using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : MonoBehaviour {

	public Vector3 start;
	[Range(0,360)] public float direction;
	public float speed;
	public Vector3 offset;
	public Vector3 scaleDecrease;
	private bool scale;
	private bool worldSpace;

	void Start () {
		
	}

	void Update () {
		if(start == null) return;

		float rad = direction * Mathf.Deg2Rad;
		
		start += new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * speed;

		transform.position = start;

		if(scale){
			transform.localScale -= new Vector3(scaleDecrease.x, scaleDecrease.y, scaleDecrease.z);
			if(transform.localScale.x <= 0) Destroy(gameObject);
		}

		if(worldSpace){
			transform.position = Camera.main.WorldToScreenPoint(start);
		}
	}

	public void move(Vector3 start, float direction, float speed, bool worldSpace){
		this.start = start;
		this.direction = direction;
		this.speed = speed;
		this.worldSpace = worldSpace;

		if(scaleDecrease != Vector3.zero){
			this.scaleDecrease = new Vector3(scaleDecrease.x * speed, scaleDecrease.y * speed, scaleDecrease.z);
			StartCoroutine(wait());
		}

		if(worldSpace)
			this.speed /= 20; //speed goes faster in world space because it counts in (units) instead of pixels.
	}

	public IEnumerator wait(){
		yield return new WaitForSeconds(0.2f);
		scale = true;
	}
}
