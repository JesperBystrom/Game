using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLight : MonoBehaviour {

	public Light light;
	public float interval;
	private float range;

	// Use this for initialization
	void Start () {
		range = light.range;
	}
	
	// Update is called once per frame
	void Update () {
		light.range = Random.Range(range-interval, range+interval);
	}
}
