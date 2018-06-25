using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

	void Start () {
		Mesh mesh = new Mesh();
		Vector3[] triangle = {
			new Vector3(-1,-1,0),
			new Vector3(-1,1,0),
			new Vector3(1,1,0)
		};
		mesh.name = "Test Mesh";
		mesh.vertices = triangle;
		mesh.triangles = new int[] { 0,1,2 };
		GetComponent<MeshFilter>().mesh = mesh;
		transform.Rotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)));
	}

	void Update () {
		
	}
}
