using UnityEngine;
using UnityEngine.UI;

public class EquipmentUISlot : MonoBehaviour {
	public EquipmentSlot equipSlot;
	public Image icon;

	private Item item;

	public void Equip(Equipment newItem){
		item = newItem;
		icon.sprite = item.icon;
		icon.enabled = true;
	}

	public void Unequip(){
		item = null;
		icon.sprite = null;
		icon.enabled = false;
		EquipmentManager.instance.Unequip((int)equipSlot);
	}
}
