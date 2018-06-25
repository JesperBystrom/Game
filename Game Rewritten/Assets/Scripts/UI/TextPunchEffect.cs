using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPunchEffect : MonoBehaviour {

	public Color fadeColor;
	public Vector3 targetScale;
	public float waitTimer;
	public Text text;

	private Color originColor;
	private Vector3 originScale;
	private bool shouldFade;

	// Use this for initialization
	void Start () {
		originColor = text.color;
		originScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(!shouldFade) return;

		text.color = Color.Lerp(text.color, originColor, 0.1f);
		Vector3 curr = new Vector3(text.color.r, text.color.g, text.color.b);
		Vector3 target = new Vector3(originColor.r, originColor.g, originColor.b);

		transform.localScale = Vector3.Lerp(transform.localScale, originScale, 0.1f);

		if(Vector3.Distance(curr, target) < 0.01f){
			shouldFade = false;
			text.color = originColor;
			transform.localScale = originScale;
		}
	}

	public void fade(){
		text.color = fadeColor;
		transform.localScale = targetScale;
		StartCoroutine(wait());
	}

	private IEnumerator wait(){
		yield return new WaitForSeconds(waitTimer);
		shouldFade = true;
	}
}
