using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Item {
	CHICKEN_ASSASIN, FROG_MAGE, FIRE_TOWER, WINDMILL
}

[CreateAssetMenu(fileName="New ShopItem", menuName="Shop Item")]
public class ShopItemData : ScriptableObject {
	public int cost;
	public Texture2D image;
	public GameObject prefab;
	public GameObject model;
	public Item item;

	public void createModel(GameObject parent){
		GameObject o = Object.Instantiate(model);
		o.transform.SetParent(parent.transform);
		o.transform.localScale = new Vector3(5,5,5);
		o.transform.localPosition = new Vector3(38.4f, -70.2f, -88.7f);
	}
}
