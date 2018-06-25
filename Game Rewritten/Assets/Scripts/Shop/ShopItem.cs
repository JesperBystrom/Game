using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {

	public Text price;
	public UITextBox toolTip;
	private ShopItemData data;
	public Rotator rotator;

	private void Start(){
		price.text = data.cost.ToString();
		rotator = transform.GetChild(1).GetComponent<Rotator>();
		rotator.enabled = false;
	}

	private void Update(){
		if(toolTip == null) return;
		if(toolTip.gameObject.activeInHierarchy){
			//toolTip.transform.position = new Vector3(transform.position.x + 0.9f, transform.position.y + 1.1f, transform.position.z);
			toolTip.setText("Chicken Assasin - Fast, small and nimble. Use this to assasinate your foes!");
		}
	}

	public void setData(ShopItemData data){
		this.data = data;
	}

	public ShopItemData getData(){
		return data;
	}

	public void onEnter(UITextBox toolTip){
		rotator.enabled = true;
		this.toolTip = toolTip;
		toolTip.gameObject.SetActive(true);
	}

	public void onExit(){
		rotator.enabled = false;
		toolTip.gameObject.SetActive(false);
		toolTip = null;
	}
}
