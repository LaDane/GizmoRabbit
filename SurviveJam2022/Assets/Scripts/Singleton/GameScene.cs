using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoSingleton<GameScene> {

    // Game States
    public static bool stateMainMenu = true;
    public static bool stateFly = false;
    public static bool statePlaceComponent = false;
    public static bool stateLootCrate = false;
    public static string stateCurrent = "Menu";
    public static void ResetStates() {
        stateFly = false;
        statePlaceComponent = false;
        stateLootCrate = false;
    }
    public static void EnterStateFly() {
        ResetStates();
        stateFly = true;
        stateCurrent = "Fly";
        playerObjectClone = Instantiate(playerObject, playerObject.transform.position, playerObject.transform.rotation);
        playerObjectClone.tag = "PlayerClone";
        playerObjectClone.SetActive(false);
    }
    public static void EnterStatePlaceComponent() {
        ResetStates();
        statePlaceComponent = true;
        stateCurrent = "PlaceComponent";
        timesPlayed++;
    }
    public static void EnterStateLootCrate() {
        ResetStates();
        stateLootCrate = true;
        stateCurrent = "LootCrate";
        playerObject.tag = "PlayerOld";
        //Destroy(playerObject);
        playerObject = playerObjectClone;
        playerObject.tag = "Player";
        playerObject.SetActive(true);
    }
    public static void EnterStateMainMenu() {
        componentsPlaced = 0;
        timesPlayed = 0;
        hasCompThatCanBeActivated = false;

        GameObject.FindGameObjectWithTag("Player").transform.position = playerOriginalPos;
        ResetStates();
        stateMainMenu = true;
        stateCurrent = "Menu";
    }

    // Selected Component
    public static GameObject selectedCompGO = null;
    public static float selectedCompRot;

    // Mouse Follow Component
    public static GameObject mouseFollowGO = null;
    public static Comp mouseFollowComp = null;
    public static bool canPlaceComponent = true;

    // Activate Component
    private static char[] alphabet = "bcefghijklmnopqrstuvxyz".ToCharArray();        // no w
    private static List<char> availableChars = new List<char>();

    // Player clone
    public static GameObject playerObject;
    public static GameObject playerObjectClone;
    private static Vector3 playerOriginalPos;

    // stats
    public static float distanceHighscore = 0f;
    public static int componentsPlaced = 0;
    public static int timesPlayed = 0;
    public static bool hasCompThatCanBeActivated = false;
    public static char compThatCanBeActivatedChar = 'a';


    private void Awake() {
        FillAvailableChars();
    }

    private void Start() {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        //playerOriginalPos = playerObject.transform.position;
        playerOriginalPos = new Vector3(-0.05f, -1.26f, 0f);
    }

    public static void ResetPlayerPosition() {
        playerObject.transform.position = playerOriginalPos;
    }

    private static void FillAvailableChars() {
        foreach (char c in alphabet) {
            availableChars.Add(c);
        }
    }

    public static char GetRandomLetter() {
        if (availableChars.Count <= 1) {
            FillAvailableChars();
        }

        int index = Random.Range(0, availableChars.Count);
        char indexChar = availableChars[index];
        availableChars.RemoveAt(index);
        return indexChar;
    }
}
