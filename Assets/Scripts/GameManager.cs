using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public PictureSlicer pictureSlicer;
    public SlicedPictureDisplay slicedPictureDisplay;

    private void Start() {
        var parentSize = slicedPictureDisplay.imageDisplayParent.sizeDelta;
        slicedPictureDisplay.SetSize(new Vector2(parentSize.y * pictureSlicer.ratio, parentSize.y));
    }

    private void Update() {
        if (Input.GetKeyDown("d")) {
            
        }
    }
}
