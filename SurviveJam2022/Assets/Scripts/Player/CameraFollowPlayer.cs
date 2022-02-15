using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [HideInInspector] public Vector3 cameraPos;
    public bool cameraFollowingPlayer = false;
    private bool donePanning;
    private Vector3 dragOrigin;
    private float cameraSize = 5f;
    private float newCameraSize = 5f;
    private string lastState = "Menu";


    private void Update() {

        if (player != null) {
            cameraPos = player.position;
            cameraPos.y = cameraPos.y + 2.5f + (GameScene.componentsPlaced * 0.1f);
            cameraPos.z = -10f;


            if (!cameraFollowingPlayer && GameScene.stateFly) {
                cameraFollowingPlayer = true;
            }

            if (cameraFollowingPlayer) {
                if (GameScene.statePlaceComponent) {
                    if (lastState != GameScene.stateCurrent) {
                        donePanning = false;
                        StartCoroutine(PanCameraToPlayer(0.5f, newCameraSize, cameraSize));
                    }

                    if (donePanning) {
                        if (Input.GetMouseButtonDown(1)) {
                            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                        }
                        if (Input.GetMouseButton(1)) {
                            Vector3 difference = dragOrigin - mainCamera.ScreenToWorldPoint(Input.mousePosition);
                            mainCamera.transform.position += difference;
                        }
                    }
                }

                if (GameScene.stateFly) {
                    newCameraSize = cameraSize + (GameScene.componentsPlaced * 0.1f);
                    
                    if (lastState != GameScene.stateCurrent) {
                        donePanning = false;
                        StartCoroutine(PanCameraToPlayer(0.5f, cameraSize, newCameraSize));
                    }
                    if (donePanning) {
                        transform.position = cameraPos;
                        //if (mainCamera.orthographicSize != newCameraSize) {
                        //    mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newCameraSize, Time.deltaTime * 2);
                        //}
                    }
                }

                if (GameScene.stateLootCrate) {
                    transform.position = cameraPos;
                }
            }
        }
        else {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        lastState = GameScene.stateCurrent;
    }

    private IEnumerator PanCameraToPlayer(float time, float sizeStart, float sizeEnd) {
        yield return new WaitForSeconds(0.1f);
        Vector3 startPos = transform.position;
        float sTime = Time.time;

        while (mainCamera.transform.position != cameraPos || mainCamera.orthographicSize != sizeEnd) {
            yield return new WaitForFixedUpdate();
            float t = (Time.time - sTime) / time;
            mainCamera.transform.position = new Vector3(
                Mathf.SmoothStep(startPos.x, cameraPos.x, t),
                Mathf.SmoothStep(startPos.y, cameraPos.y, t),
                -10);
            mainCamera.orthographicSize = Mathf.SmoothStep(sizeStart, sizeEnd, t);
        }
        donePanning = true;
    }
}
