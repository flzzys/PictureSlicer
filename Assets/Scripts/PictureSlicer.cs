using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureSlicer : MonoBehaviour {
    public SizeManipulator sizeManipulator;

    Vector2 size;

    private void Awake() {
        size = GetComponent<RectTransform>().sizeDelta;


    }
}
