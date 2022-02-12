using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompActivate : MonoBehaviour {

    [SerializeField] private Transform letterPos;
    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private Comp comp;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Transform forcePos1;
    [SerializeField] private Transform forcePos2;

    [Header("States")]
    [SerializeField] private bool isActivated = false;

    [Header("Adjustable Variables")]
    [SerializeField] private float activationForce = 3f;

    private PlayerController playerController;
    private Rigidbody2D playerRB;
    private bool addedText = false;
    private GameObject letterGO = null;
    private char activationChar;

    private void Awake() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerRB = playerController.GetComponent<Comp>().ragdollRB;
        if (particles != null) {
            particles.Stop();
        }
    }

    private void Update() {
        if (!comp.isPlaced) { return; }

        if (!addedText) {
            addedText = true;
            letterGO = Instantiate(letterPrefab, letterPos.position, Quaternion.Euler(0, 0, 0));
            activationChar = GameScene.GetRandomLetter();
            letterGO.GetComponent<TextMesh>().text = activationChar.ToString().ToUpper();
            letterGO.GetComponent<DestroyLetter>().attachedGameObject = gameObject;
        }

        if (letterGO != null) {
            letterGO.transform.position = letterPos.position;
        }

        if (GameScene.stateFly && playerController.isFlying) {

            if (Input.GetKey(activationChar.ToString())) {
                isActivated = true;
                if (particles != null && !particles.isPlaying) {
                    particles.Play();
                }
            }
            else {
                isActivated = false;
                if (particles != null && particles.isPlaying) {
                    particles.Stop();
                }
            }

            if (isActivated) {
                Vector2 direction = forcePos1.position - forcePos2.position;
                //comp.ragdollRB.AddForce(direction.normalized * activationForce);
                if (playerRB == null) {
                    isActivated = false;
                    return;
                }
                playerRB.AddForce(direction.normalized * activationForce);
            }
        }
        else {
            isActivated = false;
            if (particles != null && particles.isPlaying) {
                particles.Stop();
            }
        }
    }
}
