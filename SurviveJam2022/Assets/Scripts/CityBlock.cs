using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlock : MonoBehaviour {

    [SerializeField] GameObject cityBlockPrefab;
    [SerializeField] Transform nextBlockPos;
    [SerializeField] Transform cloneCheckPos;
    private Transform player;
    private bool createdClone = false;

    private void Update() {
        if (createdClone) { return; }
        if (player == null) { player = GameObject.FindGameObjectWithTag("Player").transform; }
        
        if (player.position.x > cloneCheckPos.position.x) {
            Instantiate(cityBlockPrefab, nextBlockPos.position, nextBlockPos.rotation);
            createdClone = true;
        }
    }
}
