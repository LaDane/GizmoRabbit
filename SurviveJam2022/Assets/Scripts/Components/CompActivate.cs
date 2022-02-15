using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompActivate : MonoBehaviour {

    [SerializeField] private Transform letterPos;
    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private Comp comp;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Transform forcePos1;
    [SerializeField] private Transform forcePos2;

    [Header("Energy Levels")]
    [SerializeField] private float energyDrain = 0.2f;
    public Transform energyLevelPos;
    [SerializeField] private GameObject energyLevelPrefab;
    public float energyValue = 1f;
    private Transform energyLevelsIndicatorParent;
    private GameObject energyLevelBar;
    //private Slider energySlider;

    [Header("States")]
    public bool isActivated = false;

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

    private void Start() {
        if (!comp.isPlaced) { return; }
        energyLevelsIndicatorParent = GameObject.FindGameObjectWithTag("EnergyLevels").transform;
        energyLevelBar = Instantiate(energyLevelPrefab, new Vector3(-100, -100, -100), Quaternion.identity, energyLevelsIndicatorParent);
        energyLevelBar.GetComponent<EnergyLevel>().compActivate = this;
        energyValue = 1f;
    }

    private void Update() {
        if (!comp.isPlaced) { return; }

        if (playerController.tag.Equals("PlayerOld")) {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerRB = playerController.GetComponent<Comp>().ragdollRB;
        }

        if (!addedText) {
            addedText = true;
            letterGO = Instantiate(letterPrefab, letterPos.position, Quaternion.Euler(0, 0, 0));
            activationChar = GameScene.GetRandomLetter();
            letterGO.GetComponent<GetLetterSprite>().SetActivationLetter(activationChar);
            letterGO.GetComponent<DestroyLetter>().playerGO = GameObject.FindGameObjectWithTag("Player").gameObject;
            letterGO.GetComponent<DestroyLetter>().attachedGameObject = gameObject;
        }

        if (letterGO != null) {
            letterGO.transform.position = letterPos.position;
        }

        if (GameScene.stateFly && playerController.isFlying) {
            if (Input.GetKey(activationChar.ToString()) && energyValue >= 0.01f) {
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

                energyValue = energyValue - (energyDrain * Time.deltaTime);
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
