using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoveScript : MonoBehaviour {
	[Range(0,360)] public float direction;
	public float speed;

	private RectTransform rect;
	private float x;
	private float y;

	void Start(){
		rect = GetComponent<RectTransform>();

		x = transform.position.x;
		y = transform.position.y;
	}

	void Update(){
		float cs = Mathf.Cos(direction * Mathf.Deg2Rad);
		float sn = Mathf.Sin(direction * Mathf.Deg2Rad);

		x += cs;
		y += sn;

		transform.position = Camera.main.WorldToScreenPoint(new Vector3(x, y, 0)) * speed * Time.deltaTime;

		//transform.position = Camera.main.WorldToScreenPoint(transform.position);

		//new Vector3(v.x, v.y+((rect.sizeDelta.y+48)), v.z);
	}
}
