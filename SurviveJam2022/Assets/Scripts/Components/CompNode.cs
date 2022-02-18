using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompNode : MonoBehaviour {

    [Header("Transforms")]
    public Transform placementPos;

    [Header("State")]
    [SerializeField] private bool occupied = false;

    [Header("Layers")]
    [SerializeField] private LayerMask placementLayer;

    [Header("Sprite Renderer")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hoverColor;

    private Color transparent = new Color(255, 255, 255);
    private Color originalColor;

    private Comp playerComp;

    private void Awake() {
        playerComp = GameObject.FindGameObjectWithTag("Player").GetComponent<Comp>();
    }

    private void Start() {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        transparent.a = 0.42f;
    }

    private void Update() {
        if (!gameObject.activeInHierarchy) { return; }
        if (occupied) {
            gameObject.SetActive(false);
            return;
        }
    }


    private void OnMouseOver() {
        if (GameScene.selectedCompGO == null) { return; }
        //spriteRenderer.color = Color.white;
        spriteRenderer.color = hoverColor;

        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(PlaceComponent());
        }
    }

    private void OnMouseExit() {
        //spriteRenderer.color = transparent;
        spriteRenderer.color = originalColor;
    }

    private IEnumerator PlaceComponent() {
        yield return new WaitForSeconds(0.5f);
        GameObject component = Instantiate(GameScene.selectedCompGO, 
            placementPos.position, 
            Quaternion.Euler(0, 0, GameScene.selectedCompRot), 
            playerComp.transform);

        Comp comp = component.GetComponent<Comp>();
        GameScene.selectedCompGO = null;
        GameObject[] playerTag = GameObject.FindGameObjectsWithTag("Player");
        playerTag[0].GetComponent<PlayerController>().compList.Add(comp);
        occupied = true;

        comp.joint.connectedBody = playerComp.ragdollRB;
        comp.isPlaced = true;

        GameScene.EnterStateFly();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().EnterFlyState();

        GameScene.componentsPlaced++;
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
                Comp colComp = col.transform.parent.GetComponent<Comp>();
                if (colComp != null && colComp.isPlaced) {
                    occupied = true;
                    gameObject.SetActive(false);          // COMENT THIS IN AGAIN
                }
            }
        }
    }
}
