using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

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


    private void Update() {
        distanceTrackerText.text = "Distance Traveled\n" + distanceTraveled +"\n\nAltitude\n" + altitude;

        if (GameScene.stateLootCrate) {
            if (!lootBoxMenuMoving && !searchingForComp) {
                searchingForComp = true;
                //lootBoxMenuMoving = true;
                //lootBoxMenu.localPosition = new Vector3(0, lootBoxMenuStartY, 0);
                //startTime = Time.time;
                StartCoroutine(lootCrate.GetNextComponent());
            }

            // Move loot box menu up to center of screen
            if (!lootBoxMenuMoving && lootBoxMenu.localPosition.y != 0 && GameScene.selectedCompGO != null) {
                //float t = (Time.time - startTime) / lootBoxMenuEnterTime;
                //lootBoxMenu.localPosition = new Vector3(0, Mathf.SmoothStep(lootBoxMenuStartY, 0, t), 0);

                //if (!lootBoxSpriteFound && GameScene.selectedCompGO != null) {
                //    lootBoxImage.sprite = GameScene.selectedCompGO.GetComponent<Comp>().compSprite;
                //    lootBoxImage.transform.rotation = Quaternion.Euler(0, 0, GameScene.selectedCompRot); 
                //}
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
}
