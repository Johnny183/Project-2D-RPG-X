using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

	//Static instance of GameManager which allows it to be accessed by any other script.
	public static GameManager instance = null;

	[Header("Game Manager Options")]
	public bool presetManagerValues;
	public bool resetDefaults;

	[Header("Game Options")]
	public int currentGameLevel;
	public GameObject[] weaponsList;

	[Header("Player Options")]
	public int playerStartingHealth;
	public int playerCoins;
	public int playerLevel;
	public int playerExp;
	public float playerIneractDist;
	public GameObject activeWeapon;

	[Header("Audio Options")]
	public int gameVolume;
	public int musicVolume;

	private WeaponBase weaponBase;
	private AIEnemyBase aiEnemyBase;

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

		// Load our game, if not, setup our defaults
		if(presetManagerValues){
			SaveGame();
		} else {
			if(!LoadGame() || resetDefaults){
				SetupDefaults();
			}
		}
	}

	// For testing, will be removed when we have a menu to load scenes from
	void Start(){
		weaponBase = GetComponent<WeaponBase>();
		aiEnemyBase = GetComponent<AIEnemyBase>();
		LoadGameScene("Level1");
	}

	//Setup default variables
	public void SetupDefaults(){
		Debug.Log("Setting up defaults");
		currentGameLevel = 1;
		playerStartingHealth = 100;
		playerCoins = 0;
		playerLevel = 1;
		playerExp = 0;
		playerIneractDist = 2f;
		SaveGame();
	}

	public void LoadGameScene(string sceneName){
		// Load game scene as a single entity
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

		// Load our base classes
		weaponBase.LoadWeaponBase();
		aiEnemyBase.LoadAIEnemyBase();

		// Load our persistent scenes on top of our level scene
		SceneManager.LoadScene("PlayerQMenu", LoadSceneMode.Additive);
		SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);

	}

	public void LoadTitleScene(){
		// Load menu scene as a single entity
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
		SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
	}

	// Save current game data to binary encoded file
	public void SaveGame(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gameData.dat");

		GameData data = new GameData();
		data.currentGameLevel = currentGameLevel;
		data.playerStartingHealth = playerStartingHealth;
		data.playerCoins = playerCoins;
		data.playerLevel = playerLevel;
		data.playerExp = playerExp;
		data.activeWeaponName = activeWeapon.name;

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

			currentGameLevel = data.currentGameLevel;
			playerStartingHealth = data.playerStartingHealth;
			playerCoins = data.playerCoins;
			playerLevel = data.playerLevel;
			playerExp = data.playerExp;

			// Get our active weapon through its name
			for(int i = 0; i < weaponsList.Length; i++){
				if(weaponsList[i].name == data.activeWeaponName){
					activeWeapon = weaponsList[i];
					break;
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
	public int playerCoins;
	public int playerLevel;
	public int playerExp;
	public string activeWeaponName;
}
