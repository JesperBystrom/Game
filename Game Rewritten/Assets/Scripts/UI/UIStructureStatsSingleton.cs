using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStructureStatsSingleton : MonoBehaviour {

	public static UIStructureStatsSingleton instance;
	[HideInInspector] public Image image;

	void Start () {
		instance = this;
		image = GetComponent<Image>();
	}
}
