using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PictureSlicerHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Action<PictureSlicerHandle> onMouseDown, onMouseUp;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        onMouseDown?.Invoke(this);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        onMouseUp?.Invoke(this);
    }
}
