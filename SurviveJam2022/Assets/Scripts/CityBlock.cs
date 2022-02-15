using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlock : MonoBehaviour {

    [SerializeField] GameObject cityBlockPrefab;
    [SerializeField] Transform nextBlockPos;
    [SerializeField] Transform cloneCheckPos;
    private GameObject cityblocksParent;
    private Transform player = null;
    private Transform playerClone = null;
    private bool createdClone = false;

    private void Start() {
        cityblocksParent = GameObject.FindGameObjectWithTag("Cityblocks");
    }

    private void Update() {
        if (createdClone) { return; }
        if (player == null) { 
            player = GameObject.FindGameObjectWithTag("Player").transform; 
        }
        if (GameObject.FindGameObjectWithTag("PlayerOld") != null && playerClone == null) { 
            playerClone = GameObject.FindGameObjectWithTag("PlayerOld").transform; 
        }
        
        if (player != null && player.position.x > cloneCheckPos.position.x) {
            Instantiate(cityBlockPrefab, nextBlockPos.position, nextBlockPos.rotation, cityblocksParent.transform);
            createdClone = true;
        }
        if (playerClone != null && playerClone.position.x > cloneCheckPos.position.x) {
            Instantiate(cityBlockPrefab, nextBlockPos.position, nextBlockPos.rotation, cityblocksParent.transform);
            createdClone = true;
        }
    }
}
