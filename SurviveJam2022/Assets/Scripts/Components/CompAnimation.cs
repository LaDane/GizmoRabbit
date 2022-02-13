using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompAnimation : MonoBehaviour {

    [SerializeField] Animator animator;
    [SerializeField] CompActivate compActive;
    [SerializeField] string idleName;
    [SerializeField] string movingName;

    private void Update() {
        if (compActive.isActivated) {
            animator.Play(movingName);
        }
        else {
            animator.Play(idleName);
        }
    }
}
