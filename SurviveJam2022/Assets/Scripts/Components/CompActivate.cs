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
    [SerializeField] private Transform energyLevelPos;
    [SerializeField] private GameObject energyLevelPrefab;
    private float energyValue = 1f;
    private Transform energyLevelsIndicatorParent;
    private GameObject energyLevelBar;
    private Slider energySlider;

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
        energyLevelsIndicatorParent = GameObject.FindGameObjectWithTag("EnergyLevels").transform;
        energyLevelBar = Instantiate(energyLevelPrefab, energyLevelsIndicatorParent);
        energySlider = energyLevelBar.GetComponentInChildren<Slider>();
        energyLevelBar.SetActive(false);
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
            //letterGO.GetComponent<TextMesh>().text = activationChar.ToString().ToUpper();
            letterGO.GetComponent<DestroyLetter>().playerGO = GameObject.FindGameObjectWithTag("Player").gameObject;
            letterGO.GetComponent<DestroyLetter>().attachedGameObject = gameObject;
        }

        if (letterGO != null) {
            letterGO.transform.position = letterPos.position;
        }

        if (GameScene.stateFly) {
            if (!energyLevelBar.activeInHierarchy) {
                energyLevelBar.SetActive(true);
            }
            energyLevelBar.transform.position = energyLevelPos.position;
            energyLevelBar.transform.rotation = transform.rotation;

        }
        else if (!GameScene.stateFly && energyLevelBar.activeInHierarchy) {
            energyLevelBar.SetActive(false);
        }

        if (GameScene.stateFly && playerController.isFlying) {
            Debug.Log(energyValue);
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
                energySlider.value = energyValue;
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
