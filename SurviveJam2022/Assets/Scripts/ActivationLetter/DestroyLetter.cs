using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLetter : MonoBehaviour {

    [HideInInspector] public GameObject attachedGameObject;
    [HideInInspector] public GameObject playerGO;

    private void Update() {
        if (attachedGameObject == null || playerGO.tag.Equals("PlayerOld")) {
            Destroy(gameObject);
        }
    }
}
