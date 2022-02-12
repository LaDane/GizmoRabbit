using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    // Camera sizes
    // Main menu = 10
    // Place Component = 5

    [Header("Distance + Altitude")]
    [SerializeField] private Text distanceTrackerText;
    [HideInInspector] public int distanceTraveled;
    [HideInInspector] public int altitude;

    // Loot box screen
    [Header("Loot Box Screen")]
    [SerializeField] private LootCrate lootCrate;
    [SerializeField] private Transform lootBoxMenu;
    [SerializeField] private float lootBoxMenuEnterTime = 5f;
    [SerializeField] private float lootBoxMenuExitTime = 1f;
    [SerializeField] private Image lootBoxImage;
    private bool lootBoxMenuMoving = false;
    private bool searchingForComp = false;
    //private bool lootBoxSpriteFound = false;
    private float lootBoxMenuStartY = -700;
    private float startTime;

    // Main menu screen
    [Header("Main Menu")]
    [SerializeField] private Transform dapperRabbitSprite;
    [SerializeField] private Transform buttonPlayGame;
    [SerializeField] private Transform buttonHowToPlay;
    [SerializeField] private Transform buttonSettings;
    [SerializeField] private float buttonAnimateTime = 1.5f;
    [SerializeField] private float dapperAnimateTime = 4f;
    private Vector3 mainMenuCameraPos;

    [Header("How to play + Settings")]
    [SerializeField] private Transform howToPlayMenu;
    [SerializeField] private Transform settingsMenu;
    [SerializeField] private float topMenuAnimateTime = 0.8f;
    private Vector3 howToPlayOriginalPos;
    private bool howToPlayActive = false;
    private Vector3 settingsOriginalPos;
    private bool settingsActive = false;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFollowPlayer cameraFollowPlayer;
    [SerializeField] private float cameraAnimateTime = 5f;
    //[SerializeField] private Vector3 playerFocusPos = new Vector3(0f, 0f, -10);
    private Vector3 cameraOriginalPos;
    private int mainMenuCameraZoom = 10;
    private int playCameraZoom = 5;

    private void Start() {
        howToPlayOriginalPos = howToPlayMenu.localPosition;
        settingsOriginalPos = settingsMenu.localPosition;

        mainCamera.orthographicSize = mainMenuCameraZoom;
        cameraOriginalPos = mainCamera.transform.position;

        StartCoroutine(EnterGame());
    }

    private void Update() {
        distanceTrackerText.text = "Distance Traveled\n" + distanceTraveled +"\n\nAltitude\n" + altitude;

        if (howToPlayActive && Input.GetMouseButtonDown(0)) {
            howToPlayActive = false;
            StartCoroutine(AnimateTopMenu(howToPlayMenu, 0f, howToPlayOriginalPos.y, topMenuAnimateTime));
        }
        if (settingsActive && Input.GetMouseButtonDown(0)) {                // CHANGE THIS 
            settingsActive = false;
            StartCoroutine(AnimateTopMenu(settingsMenu, 0f, settingsOriginalPos.y, topMenuAnimateTime));
        }

        if (GameScene.stateLootCrate) {
            if (!lootBoxMenuMoving && !searchingForComp) {
                searchingForComp = true;
                StartCoroutine(lootCrate.GetNextComponent());
            }

            // Move loot box menu up to center of screen
            if (!lootBoxMenuMoving && lootBoxMenu.localPosition.y != 0 && GameScene.selectedCompGO != null) {
                lootBoxMenuMoving = true;
                StartCoroutine(EnterLootBoxMenu());
                lootBoxImage.sprite = GameScene.selectedCompGO.GetComponent<Comp>().compSprite;
                lootBoxImage.transform.rotation = Quaternion.Euler(0, 0, GameScene.selectedCompRot);
            }
        }
        else {
            searchingForComp = false;
        }
    }

    // Button press function
    public void AttachComponent() {
        GameScene.EnterStatePlaceComponent();
        GameObject[] oldPlayers = GameObject.FindGameObjectsWithTag("PlayerOld");
        foreach (GameObject op in oldPlayers) {
            Destroy(op);
        }
        StartCoroutine(RemoveLootBoxMenu());
    }

    // Loot box menu
    private IEnumerator EnterLootBoxMenu() {
        startTime = Time.time;
        while (lootBoxMenu.localPosition.y != 0) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - startTime) / lootBoxMenuEnterTime;
            lootBoxMenu.localPosition = new Vector3(0, Mathf.SmoothStep(lootBoxMenuStartY, 0, t), 0);
        }
        lootBoxMenuMoving = false;
    }

    private IEnumerator RemoveLootBoxMenu() {
        startTime = Time.time;
        while (lootBoxMenu.localPosition.y != lootBoxMenuStartY) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - startTime) / lootBoxMenuExitTime;
            lootBoxMenu.localPosition = new Vector3(0, Mathf.SmoothStep(0, lootBoxMenuStartY, t), 0);
        }
        lootBoxMenuMoving = false;
        lootBoxMenu.localPosition = new Vector3(0, lootBoxMenuStartY, 0);
    }

    // Main menu
    private IEnumerator EnterGame() {
        StartCoroutine(EnterDapperRabbit());
        yield return new WaitForSeconds(0.122f);
        StartCoroutine(EnterMainMenuButton(buttonPlayGame));
        yield return new WaitForSeconds(0.12f);
        StartCoroutine(EnterMainMenuButton(buttonHowToPlay));
        yield return new WaitForSeconds(0.12f);
        StartCoroutine(EnterMainMenuButton(buttonSettings));
    }

    private IEnumerator EnterMainMenuButton(Transform button) {
        float sTime = Time.time;
        float originalYPos = button.localPosition.y;
        float startYPos = originalYPos - 700;
        button.localPosition = new Vector3(button.localPosition.x, startYPos, button.localPosition.z);
        button.gameObject.SetActive(true);

        while (button.localPosition.y != originalYPos) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / buttonAnimateTime;
            button.localPosition = new Vector3(button.localPosition.x, Mathf.SmoothStep(startYPos, originalYPos, t), button.localPosition.z);
        }
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
    }

    // How to play & Settings
    public void HowToPlay() {
        StartCoroutine(AnimateTopMenu(howToPlayMenu, howToPlayOriginalPos.y, 0f, topMenuAnimateTime));
        howToPlayActive = true;
    }

    public void Settings() {
        StartCoroutine(AnimateTopMenu(settingsMenu, settingsOriginalPos.y, 0f, topMenuAnimateTime));
        settingsActive = true;
    }

    private IEnumerator AnimateTopMenu(Transform menu, float startPos, float endPos, float animateTime) {
        float sTime = Time.time;
        while (menu.localPosition.y != endPos) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / animateTime;
            menu.localPosition = new Vector3(menu.localPosition.x, Mathf.SmoothStep(startPos, endPos, t), menu.localPosition.z);
        }
    }

    // Play
    public void Play() {
        StartCoroutine(AnimatePlay());
    }

    private IEnumerator AnimatePlay() {
        yield return new WaitForFixedUpdate();
        StartCoroutine(ExitDapperRabbit());

        StartCoroutine(ExitMainMenuButton(buttonSettings));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(ExitMainMenuButton(buttonHowToPlay));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(ExitMainMenuButton(buttonPlayGame));

        yield return new WaitForSeconds(0.2f);
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
        dapperRabbitSprite.gameObject.SetActive(false);
    }

    private IEnumerator ExitMainMenuButton(Transform button) {
        float sTime = Time.time;
        float originalYPos = button.localPosition.y - 700;
        float startYPos = button.localPosition.y;
        button.localPosition = new Vector3(button.localPosition.x, startYPos, button.localPosition.z);
        button.gameObject.SetActive(true);

        while (button.localPosition.y != originalYPos) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / (buttonAnimateTime / 2);
            button.localPosition = new Vector3(button.localPosition.x, Mathf.SmoothStep(startYPos, originalYPos, t), button.localPosition.z);
        }
    }

    private IEnumerator PanCameraToPlayer() {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().EnterFlyState();

        Vector3 playerPos = new Vector3(cameraFollowPlayer.cameraPos.x, cameraFollowPlayer.cameraPos.y, -10);
        float sTime = Time.time;
        while (mainCamera.transform.position != playerPos) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / cameraAnimateTime;
            mainCamera.transform.position = new Vector3(
                Mathf.SmoothStep(cameraOriginalPos.x, cameraFollowPlayer.cameraPos.x, t),
                Mathf.SmoothStep(cameraOriginalPos.y, cameraFollowPlayer.cameraPos.y, t),
                -10);
            mainCamera.orthographicSize = Mathf.SmoothStep(mainMenuCameraZoom, playCameraZoom, t);
        }

        GameScene.EnterStateFly();
        cameraFollowPlayer.followPlayer = true;
    }
}
