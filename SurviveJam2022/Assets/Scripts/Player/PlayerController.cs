using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public bool isFlying = false;

    [Header("Scripts")]
    [SerializeField] private Comp comp;
    [SerializeField] private Comps comps;
    [SerializeField] private LootCrate lootCrate;
    [SerializeField] private UIManager uiManager;

    [Header("Layers")]
    [SerializeField] private LayerMask placementLayer;

    [Header("Adjustable Variables")]
    [SerializeField] private float forceSpeed = 10f;
    [SerializeField] private float rotationTorque = 10f;
    [SerializeField] private float maxAngularVelocity = 200f;
    //[SerializeField] private float upForceMultiplier = 100f;
    
    [Header("Distance + Altitude")]
    [SerializeField] private Transform distanceMeasurePoint;
    [SerializeField] private Transform altitudeMeasurePoint;
    //public int distanceFlown = 0;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    private bool audioBellPlayed = false;
    private bool audioFlyPlaying = false;
    private bool audioRidePlaying = false;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private bool animRidingPlaying = false;

    [Header("Attached Components")]
    public List<Comp> compList = new List<Comp>();

    private void Start() {
        //EnterFlyState();
        //GameScene.EnterStateFly();
    }

    private void Update() {
        DetectGameStateChange();
        HandleRotation();

        if (!isFlying && audioFlyPlaying) {
            //audioManager.Stop("Flying");
            audioFlyPlaying = false;
        }

        if (Input.GetKey(KeyCode.W) && !animRidingPlaying) {
            animator.Play("Riding");
        }
        else if (!Input.GetKey(KeyCode.W)) {
            animator.Play("Idle");
        }

        if (Input.GetKey(KeyCode.W) && !audioRidePlaying) {
            audioRidePlaying = true;
            audioManager.Play("BikeRide");
        }
        else if (!Input.GetKey(KeyCode.W)) {
            audioRidePlaying = false;
            audioManager.Stop("BikeRide");
        }

        if (GameScene.stateFly) {

            ClampAngularVelocity();
            if (!audioBellPlayed) {
                audioBellPlayed = true;
                audioManager.Play("BikeBell");
            }

            // Move player forward when grounded
            if (!isFlying && Input.GetKey(KeyCode.W)) {
                Vector2 southEast = transform.right - transform.up;
                comp.ragdollRB.AddForce(southEast * forceSpeed * 100 * Time.deltaTime);
            }

            if (isFlying) {
                MeasureDistanceAltitude();
                if (!audioFlyPlaying) {
                    audioFlyPlaying = true;
                    audioManager.Play("Flying");
                }
            }
        }

        if (GameScene.statePlaceComponent) {
            audioBellPlayed = false;
            isFlying = false;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // Create component to follow mouse cursor
            if (GameScene.selectedCompGO != null && GameScene.mouseFollowGO == null) {
                CreateMouseFollowComp(mousePos);
            }

            // Update component that follows mouse's position and check if valid placement
            if (GameScene.mouseFollowGO != null) {
                GameScene.mouseFollowGO.transform.position = mousePos;
                CheckPlacementPosition();
            }
        }
        // Destroy component that follows mouse once component is placed
        if (GameScene.selectedCompGO == null && GameScene.mouseFollowGO != null) {
            GameScene.mouseFollowComp = null;
            Destroy(GameScene.mouseFollowGO);
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
        if (Input.GetKeyDown(KeyCode.F1)) {
            //GameScene.ResetStates();
            //GameScene.stateFly = true;
            EnterFlyState();
            GameScene.EnterStateFly();
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            GameScene.ResetStates();
            GameScene.statePlaceComponent = true;
            EnterPlaceComponentState();
        }
    }

    public void EnterFlyState() {
        comp.StateFly();
        foreach (Comp c in compList) {
            c.StateFly();
        }
    }

    public void EnterPlaceComponentState() {
        comp.StatePlaceComponent();
        foreach (Comp c in compList) {
            c.StatePlaceComponent();
        }
    }

    private void HandleRotation() {
        if (Input.GetKey(KeyCode.A)) {
            comp.ragdollRB.AddTorque(rotationTorque * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
            comp.ragdollRB.AddTorque(- rotationTorque * Time.deltaTime);
        }
    }

    private void ClampAngularVelocity() {
        if (comp.ragdollRB.angularVelocity < -maxAngularVelocity) { comp.ragdollRB.angularVelocity = -maxAngularVelocity; }
        if (comp.ragdollRB.angularVelocity > maxAngularVelocity) { comp.ragdollRB.angularVelocity = maxAngularVelocity; }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag.Equals("JumpSound")) {
            audioManager.Play("BikeJump");
            return;
        }

        isFlying = true;
        comp.ragdollRB.AddForce(transform.right * 20f, ForceMode2D.Impulse);
    }

    private void MeasureDistanceAltitude() {
        uiManager.distanceTraveled = (int) Mathf.Abs(transform.position.x - distanceMeasurePoint.position.x);
        uiManager.altitude = (int)Mathf.Abs(transform.position.y - altitudeMeasurePoint.position.y);
    }
}
