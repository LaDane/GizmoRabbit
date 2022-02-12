using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLetter : MonoBehaviour {

    [HideInInspector] public GameObject attachedGameObject;

    private void Update() {
        if (attachedGameObject == null) {
            Destroy(gameObject);
        }
    }
}
