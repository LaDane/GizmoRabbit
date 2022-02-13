using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompPassiveForward : MonoBehaviour {

    [SerializeField] private Comp comp;
    [SerializeField] private Transform forcePos1;
    [SerializeField] private Transform forcePos2;

    [Header("Adjustable Variables")]
    [SerializeField] private float activationForce = 1f;

    private PlayerController playerController;
    private Rigidbody2D playerRB;


    private void Awake() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerRB = playerController.GetComponent<Comp>().ragdollRB;

    }

    private void Update() {
        if (!comp.isPlaced) { return; }
        if (playerController.tag.Equals("PlayerOld")) {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerRB = playerController.GetComponent<Comp>().ragdollRB;
        }

        if (GameScene.stateFly && playerController.isFlying) {
            Vector2 direction = forcePos1.position - forcePos2.position;
            if (playerRB == null) {
                return;
            }
            playerRB.AddForce(direction.normalized * activationForce);
        }

    }
}
