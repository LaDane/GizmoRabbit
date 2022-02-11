using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour {

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

        // Check if there is a valid position
        int loopIteration = 0;
        while (true) {
            GameObject componentPrefab = grabbaleList[0];
            GameObject component = Instantiate(componentPrefab, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));
            Collider2D componentCollider = component.GetComponent<Comp>().placementCollider;

            for (int i = 0; i < 50; i++) {
                float componentRotation = Random.Range(-90, 90);
                component.transform.rotation = Quaternion.Euler(0, 0, componentRotation);

                foreach(GameObject node in allNodes) {
                    component.transform.position = node.transform.position;
                    //component.transform.position = node.GetComponent<CompNode>().placementPos.position;

                    yield return new WaitForSeconds(0.1f);

                    if (CanBePlaced(component, componentCollider)) {
                        Destroy(component);
                        grabbaleList.RemoveAt(0);
                        GameScene.selectedCompRot = componentRotation;
                        GameScene.selectedCompGO = componentPrefab;
                        yield break;
                    }
                }
            }

            Debug.Log("Failed to place "+ component.name);

            Destroy(component);
            grabbaleList.RemoveAt(0);

            //getIndex++;
            if (grabbaleList.Count == 0) {
                grabbaleList = new List<GameObject>(originalList);
            }

            loopIteration++;
            if (loopIteration > 50) {
                Debug.LogError("GetNextComponent failed 50 times");
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
