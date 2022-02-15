using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnergyLevel : MonoBehaviour {
    [HideInInspector] public CompActivate compActivate;
    private Slider energySlider;

    private void Start() {
        energySlider = GetComponentInChildren<Slider>();
    }

    private void Update() {
        if (compActivate == null) { 
            Destroy(gameObject);
            return;
        }

        if (GameScene.stateLootCrate && energySlider.gameObject.activeInHierarchy) {
            energySlider.gameObject.SetActive(false);
        }
        else if (!GameScene.stateLootCrate && !energySlider.gameObject.activeInHierarchy) {
            energySlider.gameObject.SetActive(true);
        }

        energySlider.value = compActivate.energyValue;

        transform.position = compActivate.energyLevelPos.position;
        transform.rotation = compActivate.transform.rotation;
    }
}
