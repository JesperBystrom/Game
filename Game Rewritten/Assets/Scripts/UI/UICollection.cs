using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICollection : MonoBehaviour {

	public UI unitStatsUIPrefab;
	public UI structureStatsUIPrefab;
	public GameObject abilityUIPrefab;
	public GameObject damageTextPrefab;

	[HideInInspector] public UI unitStatsUIInstance;
	[HideInInspector] public UI structureStatsUIInstance;

	public GameObject createInstance(GameObject prefab){
		GameObject o = Instantiate(prefab);
		o.transform.SetParent(GameObject.Find(prefab.transform.parent.name).transform);
		return o;
	}
}
