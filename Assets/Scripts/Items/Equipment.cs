using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
[System.Serializable]
public class Equipment : Item {
	public EquipmentSlot equipSlot;
	public GameObject refObject = null;

	public int armorModifier;
	public int damageModifier;

	public override void Use(){
		base.Use();
		RemoveFromInventory();
		EquipmentManager.instance.Equip(this);
	}
}

public enum EquipmentSlot {Head, Chest, Legs, Boots, Weapon}