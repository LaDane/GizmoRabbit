using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    [HideInInspector] public int distanceTraveled;
    [HideInInspector] public int altitude;

    // Main menu screen
    [Header("Main Menu")]
    [SerializeField] private Transform dapperRabbitSprite;
    [SerializeField] private Transform buttonPlayGame;
    [SerializeField] private Transform buttonHowToPlay;
    [SerializeField] private Transform buttonExit;
    [SerializeField] private float buttonAnimateTime = 1.5f;
    [SerializeField] private float dapperAnimateTime = 4f;
    //private Vector3 mainMenuCameraPos;

    [Header("How to play + Settings")]
    [SerializeField] private Transform howToPlayMenu;
    [SerializeField] private Transform settingsMenu;
    [SerializeField] private float topMenuAnimateTime = 0.8f;
    private Vector3 howToPlayOriginalPos;
    private bool howToPlayActive = false;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFollowPlayer cameraFollowPlayer;
    [SerializeField] private float cameraAnimateTime = 5f;
    private Vector3 cameraOriginalPos;
    private int mainMenuCameraZoom = 10;
    private int playCameraZoom = 5;

    [Header("Game State Tracker")]
    [SerializeField] private Transform gameStateTracker;
    [SerializeField] private Text gameStateTrackerDistance;
    [SerializeField] private Text gameStateTrackerAltitude;

    // Loot box screen
    [Header("Loot Box Screen")]
    [SerializeField] private LootCrate lootCrate;
    [SerializeField] private Transform lootBoxMenu;
    [SerializeField] private float lootBoxMenuEnterTime = 5f;
    [SerializeField] private float lootBoxMenuExitTime = 1f;
    [SerializeField] private Image lootBoxImage;
    [SerializeField] private Button buttonAttach;
    [SerializeField] private Button buttonReroll;
    [SerializeField] private Sprite rerollProcess;
    [SerializeField] private Text textCompName;
    [SerializeField] private Text textCompDescription;
    private bool lootBoxMenuMoving = false;
    private bool searchingForComp = false;
    private float lootBoxMenuStartY = -700;

    // Crash screen
    [Header("Stats Screen")]
    [SerializeField] private Transform statsMenu;
    [SerializeField] private Text statsDistance;
    [SerializeField] private Text statsHighscore;
    [SerializeField] private Transform newRecordImage;
    private bool statsDoneCounting = false;
    private bool displayingNewRecordImage = false;
    private Vector3 newRecordImageOriginalScale;

    [Header("Place Component Screen")]
    //[SerializeField] private GameObject placeComponentHomeButton;
    //[SerializeField] private GameObject componentPlacementGuide;
    [SerializeField] private Transform compPlaceGuide;
    [SerializeField] private Transform homeButton;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Visual Tutorials")]
    [SerializeField] private Transform wForForwardMenu;
    [SerializeField] private Transform adToRotateMenu;
    [SerializeField] private Transform rightMousePanMenu;
    [SerializeField] private Transform compCanBeActivatedMenu;
    [SerializeField] private GetLetterSprite getLetterSprite;
    private bool wForForwardMenuDisplayed = false;
    private bool compCanBeActivatedMenuDisplayed = false;

    private string lastState = "Menu";

    private void Start() {
        howToPlayOriginalPos = howToPlayMenu.localPosition;

        mainCamera.orthographicSize = mainMenuCameraZoom;
        cameraOriginalPos = mainCamera.transform.position;

        newRecordImageOriginalScale = newRecordImage.localScale;
        newRecordImage.localScale = Vector3.zero;

        StartCoroutine(EnterGame());
    }

    private void Update() {
        if (lastState != GameScene.stateCurrent) {
            searchingForComp = false;
            statsDoneCounting = false;
            lootBoxMenuMoving = false;
        }

        if (GameScene.stateFly) {
            if (lastState != GameScene.stateCurrent) {
                if (GameScene.timesPlayed == 0) {
                    wForForwardMenuDisplayed = true;
                    StartCoroutine(AnimateMenuVertical(wForForwardMenu, wForForwardMenu.localPosition.y, wForForwardMenu.localPosition.y - 400, 1f));
                }

                if (GameScene.timesPlayed == 1) {
                    StartCoroutine(AnimateMenuHorizontal(compPlaceGuide, compPlaceGuide.localPosition.x, compPlaceGuide.localPosition.x - 300, 1f));
                }

                if (GameScene.timesPlayed == 2) {
                    StartCoroutine(AnimateMenuVertical(rightMousePanMenu, rightMousePanMenu.localPosition.y, rightMousePanMenu.localPosition.y + 400, 1f));
                }

                StartCoroutine(AnimateMenuVertical(gameStateTracker, gameStateTracker.localPosition.y, gameStateTracker.localPosition.y + 250, 0.5f));

                //if (GameScene.timesPlayed != 0) {
                //    StartCoroutine(AnimateMenuHorizontal(homeButton, homeButton.localPosition.x, homeButton.localPosition.x - 300, 1f));
                //}
            }

            if (GameScene.hasCompThatCanBeActivated && !compCanBeActivatedMenuDisplayed) {
                compCanBeActivatedMenuDisplayed = true;
                StartCoroutine(AnimateCompCanBeActivatedVisualTutorial());
            }

            if (GameScene.timesPlayed == 0 && Input.GetKey(KeyCode.W) && wForForwardMenuDisplayed && wForForwardMenu.localPosition.y == 50) {
                wForForwardMenuDisplayed = false;
                StartCoroutine(AnimateRotationVisualTutorial());
            }

            gameStateTrackerDistance.text = distanceTraveled + " METERS";
            gameStateTrackerAltitude.text = altitude + " METERS";
        }

        else if (GameScene.statePlaceComponent) {
            if (lastState != GameScene.stateCurrent) {
                if (GameScene.timesPlayed == 1) {
                    StartCoroutine(AnimateMenuHorizontal(compPlaceGuide, compPlaceGuide.localPosition.x, compPlaceGuide.localPosition.x + 300, 1f));
                }
                if (GameScene.timesPlayed == 2) {
                    StartCoroutine(AnimateMenuVertical(rightMousePanMenu, rightMousePanMenu.localPosition.y, rightMousePanMenu.localPosition.y - 400, 1f));
                }
                StartCoroutine(AnimateMenuHorizontal(homeButton, homeButton.localPosition.x, homeButton.localPosition.x + 300, 1f));
            }
        }

        else if (GameScene.stateLootCrate) {
            if (lastState != GameScene.stateCurrent) {
                if (!searchingForComp) {
                    searchingForComp = true;
                    StartCoroutine(lootCrate.GetNextComponent());
                    StartCoroutine(AnimateMenuVertical(statsMenu, statsMenu.localPosition.y, statsMenu.localPosition.y + 700, 0.5f));
                    StartCoroutine(AnimateStatsCounter());
                }
                StartCoroutine(AnimateMenuVertical(gameStateTracker, gameStateTracker.localPosition.y, gameStateTracker.localPosition.y - 250, 0.5f));
                if (distanceTraveled > GameScene.distanceHighscore) {
                    GameScene.distanceHighscore = distanceTraveled;
                }
            }

            if (!lootBoxMenuMoving && GameScene.selectedCompGO != null && statsDoneCounting) {
                if (GameScene.timesPlayed != 0) {
                    StartCoroutine(AnimateMenuHorizontal(homeButton, homeButton.localPosition.x, homeButton.localPosition.x - 300, 1f));
                }

                lootBoxMenuMoving = true;
                StartCoroutine(AnimateMenuVertical(lootBoxMenu, lootBoxMenuStartY, 0, lootBoxMenuEnterTime));

                lootBoxImage.sprite = GameScene.selectedCompGO.GetComponent<Comp>().compSprite;
                lootBoxImage.transform.rotation = Quaternion.Euler(0, 0, GameScene.selectedCompRot);

                textCompName.text = GameScene.selectedCompGO.GetComponent<Comp>().compName;
                textCompDescription.text = GameScene.selectedCompGO.GetComponent<Comp>().compDescription;
            }
        }

        if (howToPlayActive && Input.GetMouseButtonDown(0)) {
            howToPlayActive = false;
            StartCoroutine(AnimateMenuVertical(howToPlayMenu, 0f, howToPlayOriginalPos.y, topMenuAnimateTime));
        }

        lastState = GameScene.stateCurrent;
    }

    #region Enter Game
    private IEnumerator EnterGame() {
        animator.Play("Riding");
        StartCoroutine(EnterDapperRabbit());
        yield return new WaitForSeconds(0.122f);

        StartCoroutine(AnimateMenuVertical(buttonPlayGame, buttonPlayGame.localPosition.y, buttonPlayGame.localPosition.y + 700, buttonAnimateTime));
        yield return new WaitForSeconds(0.12f);

        StartCoroutine(AnimateMenuVertical(buttonHowToPlay, buttonHowToPlay.localPosition.y, buttonHowToPlay.localPosition.y + 700, buttonAnimateTime));
        yield return new WaitForSeconds(0.12f);

        StartCoroutine(AnimateMenuVertical(buttonExit, buttonExit.localPosition.y, buttonExit.localPosition.y + 700, buttonAnimateTime));
    }

    private IEnumerator EnterDapperRabbit() {
        float sTime = Time.time;
        float originalXPos = dapperRabbitSprite.localPosition.x;
        float startXPos = originalXPos - 30;
        dapperRabbitSprite.localPosition = new Vector3(startXPos, dapperRabbitSprite.localPosition.y, dapperRabbitSprite.localPosition.z);
        dapperRabbitSprite.gameObject.SetActive(true);

        while (dapperRabbitSprite.localPosition.x != originalXPos) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / dapperAnimateTime;
            dapperRabbitSprite.localPosition = new Vector3(Mathf.SmoothStep(startXPos, originalXPos, t), dapperRabbitSprite.localPosition.y, dapperRabbitSprite.localPosition.z);
        }
        animator.Play("Idle");
    }
    #endregion

    #region Animations Vertical & Horizontal
    private IEnumerator AnimateMenuVertical(Transform menu, float startPos, float endPos, float time) {
        float sTime = Time.time;
        while (menu.localPosition.y != endPos) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / time;
            menu.localPosition = new Vector3(menu.localPosition.x, Mathf.SmoothStep(startPos, endPos, t), menu.localPosition.z);
        }
    }

    private IEnumerator AnimateMenuHorizontal(Transform menu, float startPos, float endPos, float time) {
        float sTime = Time.time;
        while (menu.localPosition.x != endPos) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / time;
            menu.localPosition = new Vector3(Mathf.SmoothStep(startPos, endPos, t), menu.localPosition.y, menu.localPosition.z);
        }
    }
    #endregion

    #region Animations Stats
    private IEnumerator AnimateHighscoreSpriteScale() {
        float sTime = Time.time;
        while (newRecordImage.localScale != newRecordImageOriginalScale) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / 0.5f;
            newRecordImage.localScale = new Vector3(
                Mathf.SmoothStep(0f, newRecordImageOriginalScale.x, t),
                Mathf.SmoothStep(0f, newRecordImageOriginalScale.y, t),
                newRecordImageOriginalScale.z);
        }
    }

    private IEnumerator AnimateStatsCounter() {
        statsHighscore.text = GameScene.distanceHighscore + " METERS";
        statsDistance.text = "0 METERS";
        float oldHS = GameScene.distanceHighscore;
        yield return new WaitForSeconds(1f);
        float d = 0f;
        float hs = oldHS;
        int plusModifier = ((int) GameScene.distanceHighscore / 500) + 2;

        while (d != distanceTraveled || hs != GameScene.distanceHighscore) {
            yield return new WaitForFixedUpdate();
            if (d != distanceTraveled) {
                d = d + plusModifier;
                if (d > distanceTraveled) {
                    d = distanceTraveled;
                }
                statsDistance.text = d + " METERS";
            }
            if (d > oldHS && hs != GameScene.distanceHighscore) {
                hs = hs + plusModifier;
                statsHighscore.text = hs + " METERS";
                if (hs > GameScene.distanceHighscore) {
                    hs = GameScene.distanceHighscore;
                }
                if (!displayingNewRecordImage) {
                    displayingNewRecordImage = true;
                    StartCoroutine(AnimateHighscoreSpriteScale());
                }
            }
        }
        yield return new WaitForSeconds(1f);
        statsDoneCounting = true;
    }
    #endregion

    #region Animation Camera
    private IEnumerator PanCameraToPlayer() {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().EnterFlyState();

        float sTime = Time.time;
        while (mainCamera.transform.position != new Vector3(cameraFollowPlayer.cameraPos.x, cameraFollowPlayer.cameraPos.y, -10)) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / cameraAnimateTime;
            mainCamera.transform.position = new Vector3(
                Mathf.SmoothStep(cameraOriginalPos.x, cameraFollowPlayer.cameraPos.x, t),
                Mathf.SmoothStep(cameraOriginalPos.y, cameraFollowPlayer.cameraPos.y, t),
                -10);
            mainCamera.orthographicSize = Mathf.SmoothStep(mainMenuCameraZoom, playCameraZoom, t);
        }

        GameScene.EnterStateFly();
        cameraFollowPlayer.cameraFollowingPlayer = true;
    }
    #endregion

    #region Animation Visual Tutorials

    private IEnumerator AnimateRotationVisualTutorial() {
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(AnimateMenuVertical(wForForwardMenu, wForForwardMenu.localPosition.y, wForForwardMenu.localPosition.y + 400, 0.5f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(AnimateMenuVertical(adToRotateMenu, adToRotateMenu.localPosition.y, adToRotateMenu.localPosition.y - 400, 0.5f));
        //yield return new WaitForSeconds(0.3f);
        //wForForwardMenu.gameObject.SetActive(false);
        yield return new WaitForSeconds(3.2f);
        StartCoroutine(AnimateMenuVertical(adToRotateMenu, adToRotateMenu.localPosition.y, adToRotateMenu.localPosition.y + 400, 0.5f));
        wForForwardMenu.gameObject.SetActive(false);
    }

    private IEnumerator AnimateCompCanBeActivatedVisualTutorial() {
        yield return new WaitForSeconds(0.1f);
        getLetterSprite.SetActivationLetter(GameScene.compThatCanBeActivatedChar);
        StartCoroutine(AnimateMenuVertical(compCanBeActivatedMenu, compCanBeActivatedMenu.localPosition.y, compCanBeActivatedMenu.localPosition.y - 400, 1f));
        yield return new WaitForSeconds(8f);
        StartCoroutine(AnimateMenuVertical(compCanBeActivatedMenu, compCanBeActivatedMenu.localPosition.y, compCanBeActivatedMenu.localPosition.y + 400, 1f));
    }

    #endregion

    #region Button Home
    public void HomeButton() {
        GameScene.EnterStateMainMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region Button Play
    public void Play() {
        StartCoroutine(AnimatePlay());
    }

    private IEnumerator AnimatePlay() {
        yield return new WaitForFixedUpdate();
        animator.Play("Riding");
        StartCoroutine(ExitDapperRabbit());

        StartCoroutine(AnimateMenuVertical(buttonExit, buttonExit.localPosition.y, buttonExit.localPosition.y - 700, buttonAnimateTime / 2));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(AnimateMenuVertical(buttonHowToPlay, buttonHowToPlay.localPosition.y, buttonHowToPlay.localPosition.y - 700, buttonAnimateTime / 2));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(AnimateMenuVertical(buttonPlayGame, buttonPlayGame.localPosition.y, buttonPlayGame.localPosition.y - 700, buttonAnimateTime / 2));

        yield return new WaitForSeconds(0.2f);
        GameScene.ResetPlayerPosition();
        StartCoroutine(PanCameraToPlayer());
    }

    private IEnumerator ExitDapperRabbit() {
        float sTime = Time.time;
        float startXPos = dapperRabbitSprite.localPosition.x;
        float endXPos = startXPos + 100;

        while (dapperRabbitSprite.localPosition.x != endXPos) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / dapperAnimateTime;
            dapperRabbitSprite.localPosition = new Vector3(Mathf.SmoothStep(startXPos, endXPos, t), dapperRabbitSprite.localPosition.y, dapperRabbitSprite.localPosition.z);
        }
        animator.Play("Idle");
        dapperRabbitSprite.gameObject.SetActive(false);
    }
    #endregion

    #region Button How To Play
    public void HowToPlay() {
        StartCoroutine(AnimateMenuVertical(howToPlayMenu, howToPlayOriginalPos.y, 0f, topMenuAnimateTime));
        howToPlayActive = true;
    }
    #endregion

    #region Button Attach Component
    public void AttachComponent() {
        GameScene.EnterStatePlaceComponent();
        GameObject[] oldPlayers = GameObject.FindGameObjectsWithTag("PlayerOld");
        foreach (GameObject op in oldPlayers) {
            Destroy(op);
        }
        StartCoroutine(AnimateMenuVertical(lootBoxMenu, 0, lootBoxMenuStartY, lootBoxMenuExitTime));
        lootBoxMenuMoving = false;
        lootBoxMenu.localPosition = new Vector3(0, lootBoxMenuStartY, 0);
        buttonReroll.interactable = true;

        statsMenu.localPosition = new Vector3(0f, -700, 0f);

        displayingNewRecordImage = false;
        newRecordImage.localScale = Vector3.zero;
    }
    #endregion

    #region Button Reroll Component
    // Reroll Component button
    public void RerollComponent() {
        buttonReroll.interactable = false;
        buttonAttach.interactable = false;
        lootBoxImage.sprite = rerollProcess;
        textCompName.text = "Rerolling";
        textCompDescription.text = "Checking for valid placements";
        StartCoroutine(StartRerollComponent());
    }

    public IEnumerator StartRerollComponent() {
        GameObject oldComp = GameScene.selectedCompGO;
        StartCoroutine(lootCrate.GetNextComponent());

        System.DateTime startTime = System.DateTime.UtcNow;
        System.TimeSpan ts = System.DateTime.UtcNow - startTime;
        float rotZ = 0f;

        while (GameScene.selectedCompGO == oldComp || ((int)ts.TotalMilliseconds) < 2000) {
            yield return new WaitForFixedUpdate();
            ts = System.DateTime.UtcNow - startTime;

            rotZ = rotZ - 6f;
            lootBoxImage.rectTransform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
        lootBoxImage.sprite = GameScene.selectedCompGO.GetComponent<Comp>().compSprite;
        lootBoxImage.transform.rotation = Quaternion.Euler(0, 0, GameScene.selectedCompRot);
        buttonAttach.interactable = true;

        textCompName.text = GameScene.selectedCompGO.GetComponent<Comp>().compName;
        textCompDescription.text = GameScene.selectedCompGO.GetComponent<Comp>().compDescription;
    }
    #endregion

    #region Button Exit
    public void ExitButton() {
        Application.Quit();
    }
    #endregion
}
