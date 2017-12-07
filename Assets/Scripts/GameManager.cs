using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour {

	//Static instance of GameManager which allows it to be accessed by any other script.
	public static GameManager instance = null;

	[HideInInspector] public string gameState = "Menu";

	[Header("Game Manager Options")]
	public bool presetManagerValues;
	public bool resetDefaults;

	[Header("Game Options")]
	public int currentGameLevel;
	public float aiRepathRate;
	public List<Equipment> gameEquipment;
	public List<Item> gameItems;

	[Header("Player Options")]
	public int playerStartingHealth;
	public int playerStartingDamage;
	public int playerCoins;
	public int playerLevel;
	public int playerExp;
	public float playerIneractDist;

	[Header("Audio Options")]
	public float gameVolume;
	public float musicVolume;

	private FadeScenes fadeScenes;

	void Awake(){
		//Check if instance already exists
        if (instance == null)
        {
            instance = this;
        } else if (instance != this) //If instance already exists and it's not this:
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
	}

	void Start(){
		// Load our game, if not, setup our defaults
		if(presetManagerValues){
			int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
			EquipmentManager.instance.currentEquipment = new Equipment[numSlots];
			InventoryManager.instance.items = new List<Item>();
			SaveGame();
		} else {
			if(!LoadGame() || resetDefaults){
				SetupDefaults();
			}
		}

		SceneManager.sceneLoaded += OnSceneLoaded;
		fadeScenes = GetComponent<FadeScenes>();

		// Load our persistent scenes on top of our level scene
		SceneManager.LoadScene("PlayerQMenu", LoadSceneMode.Additive);

		StartCoroutine(ScanGraph());
		LoadGameScene("MainMenu");
	}

	//Setup default variables
	public void SetupDefaults(){
		Debug.Log("Setting up defaults");
		currentGameLevel = 1;
		playerStartingHealth = 100;
		playerStartingDamage = 10;
		playerCoins = 0;
		playerLevel = 1;
		playerExp = 0;
		playerIneractDist = 2f;

		int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
		EquipmentManager.instance.currentEquipment = new Equipment[numSlots];
		InventoryManager.instance.items = new List<Item>();

		SaveGame();
	}

	// A* PathFinding update graph
	private IEnumerator ScanGraph(){
		while(true){
			yield return new WaitForSeconds(aiRepathRate);
			if(AstarPath.active){
				AstarPath.active.Scan(AstarPath.active.data.gridGraph);
			}
		}
	}

	public void LoadGameScene(string sceneName){
		if(sceneName == "MainMenu"){
			gameState = "Menu";
		} else {
			gameState = "Game";
		}

		Time.timeScale = 1;
		StartCoroutine(LoadScene(sceneName));
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode){
		Debug.Log("Loading Complete...");
		if(gameState == "Game"){
			EquipmentManager.instance.LoadPlayerEquipment();
		}
	}

	private IEnumerator LoadScene(string sceneName){
		Cursor.visible = false;
		Debug.Log("Preparing Level..." + sceneName);
		fadeScenes.BeginFade(1);
		yield return new WaitForSeconds(fadeScenes.fadeSpeed);

		// Load game scene as a single entity
		Debug.Log("Loading Level..." + sceneName);
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
		fadeScenes.alpha = 1;
		fadeScenes.BeginFade(-1);
	}

	// Save current game data to binary encoded file
	public void SaveGame(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gameData.dat");
		GameData data = new GameData();

		// Save generic game data
		data.currentGameLevel = currentGameLevel;
		data.playerStartingHealth = playerStartingHealth;
		data.playerStartingDamage = playerStartingDamage;
		data.playerCoins = playerCoins;
		data.playerLevel = playerLevel;
		data.playerExp = playerExp;

		// Save equipment names in string list for storing (Can't store custom scriptable objects)
		for(int i = 0; i < EquipmentManager.instance.currentEquipment.Length; i++){
			if(EquipmentManager.instance.currentEquipment[i] != null){
				data.currentEquipment.Add(EquipmentManager.instance.currentEquipment[i].name);
			} else {
				data.currentEquipment.Add(null);
			}
		}
		
		// Save inventory names in string list for storing (Can't store custom scriptable objects)
		for(int i = 0; i < InventoryManager.instance.items.Count; i++){
			data.inventory.Add(InventoryManager.instance.items[i].name);
		}

		// Serialize our file with the GameData class and close the file
		bf.Serialize(file, data);
		file.Close();
		Debug.Log("Game saved to: " + Application.persistentDataPath + "/gameData.dat successfully");
	}

	// Load game data from save file
	public bool LoadGame(){
		if(File.Exists(Application.persistentDataPath + "/gameData.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gameData.dat", FileMode.Open);
			GameData data = (GameData)bf.Deserialize(file);
			file.Close();

			// Load generic game data
			currentGameLevel = data.currentGameLevel;
			playerStartingHealth = data.playerStartingHealth;
			playerStartingDamage = data.playerStartingDamage;
			playerCoins = data.playerCoins;
			playerLevel = data.playerLevel;
			playerExp = data.playerExp;

			// Load player equipment
			for(int i = 0; i < data.currentEquipment.Count; i++){
				for(int y = 0; y < gameEquipment.Count; y++){
					if(gameEquipment[y].name == data.currentEquipment[i]){
						EquipmentManager.instance.AddToEquipmentIndex(gameEquipment[y], i);
						break;
					}
				}
			}

			// Load player inventory
			for(int i = 0; i < data.inventory.Count; i++){
				for(int y = 0; y < gameItems.Count; y++){
					if(gameItems[y].name == data.inventory[i]){
						InventoryManager.instance.AddToInventoryIndex(gameItems[y]);
						break;
					}
				}

				for(int y = 0; y < gameEquipment.Count; y++){
					if(gameEquipment[y].name == data.inventory[i]){
						InventoryManager.instance.AddToInventoryIndex(gameEquipment[y]);
						break;
					}
				}
			}

			Debug.Log("Game loaded from: " + Application.persistentDataPath + "/gameData.dat successfully");
			return true;
		} else {
			Debug.Log("Failed to find file: " + Application.persistentDataPath + "/gameData.dat");
			return false;
		}
	}

	// Save game when they quit the application
	void OnApplicationQuit(){
		Debug.Log("Application ending after " + Time.time + " seconds");
		SaveGame();
	}
}

// Used to store game data for when writing to our data file
[Serializable]
class GameData
{
	public int currentGameLevel;
	public int playerStartingHealth;
	public int playerStartingDamage;
	public int playerCoins;
	public int playerLevel;
	public int playerExp;
	public List<string> currentEquipment = new List<string>();
	public List<string> inventory = new List<string>();
}
