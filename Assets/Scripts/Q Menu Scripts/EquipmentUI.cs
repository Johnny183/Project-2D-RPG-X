using UnityEngine;

public class EquipmentUI : MonoBehaviour {

	public Transform equipmentParent;

	private EquipmentManager equipmentManager;
	private EquipmentUISlot[] slots;

	// Use this for initialization
	void Awake () {
		equipmentManager = EquipmentManager.instance;
		equipmentManager.onEquipmentChanged += UpdateUI;

		slots = equipmentParent.GetComponentsInChildren<EquipmentUISlot>();
		UpdateUI();
	}

	public void UpdateUI() {
		for(int i = 0; i < slots.Length; i++){
			if(EquipmentManager.instance.currentEquipment[i] != null){
				slots[i].Equip(EquipmentManager.instance.currentEquipment[i]);
			} else {
				slots[i].Unequip();
			}
		}
	}
}
