using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlicedPictureDisplay : MonoBehaviour {
    public RectTransform imageDisplayParent;
    public Image imageDisplay;

    public void SetSize(Vector2 size) {
        imageDisplayParent.sizeDelta = size;
    }

    public void Set(SliceData sliceData) {
        //设置位移和尺寸
        var offset = sliceData.offset;
        var zoom = sliceData.zoom;

        //后续考虑截图器和显示器比例不同问题
        Vector2 scale = Vector2.one * zoom;

        //截图器偏移是从左上角开始，转换成中心偏移
        Vector2 imageSize = new Vector2(imageDisplay.sprite.texture.width, imageDisplay.sprite.texture.height);
        Vector2 realOffset = new Vector2(offset.x / imageSize.x, offset.y / imageSize.y);

        realOffset -= Vector2.one * .5f;
        realOffset *= 2;
        realOffset *= new Vector2(1, -1);

        Vector2 pictureRectSize = imageDisplayParent.sizeDelta;
        Vector2 pos = realOffset * pictureRectSize.y * scale / 2;
        pos *= -1;

        imageDisplay.rectTransform.anchoredPosition = pos;

        imageDisplay.rectTransform.sizeDelta = Vector2.one * imageDisplayParent.sizeDelta.y * zoom;

        //print(sliceData);
    }
}
