using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour {

    [SerializeField] PlayerController playerController;

    private bool ragdollActivated = false;

    private void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update() {
        if (GameScene.stateLootCrate && !ragdollActivated) {
            ragdollActivated = true;

            if (playerController.compList.Count != 0) {
                foreach (Comp c in playerController.compList) {
                    c.joint.enabled = false;
                }
            }
        }
        if (GameScene.statePlaceComponent) {
            ragdollActivated = false;
        }
    }
}
