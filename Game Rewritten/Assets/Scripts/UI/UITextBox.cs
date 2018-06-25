using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UITextBox : MonoBehaviour {

	public RectTransform rectTransform;
	public Text textBox;
	public string text;

	private Vector2 size;

	// Use this for initialization
	void Start () {
		this.size = rectTransform.sizeDelta;
	}
	
	// Update is called once per frame
	void Update () {
		textBox.text = text;
		rectTransform.sizeDelta = new Vector2(textBox.text.Length * (size.x/20), size.y);
		textBox.GetComponent<RectTransform>().sizeDelta = rectTransform.sizeDelta;
	}

	public void setText(string text){
		this.text = text;
	}
}
