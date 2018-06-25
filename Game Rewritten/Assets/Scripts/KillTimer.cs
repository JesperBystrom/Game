using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTimer : MonoBehaviour {

	public float timer;

	void Start () {
		
	}

	void Update () {
		timer--;
		if(timer <= 0){
			Destroy(gameObject);
		}
	}
}
