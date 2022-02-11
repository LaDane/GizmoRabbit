using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour {

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Comps comps;

    private List<GameObject> originalList;
    private List<GameObject> grabbaleList;

    private void Awake() {
        originalList = new List<GameObject>(comps.components);
        originalList.Shuffle();

        grabbaleList = new List<GameObject>(originalList);
    }
}
