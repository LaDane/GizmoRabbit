using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Scripts")]
    [SerializeField] private Comp comp;
    [SerializeField] private Comps comps;

    [Header("Layers")]
    [SerializeField] private LayerMask placementLayer;

    [Header("Attached Components")]
    public List<Comp> compList = new List<Comp>();

    [Header("Adjustable Variables")]
    [SerializeField] private float forceSpeed = 50f;


    private void Update() {
        DetectGameStateChange();

        if (GameScene.stateFly) {
            if (Input.GetKey(KeyCode.W)) {
                comp.ragdollRB.AddForce(Vector2.right * forceSpeed * Time.deltaTime);
            }
        }

        if (GameScene.statePlaceComponent) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // Get random component by clicking space
            if (Input.GetKeyDown(KeyCode.Space)) {
                GameScene.selectedCompGO = comps.components[Random.Range(0, comps.components.Count)];
                GameScene.selectedCompRot = Random.Range(-90, 90);
            }

            // Create component to follow mouse cursor
            if (GameScene.selectedCompGO != null && GameScene.mouseFollowGO == null) {
                CreateMouseFollowComp(mousePos);
            }

            // Destroy component that follows mouse once component is placed
            if (GameScene.selectedCompGO == null && GameScene.mouseFollowGO != null) {
                GameScene.mouseFollowComp = null;
                Destroy(GameScene.mouseFollowGO);
            }

            // Update component that follows mouse's position and check if valid placement
            if (GameScene.mouseFollowGO != null) {
                GameScene.mouseFollowGO.transform.position = mousePos;
                CheckPlacementPosition();
            }
        }
    }

    private void CreateMouseFollowComp(Vector3 mousePos) {
        GameScene.mouseFollowGO = Instantiate(GameScene.selectedCompGO, mousePos, Quaternion.Euler(0, 0, GameScene.selectedCompRot));
        GameScene.mouseFollowComp = GameScene.mouseFollowGO.GetComponent<Comp>();
        GameScene.mouseFollowComp.ragdollCollider.enabled = false;
        foreach (GameObject go in GameScene.mouseFollowComp.componentNodes) {
            go.SetActive(false);
        }
    }

    private void CheckPlacementPosition() {
        if (GameScene.mouseFollowGO == null) { return; }
        if (GameScene.mouseFollowComp == null) { return; }

        Collider2D[] allOverlappingColliders = new Collider2D[16];
        List<Collider2D> results = new List<Collider2D>();
        Collider2D col2d = GameScene.mouseFollowComp.placementCollider;

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(placementLayer);
        contactFilter.useLayerMask = true;

        int overlapCount = Physics2D.OverlapCollider(col2d, contactFilter, allOverlappingColliders);
        if (overlapCount == 0) {
            GameScene.canPlaceComponent = true;
        } else {
            GameScene.canPlaceComponent = false;
        }
    }

    private void DetectGameStateChange() {
        if (Input.GetKey(KeyCode.F1)) {
            GameScene.ResetStates();
            GameScene.stateFly = true;
            EnterFlyState();
            Debug.Log("Game State = Fly");
        }
        if (Input.GetKey(KeyCode.F2)) {
            GameScene.ResetStates();
            GameScene.statePlaceComponent = true;
            EnterPlaceComponentState();
            Debug.Log("Game state = Place Component");
        }
    }

    private void EnterFlyState() {
        comp.StateFly();
        foreach (Comp c in compList) {
            c.StateFly();
        }
    }

    private void EnterPlaceComponentState() {
        comp.StatePlaceComponent();
        foreach (Comp c in compList) {
            c.StatePlaceComponent();
        }
    }
}
