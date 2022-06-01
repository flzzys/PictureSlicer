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
        //����λ�ƺͳߴ�
        var offset = sliceData.offset;
        var zoom = sliceData.zoom;

        //�������ǽ�ͼ������ʾ��������ͬ����
        Vector2 scale = Vector2.one * zoom;

        //��ͼ��ƫ���Ǵ����Ͻǿ�ʼ��ת��������ƫ��
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
