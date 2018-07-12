using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Shop {
	UNIT, STRUCTURE
}

public class ShopCollection : MonoBehaviour {

	public ShopManager[] shops;
	public static ShopCollection instance;

	private void Start(){
		instance = this;
	}

	public ShopManager getShop(Shop shop){
		foreach(ShopManager s in shops){
			if(s.shopType == shop){
				return s;
			}
		}
		return null;
	}
}
