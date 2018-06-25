using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChildrenDisabler : MonoBehaviour {

	public Image image;
	private Image[] children;

	void Start () {
		children = GetComponentsInChildren<Image>();
	}

	public void setVisible(bool visible){
		image.enabled = visible;
		foreach(Image i in children){
			i.enabled = image.enabled;
		}
	}
}
