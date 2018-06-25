using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlinePulsator : MonoBehaviour {

	public Outline outline;
	private float intensity;
	private float v;

	void Update(){
		if(!outline.enabled) return;
		//outline.effectDistance = Vector2.Lerp(outline.effectDistance, new Vector2(4, -4), 0.2f);
		float c = Mathf.Cos(v / 10) + 1;
		outline.effectDistance = new Vector2((c + 1) / 2, -(c + 1) / 2) * intensity;
		v++;
	}
 
	public void pulsate(float intensity){
		this.intensity = intensity;
		outline.enabled = true;
	}

	public void stopPulsating(){
		this.intensity = 0;
		outline.enabled = false;
	}
}
