using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerQMenu : MonoBehaviour {
	public Canvas playerQMenuCanvas;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q)){
			Debug.Log("Pressed Q");
			if(playerQMenuCanvas.gameObject.activeInHierarchy){
				playerQMenuCanvas.gameObject.SetActive(false);
				Time.timeScale = 1;
			} else {
				playerQMenuCanvas.gameObject.SetActive(true);
				Time.timeScale = 0;
			}
		}
	}
}
