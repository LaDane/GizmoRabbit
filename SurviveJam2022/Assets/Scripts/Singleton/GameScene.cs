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


    public static void ResetStates() {
        stateFly = false;
        statePlaceComponent = false;
    }
}
