using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitObject : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other){
		if(other.CompareTag("Player")){
			if(GameObject.FindGameObjectWithTag("Enemy") != null){
				return;
			}

			other.GetComponent<PlayerController>().isPlayerDead = true;
			GameManager.instance.currentGameLevel++;
			GameManager.instance.LoadGameScene("MainMenu");
		}
	}
}
