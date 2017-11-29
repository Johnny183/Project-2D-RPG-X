using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
[System.Serializable]
public class Item : ScriptableObject {
	new public string name = "New Item";
	public string desc = "Item Description";
	public Sprite icon = null;
	public int sellPrice = 0;
	public int buyPrice = 0;

	public virtual void Use(){
		Debug.Log("Using " + name);
	}

	public void RemoveFromInventory(){
		InventoryManager.instance.Remove(this);
	}
}
