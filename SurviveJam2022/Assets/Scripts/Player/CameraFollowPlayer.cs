using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    [SerializeField] private Transform player;

    private void Update() {

        if (player != null) {
            Vector3 cameraPos = player.position;
            cameraPos.z = -10f;
            transform.position = cameraPos;
        }
        else {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
