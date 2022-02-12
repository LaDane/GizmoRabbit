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

    private void Awake() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update() {
        if (!comp.isPlaced) { return; }

        if (GameScene.stateFly && playerController.isFlying) {
            Vector2 direction = forcePos1.position - forcePos2.position;
            comp.ragdollRB.AddForce(direction.normalized * activationForce);
        }

    }
}
