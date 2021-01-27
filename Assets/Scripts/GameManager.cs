using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public Button button_Slice;
    public Image image;

    public PictureSlicer pictureSlicer;

    private void Awake() {
        button_Slice.onClick.AddListener(() => {
            Slice();
        });
    }

    private void Update() {
        Slice();
    }

    void Slice() {
        SliceData sliceData = pictureSlicer.GetSliceData();

        //设置位移和尺寸
        image.rectTransform.anchoredPosition = -image.transform.parent.GetComponent<RectTransform>().sizeDelta * sliceData.offset / sliceData.zoom / 2;
        image.transform.localScale = Vector3.one * (1 / sliceData.zoom);
    }
}
