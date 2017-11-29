using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerQMenu : MonoBehaviour {
	public Canvas playerQMenuCanvas;
	
	void Awake(){
		DontDestroyOnLoad(playerQMenuCanvas);
		DontDestroyOnLoad(this);
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q) && GameManager.instance.gameState == "Game"){
			if(playerQMenuCanvas.gameObject.activeInHierarchy){
				playerQMenuCanvas.gameObject.SetActive(false);
				Cursor.visible = false;
				Time.timeScale = 1;
			} else {
				playerQMenuCanvas.gameObject.SetActive(true);
				playerQMenuCanvas.GetComponent<EquipmentUI>().UpdateUI();
				playerQMenuCanvas.GetComponent<InventoryUI>().UpdateUI();
				Cursor.visible = true;
				Time.timeScale = 0;
			}
		}
	}
}
