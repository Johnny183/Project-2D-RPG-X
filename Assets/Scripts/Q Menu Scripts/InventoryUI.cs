using UnityEngine;

public class InventoryUI : MonoBehaviour {

	public Transform itemsParent;

	private InventoryManager inventoryManager;
	private InventorySlot[] slots;

	// Use this for initialization
	void Awake () {
		inventoryManager = InventoryManager.instance;
		inventoryManager.onItemChangedCallback += UpdateUI;

		slots = itemsParent.GetComponentsInChildren<InventorySlot>();
		UpdateUI();
	}

	public void UpdateUI(){
		for(int i = 0; i < slots.Length; i++){
			if(i < inventoryManager.items.Count){
				slots[i].AddItem(inventoryManager.items[i]);
			} else {
				slots[i].ClearSlot();
			}
		}
	}
}
