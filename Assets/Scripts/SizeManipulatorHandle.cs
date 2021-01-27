using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SizeManipulatorHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Action<SizeManipulatorHandle> onMouseDown, onMouseUp;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        onMouseDown(this);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        onMouseUp(this);
    }
}
