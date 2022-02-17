using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLetterSprite : MonoBehaviour {

    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Letter Sprites")]
    [SerializeField] private Sprite sB;
    [SerializeField] private Sprite sC;
    [SerializeField] private Sprite sE;
    [SerializeField] private Sprite sF;
    [SerializeField] private Sprite sG;
    [SerializeField] private Sprite sH;
    [SerializeField] private Sprite sI;
    [SerializeField] private Sprite sJ;
    [SerializeField] private Sprite sK;
    [SerializeField] private Sprite sL;
    [SerializeField] private Sprite sM;
    [SerializeField] private Sprite sN;
    [SerializeField] private Sprite sO;
    [SerializeField] private Sprite sP;
    [SerializeField] private Sprite sQ;
    [SerializeField] private Sprite sR;
    [SerializeField] private Sprite sS;
    [SerializeField] private Sprite sT;
    [SerializeField] private Sprite sU;
    [SerializeField] private Sprite sV;
    [SerializeField] private Sprite sX;
    [SerializeField] private Sprite sY;
    [SerializeField] private Sprite sZ;

    public Dictionary<char, Sprite> letterSpriteDict = new Dictionary<char, Sprite>();

    public void SetActivationLetter(char c) {
        FillLetterDict();

        Sprite letterSprite = null;
        letterSpriteDict.TryGetValue(c, out letterSprite);
        if (letterSprite != null) {
            spriteRenderer.sprite = letterSprite;
        }
        else {
            Debug.LogError("Could not get activation letter sprite for char : " + c);
        }
    }

    private void FillLetterDict() {
        letterSpriteDict.Add('b', sB);
        letterSpriteDict.Add('c', sC);
        letterSpriteDict.Add('e', sE);
        letterSpriteDict.Add('f', sF);
        letterSpriteDict.Add('g', sG);
        letterSpriteDict.Add('h', sH);
        letterSpriteDict.Add('i', sI);
        letterSpriteDict.Add('j', sJ);
        letterSpriteDict.Add('k', sK);
        letterSpriteDict.Add('l', sL);
        letterSpriteDict.Add('m', sM);
        letterSpriteDict.Add('n', sN);
        letterSpriteDict.Add('o', sO);
        letterSpriteDict.Add('p', sP);
        letterSpriteDict.Add('q', sQ);
        letterSpriteDict.Add('r', sR);
        letterSpriteDict.Add('s', sS);
        letterSpriteDict.Add('t', sT);
        letterSpriteDict.Add('u', sU);
        letterSpriteDict.Add('v', sV);
        letterSpriteDict.Add('x', sX);
        letterSpriteDict.Add('y', sY);
        letterSpriteDict.Add('z', sZ);
    }
}
