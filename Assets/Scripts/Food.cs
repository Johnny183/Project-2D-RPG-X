using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : InteractableObject {

	void Start () {
		canvas.gameObject.SetActive(true);
		text.gameObject.SetActive(false);
	}

	void Update(){
		UpdateUI();
	}
}
