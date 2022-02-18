using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRotation : MonoBehaviour {

    [SerializeField] private float rotationAmount = 10f;

    void Update() {
        //transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z + rotationAmount);
        transform.RotateAround(transform.position, Vector3.forward, rotationAmount * Time.deltaTime);
    }
}
