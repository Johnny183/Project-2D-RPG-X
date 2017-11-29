using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {
	public static EquipmentManager instance = null;

	public delegate void OnEquipmentChanged();
	public OnEquipmentChanged onEquipmentChanged;

	public Equipment[] currentEquipment;

	void Awake(){
		//Check if instance already exists
        if (instance == null)
        {
            instance = this;
        } else if (instance != this) //If instance already exists and it's not this:
        {
            Destroy(gameObject);
        }

		int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
		currentEquipment = new Equipment[numSlots];
	}

	public void LoadPlayerEquipment(){
		GameObject[] delEquipment = GameObject.FindGameObjectsWithTag("Equipment");
		for(int i = 0; i < delEquipment.Length; i++){
			Destroy(delEquipment[i]);
		}

		Transform parent = GameObject.FindGameObjectWithTag("EquipmentSlot").transform;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				if(currentEquipment[i].refObject != null){
					Instantiate(currentEquipment[i].refObject);
				}
			}
		}
	}

	public void AddToEquipmentIndex(Equipment item, int index){
		currentEquipment[index] = item;
	}

	public void Equip(Equipment newItem){
		int slotIndex = (int)newItem.equipSlot;
		Equipment oldItem = null;

		if(currentEquipment[slotIndex] != null){
			oldItem = currentEquipment[slotIndex];
			InventoryManager.instance.Add(oldItem);
		}

		if(onEquipmentChanged != null){
			onEquipmentChanged.Invoke();
		}

		Transform equipSlot = GameObject.Find("EquipmentParent").transform.GetChild(slotIndex);
		equipSlot.GetComponent<EquipmentUISlot>().Equip(newItem);
		currentEquipment[slotIndex] = newItem;
		LoadPlayerEquipment();
	}

	public void Unequip(int slotIndex){
		if(currentEquipment[slotIndex] != null){
			Equipment oldItem = currentEquipment[slotIndex];
			InventoryManager.instance.Add(oldItem);
			currentEquipment[slotIndex] = null;

			if(onEquipmentChanged != null){
				onEquipmentChanged.Invoke();
			}
			LoadPlayerEquipment();
		}
	}

	public void UnequipAll(){
		for(int i = 0; i < currentEquipment.Length; i++){
			Unequip(i);
		}
	}
}
