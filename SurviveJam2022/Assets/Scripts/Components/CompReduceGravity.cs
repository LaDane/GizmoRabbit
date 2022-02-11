using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompReduceGravity : MonoBehaviour {

    [SerializeField] private Comp comp;

    [Header("Adjustable Variables")]
    [SerializeField] private float gravityReduction = 0.1f;

    private Comp playerComp;
    private bool changedGravity = false;

    private void Awake() {
        playerComp = GameObject.FindGameObjectWithTag("Player").GetComponent<Comp>();
    }

    private void Update() {
        if (changedGravity) { return; }
        if (comp.isPlaced) {
            changedGravity = true;
            if (playerComp.ragdollRB.gravityScale > .5f) {
                playerComp.ragdollRB.gravityScale = playerComp.ragdollRB.gravityScale - gravityReduction;
            }
        }
    }
}
