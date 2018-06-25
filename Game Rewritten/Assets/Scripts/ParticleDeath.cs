using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDeath : MonoBehaviour {

	public int time;
	private int timeToDeath = 10;
	public ParticleSystem sys;

	void Update () {
		time--;
		if(time <= 0){
			sys.Stop();
			timeToDeath--;
			if(timeToDeath <= 0)
				Destroy(gameObject);
		}
	}
}
