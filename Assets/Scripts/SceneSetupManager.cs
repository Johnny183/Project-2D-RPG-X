using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneSetupManager : MonoBehaviour {
	public Transform environment;
	public GameObject[] floors;
	public GameObject[] walls;
	public GameObject[] enemies;
	public GameObject[] randomItems;

	// Called while scene is loading since gameobject is already in scene
	void Awake () {
		ReplaceWallTiles();
		ReplaceFloorTiles();
		ReplaceEnemies();
		SetupPlayer();
	}

	void Start(){
		Destroy(gameObject);
	}

	// Replace generic floor tiles prefab with random floor tiles carrying array of floor tiles to replace with
	private void ReplaceFloorTiles(){
		if(floors.Length == 0){ SceneSetupError(); return; }

		GameObject[] objects = GameObject.FindGameObjectsWithTag("GenericFloor");
		for(int i = 0; i < objects.Count(); i++){
			int rand = Random.Range(0, floors.Length);
			var newPrefab = Instantiate(floors[rand], objects[i].transform.position, Quaternion.identity);
			if(objects[i].transform.parent == null){
				newPrefab.transform.SetParent(environment);
			} else {
				newPrefab.transform.SetParent(objects[i].transform.parent);
			}

			Destroy(objects[i].gameObject);
		}
	}

	// Replace generic wall tiles prefab with random wall tiles carrying array of walltiles to replace with
	private void ReplaceWallTiles(){
		if(walls.Length == 0){ SceneSetupError(); return; }

		GameObject[] objects = GameObject.FindGameObjectsWithTag("GenericWall");
		for(int i = 0; i < objects.Count(); i++){
			int rand = Random.Range(0, walls.Length);
			var newPrefab = Instantiate(walls[rand], objects[i].transform.position, Quaternion.identity);
			if(objects[i].transform.parent == null){
				newPrefab.transform.SetParent(environment);
			} else {
				newPrefab.transform.SetParent(objects[i].transform.parent);
			}

			Destroy(objects[i].gameObject);
		}
	}

	// Replace random items generic prefab with a random item
	private void SpawnRandomItems(){
		if(randomItems.Length == 0) return;

		GameObject[] objects = GameObject.FindGameObjectsWithTag("GenericItem");
		for(int i = 0; i < objects.Count(); i++){
			int rand = Random.Range(0, walls.Length);
			var newPrefab = Instantiate(randomItems[rand], objects[i].transform.position, Quaternion.identity);
			if(objects[i].transform.parent == null){
				newPrefab.transform.SetParent(environment);
			} else {
				newPrefab.transform.SetParent(objects[i].transform.parent);
			}

			Destroy(objects[i].gameObject);
		}
	}

	// Replace generic enemies prefab with random enemies carrying properties
	private void ReplaceEnemies(){
		
	}

	// Setup player sets player specific preferences which we take from our gamemanager script like weapons, armor, etc
	private void SetupPlayer(){
		
	}

	private void SceneSetupError(){
		Debug.Log("[SCENE SETUP] Error setting up scene! Make sure all properties are filled.");
		Application.Quit();
	}
}
