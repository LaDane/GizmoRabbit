using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompNode : MonoBehaviour {

    [Header("Transforms")]
    [SerializeField] private Transform placementPos;

    [Header("State")]
    [SerializeField] private bool occupied = false;

    [Header("Layers")]
    [SerializeField] private LayerMask placementLayer;

    private SpriteRenderer spriteRenderer;
    private Color transparent = new Color(255, 255, 255);

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transparent.a = 0.42f;
    }

    private void Update() {
        if (occupied) {
            gameObject.SetActive(false);
            return;
        }

        if (GameScene.statePlaceComponent) {
            CheckOverlap();
        }
    }


    private void OnMouseOver() {
        if (GameScene.selectedCompGO == null) { return; }
        if (!GameScene.canPlaceComponent) {
            spriteRenderer.color = Color.red;
            return; 
        } else {
            spriteRenderer.color = Color.white;
        }

        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(PlaceComponent());
        }
    }

    private void OnMouseExit() {
        spriteRenderer.color = transparent;
    }

    private IEnumerator PlaceComponent() {
        yield return new WaitForSeconds(0.5f);
        if (GameScene.canPlaceComponent) {
            GameObject component = Instantiate(GameScene.selectedCompGO, 
                placementPos.position, 
                Quaternion.Euler(0, 0, GameScene.selectedCompRot), 
                transform.parent.parent);

            GameScene.selectedCompGO = null;
            GameObject[] playerTag = GameObject.FindGameObjectsWithTag("Player");
            playerTag[0].GetComponent<PlayerController>().compList.Add(component.GetComponent<Comp>());
            occupied = true;

            component.GetComponent<Comp>().joint.connectedBody = transform.parent.parent.GetComponent<Rigidbody2D>();
        }
    }

    private void CheckOverlap() {
        Collider2D col2d = GetComponent<Collider2D>();
        List<Collider2D> results = new List<Collider2D>();

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(placementLayer);
        contactFilter.useLayerMask = true;

        Physics2D.OverlapCollider(col2d, contactFilter, results);

        foreach (Collider2D col in results) {
            if (col.transform.parent.gameObject == GameScene.mouseFollowGO) {
                continue;
            } else {
                occupied = true;
                gameObject.SetActive(false);
            }
        }
    }
}
