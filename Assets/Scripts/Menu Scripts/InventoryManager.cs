using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
	public static InventoryManager instance = null;

	void Awake(){
		//Check if instance already exists
        if (instance == null)
        {
            instance = this;
        } else if (instance != this) //If instance already exists and it's not this:
        {
            Destroy(gameObject);
        }
	}

	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;
	public int space = 20;
	public List<Item> items = new List<Item>();

	public void AddToInventoryIndex(Item item){
		items.Add(item);
	}

	public bool Add(Item item){
		if(items.Count >= space){
			return false;
		}

		items.Add(item);
		if(onItemChangedCallback != null) {
			onItemChangedCallback.Invoke();
		}
		return true;
	}

	public void Remove(Item item){
		items.Remove(item);
		if(onItemChangedCallback != null) {
			onItemChangedCallback.Invoke();
		}
	}
}
