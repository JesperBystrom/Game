using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {

	public UI panel;
	public GameObject shopItemReference;
	public Text floatingText;
	public ShopItemData[] items;
	private Entity purchasedObject;
	private ShopItem shopItem;

	void Start () {
		RectTransform shopItemRect = shopItemReference.GetComponent<RectTransform>();
		float shopItemWidth = shopItemRect.sizeDelta.x;
		float shopItemHeight = shopItemRect.sizeDelta.y;
		float xOffset = 15;
		float yOffset = 25;
		short itemsPerColumn = 3;

		for(int i=0;i<items.Length;i++){
			Vector3 pos = new Vector3((i%itemsPerColumn)*(shopItemWidth+xOffset), (i/itemsPerColumn)*-(shopItemHeight+yOffset), 0);
			GameObject o = UI.createInstance3D(shopItemReference, pos);
			ShopItem item = o.GetComponent<ShopItem>();
			item.setData(items[i]);
			item.getData().createModel(o);
			o.SetActive(true);

			/*GameObject o = Instantiate(shopItemReference);
			o.transform.SetParent(shopItemReference.transform.parent);
			o.GetComponent<RectTransform>().anchoredPosition3D = new Vector3((i%itemsPerColumn)*(shopItemWidth+xOffset), (i/itemsPerColumn)*-(shopItemHeight+yOffset), 0);
			o.GetComponent<RectTransform>().rotation = shopItemReference.GetComponent<RectTransform>().rotation;
			o.GetComponent<RectTransform>().localScale = Vector3.one;
			o.GetComponent<ShopItem>().setData(items[i]);
			o.SetActive(true);*/
		}
	}

	void Update(){
		if(purchasedObject == null) return;

		if(Input.GetMouseButtonDown(Mouse.LEFT_CLICK)){

			if(Mouse.mouseInAir()){
				Destroy(purchasedObject.gameObject);
				purchasedObject = null;
				return;
			}

			Territory t = GridMover.instance.getTerritory();

			if(!Game.instance.debug)
				if(t.getPlayer() != Game.instance.getCurrentPlayer()) return;

			if(!t.hasEntity()){
				t.put(purchasedObject, Game.instance.getCurrentPlayer());
				GridMover.instance.detachObject();
				purchasedObject = null;
				Debug.Log(shopItem);
				Game.instance.getCurrentPlayer().removeGold(shopItem.getData().cost);
			}
		}
	}

	public void purchaseItem(ShopItem shopItem){
		purchaseItem(shopItem.getData());
		this.shopItem = shopItem;
	}

	public void purchaseItem(ShopItemData data){
		if(Game.instance.getCurrentPlayer().gold < data.cost) return;
		GameObject o = Instantiate(data.prefab);

		GridMover.instance.attachObject(o);
		purchasedObject = o.GetComponent<Entity>();
		panel.toggle();
	}

	public Entity purchaseItemAI(ShopItemData data, Territory territory){
		if(Game.instance.getCurrentPlayer().gold < data.cost) return null;
		GameObject o = Instantiate(data.prefab);
		Entity e = o.GetComponent<Entity>();
		territory.put(e, Game.instance.getCurrentPlayer());
		Game.instance.getCurrentPlayer().removeGold(data.cost);
		return e;
	}

	public ShopItemData getItem(Item item){
		foreach(ShopItemData d in items){
			if(d.item == item){
				return d;
			}
		}
		return null;
	}
}
