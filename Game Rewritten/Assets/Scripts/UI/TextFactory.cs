using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TextType {
	LOSS, GAIN, GOLD //damage, heal, cost etc.
}

public class TextFactory {
	private static TextFactory instance = new TextFactory();

	public Text createTextFromValue(GameObject prefab, Vector3 position, int value, TextType type, bool worldSpace){
		GameObject o = UI.createInstance(prefab);
		o.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		o.transform.localPosition = position;
		if(worldSpace){
			o.transform.localPosition = Camera.main.WorldToScreenPoint(position);
		}
		Text t = o.GetComponent<Text>();
		switch(type){
		case TextType.LOSS:
			t.text = "- " + value;
			t.color = Color.red;
			break;
		case TextType.GAIN:
			t.text = "+ " + value;
			t.color = Color.green;
			break;
		case TextType.GOLD:
			t.text = "+ " + value;
			t.color = Color.yellow;
			break;
		}
		return t;
	}

	public void createMovingText(GameObject prefab, Vector3 position, int value, TextType type, bool worldSpace, float speed=1f, float direction=90f){
		Text t = createTextFromValue(prefab, position, value, type, worldSpace);
		UIMover mover = t.GetComponent<UIMover>();
		position += mover.offset;
		mover.move(position, direction, speed, worldSpace);
	}

	public static TextFactory getInstance(){
		return instance;
	}
}
