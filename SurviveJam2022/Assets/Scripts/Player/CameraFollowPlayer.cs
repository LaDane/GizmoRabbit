using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    [SerializeField] private Transform player;
    [HideInInspector] public Vector3 cameraPos;
    public bool followPlayer = false;

    private void Update() {

        if (player != null) {
            cameraPos = player.position;
            cameraPos.y = cameraPos.y + 2.5f;
            cameraPos.z = -10f;

            if (followPlayer) {
                transform.position = cameraPos;
            }
        }
        else {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
