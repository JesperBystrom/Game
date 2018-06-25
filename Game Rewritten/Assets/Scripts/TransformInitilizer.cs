using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformInitilizer : MonoBehaviour {

	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
	public bool randomizeRotation;

	void Start(){
		transform.localPosition = position;
		transform.rotation = rotation;
		transform.localScale = scale;
		if(randomizeRotation){
			transform.Rotate(new Vector3(0, Random.Range(0,360), 0));
		}
	}
}
