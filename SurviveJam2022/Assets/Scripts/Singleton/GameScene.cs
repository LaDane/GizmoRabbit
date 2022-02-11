using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoSingleton<GameScene> {

    // Game States
    public static bool stateFly = false;
    public static bool statePlaceComponent = true;

    // Selected Component
    public static GameObject selectedCompGO = null;
    public static float selectedCompRot;

    // Mouse Follow Component
    public static GameObject mouseFollowGO = null;
    public static Comp mouseFollowComp = null;
    public static bool canPlaceComponent = true;

    // Activate Component
    private static char[] alphabet = "abcdefghijklmnopqrstuvxyz".ToCharArray();        // no w
    private static List<char> availableChars = new List<char>();

    private void Awake() {
        FillAvailableChars();
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

    public static void ResetStates() {
        stateFly = false;
        statePlaceComponent = false;
    }
}
