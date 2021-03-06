using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour {

    [Header("Test")]
    [SerializeField] private bool getSpecificComp = false;
    [SerializeField] private GameObject specificComp = null;
    [SerializeField] private float specificRotation = -90;

    [Header("Drag")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Comp playerComp;
    [SerializeField] private Comps comps;
    [SerializeField] private LayerMask placementLayer;

    private List<GameObject> originalList;
    private List<GameObject> grabbaleList;

    private void Awake() {
        originalList = new List<GameObject>(comps.components);
        originalList.Shuffle();

        grabbaleList = new List<GameObject>(originalList);
    }

    public IEnumerator GetNextComponent() {
        //if (getSpecificComp && specificComp != null) {
        //    GameScene.selectedCompRot = specificRotation;
        //    GameScene.selectedCompGO = specificComp;
        //    yield break;
        //}

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerComp = player.GetComponent<Comp>();

        playerController.EnterPlaceComponentState();

        if (grabbaleList.Count == 0) {
            grabbaleList = new List<GameObject>(originalList);
        }

        // Get all nodes
        List<GameObject> allNodes = new List<GameObject>();
        foreach (GameObject node in playerComp.componentNodes) {
            if (node.activeInHierarchy) {
                allNodes.Add(node);
            }
        }
        foreach (Comp c in playerController.compList) {
            foreach (GameObject node in c.componentNodes) {
                if (node.activeInHierarchy) {
                    allNodes.Add(node);
                }
            }
        }
        allNodes.Reverse();

        // Check if there is a valid position
        System.DateTime startTime = System.DateTime.UtcNow;
        int loopIteration = 0;
        bool compCanBePlaced = false;
        while (true) {
            GameObject componentPrefab = grabbaleList[0];
            GameObject component = Instantiate(componentPrefab, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));
            Collider2D componentCollider = component.GetComponent<Comp>().placementCollider;
            component.GetComponent<Comp>().StatePlaceComponent();
            

            for (int i = 0; i < 20; i++) {
                float componentRotation = Random.Range(-90, 90);
                component.transform.rotation = Quaternion.Euler(0, 0, componentRotation);

                foreach(GameObject node in allNodes) {
                    node.SetActive(true);
                    component.transform.position = node.transform.position;

                    yield return new WaitForSeconds(0.01f);

                    if (CanBePlaced(component, componentCollider)) {
                        //Destroy(component);

                        if (getSpecificComp && specificComp != null) {
                            GameScene.selectedCompRot = specificRotation;
                            GameScene.selectedCompGO = specificComp;
                        }
                        else {
                            //grabbaleList.RemoveAt(0);
                            GameScene.selectedCompRot = componentRotation;
                            GameScene.selectedCompGO = componentPrefab;
                        }


                        compCanBePlaced = true;
                        //yield break;
                    }
                    else {
                        node.SetActive(false);
                    }
                }
                if (compCanBePlaced) {
                    grabbaleList.RemoveAt(0);
                    System.TimeSpan ts = System.DateTime.UtcNow - startTime;
                    Debug.Log(component.name + " can be placed at : " + component.transform.position + "\tDuration seconds : " + (ts.TotalMilliseconds / 1000).ToString("0.00"));
                    Destroy(component);
                    yield break;
                }
            }

            Debug.Log("Failed to place "+ component.name);

            Destroy(component);
            grabbaleList.RemoveAt(0);

            //getIndex++;
            if (grabbaleList.Count == 1) {
                originalList.Shuffle();
                grabbaleList = new List<GameObject>(originalList);
            }

            loopIteration++;
            if (loopIteration > 50) {
                Debug.LogError("GetNextComponent failed 50 times");
                // Give messge to player that theres no spcae to place comps on their vehicle with home button
                yield break;
            }
        }
    }

    private bool CanBePlaced(GameObject component, Collider2D col2d) {
        Collider2D[] allOverlappingColliders = new Collider2D[16];

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(placementLayer);
        contactFilter.useLayerMask = true;

        int overlapCount = Physics2D.OverlapCollider(col2d, contactFilter, allOverlappingColliders);
        if (overlapCount == 0) {
            return true;
        }
        else {
            return false;
        }
    }
}
