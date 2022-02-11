using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comp : MonoBehaviour {

    public bool isPlaced = false;

    [Header("Ragdoll")]
    public Rigidbody2D ragdollRB;
    public Collider2D ragdollCollider;
    public Joint2D joint;

    [Header("Placement")]
    public Rigidbody2D placementRB;
    public Collider2D placementCollider;

    [Header("Nodes")]
    public List<GameObject> componentNodes = new List<GameObject>();

    private void Awake() {
        if (GameScene.stateFly) {
            StateFly();
        }
        if (GameScene.statePlaceComponent) {
            StatePlaceComponent();
        }
    }

    public void StateFly() {
        placementRB.simulated = false;
        placementCollider.enabled = false;

        ragdollRB.simulated = true;
        ragdollCollider.enabled = true;

        foreach (GameObject go in componentNodes) {
            go.SetActive(false);
        }
    }

    public void StatePlaceComponent() {
        ragdollRB.simulated = false;
        ragdollCollider.enabled = false;

        placementRB.simulated = true;
        placementCollider.enabled = true;

        foreach (GameObject go in componentNodes) {
            go.SetActive(true);
        }
    }
}
