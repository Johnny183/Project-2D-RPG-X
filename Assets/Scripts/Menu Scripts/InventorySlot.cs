using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

	public Image icon;
	public Button removeButton;

	private Item item;

	public void AddItem(Item newItem){
		item = newItem;
		if(icon != null){
			icon.enabled = true;
			icon.sprite = item.icon;
		}
		removeButton.interactable = true;
	}

	public void ClearSlot() {
		item = null;
		if(icon != null){
			icon.sprite = null;
			icon.enabled = false;
		}
		removeButton.interactable = false;
	}

	public void OnRemoveButton(){
		InventoryManager.instance.Remove(item);
	}

	public void UseItem(){
		if(item != null){
			item.Use();
		}
	}
}
