using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldTextSingleton : MonoBehaviour {

	public static GoldTextSingleton instance;
	public Text text;
	public RectTransform rect;
	public GameObject floatingText;
	public TextPunchEffect textFade;

	void Start () {
		instance = this;
		rect = GetComponent<RectTransform>();
	}

	public void onGoldLose(int amount){
		TextFactory.getInstance().createMovingText(floatingText.gameObject, new Vector3(transform.position.x - 80, Screen.height - 32), amount, TextType.LOSS, false, 0.4f, 90);
		textFade.fade();
	}
}
