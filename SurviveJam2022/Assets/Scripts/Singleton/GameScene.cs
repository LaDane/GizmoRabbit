using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoSingleton<GameScene> {

    // Game States
    public static bool stateMainMenu = true;
    public static bool stateFly = false;
    public static bool statePlaceComponent = false;
    public static bool stateLootCrate = false;
    public static void ResetStates() {
        stateFly = false;
        statePlaceComponent = false;
        stateLootCrate = false;
    }
    public static void EnterStateFly() {
        ResetStates();
        stateFly = true;
        playerObjectClone = Instantiate(playerObject, playerObject.transform.position, playerObject.transform.rotation);
        playerObjectClone.tag = "PlayerClone";
        playerObjectClone.SetActive(false);
    }
    public static void EnterStatePlaceComponent() {
        ResetStates();
        statePlaceComponent = true;
    }
    public static void EnterStateLootCrate() {
        ResetStates();
        stateLootCrate = true;
        playerObject.tag = "PlayerOld";
        //Destroy(playerObject);
        playerObject = playerObjectClone;
        playerObject.tag = "Player";
        playerObject.SetActive(true);
    }
    public static void EnterStateMainMenu() {
        GameObject.FindGameObjectWithTag("Player").transform.position = playerOriginalPos;
        ResetStates();
        stateMainMenu = true;
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

    // Highscore
    public static float distanceHighscore = 0f;


    private void Awake() {
        FillAvailableChars();
    }

    private void Start() {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerOriginalPos = playerObject.transform.position;
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
