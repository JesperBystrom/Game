using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
	private Image image;
	private Image[] children;
	protected bool opened = false;
	private CanvasGroup canvasGroup;
	private float alpha = -1;

	void Start(){
		image =  GetComponent<Image>();
		children = GetComponentsInChildren<Image>();
		setVisible(image.enabled);
		this.canvasGroup = transform.root.GetComponent<CanvasGroup>();

		if(canvasGroup != null)
			forceFade(1);
	}

	void Update(){
		if(alpha != -1){
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, alpha, 0.2f);
			if(Mathf.Abs(canvasGroup.alpha - alpha) <= 0.1f){
				alpha = -1;
			}
		}
	}

	public virtual void open(){

	}

	public virtual void close(){

	}

	public virtual void forceOpen(){

	}

	public virtual void toggle(){

	}

	public void setVisible(bool visible){
		image.enabled = visible;
		foreach(Image i in children){
			i.enabled = image.enabled;
		}
	}

	public void fade(float alpha){
		//this.alpha = alpha;
	}

	public void forceFade(float alpha){
		this.alpha = -1;
		this.canvasGroup.alpha = alpha;
	}

	public static GameObject createInstance(GameObject prefab){
		GameObject o = Instantiate(prefab);
		o.transform.SetParent(GameObject.Find("StatsCanvas").transform);
		return o;
	}

	public static GameObject createInstance3D(GameObject prefab, Vector3 position){
		GameObject o = Instantiate(prefab);
		o.transform.SetParent(prefab.transform.parent);
		RectTransform rect = o.GetComponent<RectTransform>();
		rect.anchoredPosition3D = position;//new Vector3((i%itemsPerColumn)*(shopItemWidth+xOffset), (i/itemsPerColumn)*-(shopItemHeight+yOffset), 0);
		rect.rotation = prefab.GetComponent<RectTransform>().rotation;//shopItemReference.GetComponent<RectTransform>().rotation;
		rect.localScale = Vector3.one;
		return o;
	}

	/*public static void createFloatingText(GameObject prefab, Vector3 position, string text, Color color){
		GameObject o = UI.createInstance(prefab);
		o.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		o.GetComponent<UIToGame>().moveRelativeTo(position, 90, 3);
		Text t = o.GetComponent<Text>();
		t.text = text;
		t.color = color;
	}*/
}
