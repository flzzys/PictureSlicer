using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//截图器的移动框，水平竖直缩放尚未完成
public class SizeManipulator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    Vector2 size;
    Vector2 offset;
    bool pressHandle;

    public Transform siliceParent;

    //父级尺寸
    Vector2 parentSize;

    float movementRestrict;

    public SizeManipulatorHandle handle_TopLeft;
    public SizeManipulatorHandle handle_TopRight;
    public SizeManipulatorHandle handle_BottomLeft;
    public SizeManipulatorHandle handle_BottomRight;
    public SizeManipulatorHandle handle_Top;
    public SizeManipulatorHandle handle_Right;
    public SizeManipulatorHandle handle_Bottom;
    public SizeManipulatorHandle handle_Left;

    Dictionary<SizeManipulatorHandle, Vector2> handleDic = new Dictionary<SizeManipulatorHandle, Vector2>();
    SizeManipulatorHandle currentHandle;

    void Awake() {
        handleDic.Add(handle_TopLeft, new Vector2(-1, 1));
        handleDic.Add(handle_TopRight, new Vector2(1, 1));
        handleDic.Add(handle_BottomLeft, new Vector2(-1, -1));
        handleDic.Add(handle_BottomRight, new Vector2(1, -1));
        handleDic.Add(handle_Top, new Vector2(0, 1));
        handleDic.Add(handle_Right, new Vector2(1, 0));
        handleDic.Add(handle_Bottom, new Vector2(0, -1));
        handleDic.Add(handle_Left, new Vector2(-1, 0));

        foreach (var item in handleDic) {
            item.Key.onMouseDown = Handle_OnMouseDown;
            item.Key.onMouseUp = Handle_OnMouseUp;
        }

        parentSize = siliceParent.GetComponent<RectTransform>().sizeDelta;
    }

    int minWidth = 200;

    //获取可用位置
    //Vector2 GetClampPos(Vector2 pos, Vector2 area) {
        
    //}

    void Update() {
        size = GetComponent<RectTransform>().sizeDelta;
        if (moving) {
            Vector2 desiredPos = (Vector2)Input.mousePosition + offset;

            //移动范围限制
            Vector2 movementRestrict = (Vector2)Camera.main.WorldToScreenPoint(transform.parent.position) + (parentSize - size) / 2;
            movementRestrict = Camera.main.ScreenToWorldPoint(movementRestrict);
            //movementRestrict += (Vector2)transform.parent.position;
            print(movementRestrict);
            Debug.DrawLine(-movementRestrict, new Vector2(-movementRestrict.x, movementRestrict.y));

            //movementRestrict = (Vector2)transform.parent.position + (parentSize.x - size.x) / 2;
            //desiredPos.x = Mathf.Clamp(desiredPos.x, -movementRestrict.x, movementRestrict.x);
            //desiredPos.y = Mathf.Clamp(desiredPos.y, -movementRestrict.y, movementRestrict.y);

            //transform.position = desiredPos;

            Debug.DrawLine(Vector2.zero, Camera.main.ScreenToWorldPoint(desiredPos));
        }

        if (pressHandle) {
            //目标移动方向
            Vector2 targetDir = handleDic[currentHandle];

        }

        return;

        //切割器尺寸
        size = GetComponent<RectTransform>().sizeDelta;

        //移动范围
        movementRestrict = (parentSize.x - size.x) / 2;

        if (pressHandle) {
            Vector2 targetDir = handleDic[currentHandle];

            Vector2 desiredPos = (Vector2)Input.mousePosition + offset;

            Vector2 desiredPosOffset = desiredPos - originPos;

            float shortest = Mathf.Min(Mathf.Abs(desiredPosOffset.x), Mathf.Abs(desiredPosOffset.y));
            if(targetDir.magnitude == 1) {
                shortest = (new Vector2(Mathf.Abs(targetDir.x), Mathf.Abs(targetDir.y)) * desiredPosOffset).magnitude;
            }

            Vector2 realPos = originPos + targetDir * shortest;

            realPos.x = Mathf.Clamp(realPos.x, -(parentSize.x / 2), parentSize.x / 2);
            realPos.y = Mathf.Clamp(realPos.y, -(parentSize.x / 2), parentSize.x / 2);

            //print(originPos.x + targetDir.x * minWidth + ", " + parentSize.x / 2);
            //realPos.x = Mathf.Clamp(realPos.x, originPos.x + targetDir.x * minWidth, parentSize.x / 2);
            
            //pos = realPos;

            GetComponent<RectTransform>().anchoredPosition = originPos + (realPos - originPos) / 2;
            //GetComponent<RectTransform>().sizeDelta = Vector2.one * Mathf.Max(Mathf.Abs((realPos - originPos).x), Mathf.Abs((realPos - originPos).y));
            GetComponent<RectTransform>().sizeDelta = Vector2.one * Mathf.Min(Mathf.Abs((realPos - originPos).x), Mathf.Abs((realPos - originPos).y));
        }

        if (moving) {
            Vector2 desiredPos = (Vector2)Input.mousePosition + offset;
            desiredPos.x = Mathf.Clamp(desiredPos.x, -movementRestrict, movementRestrict);
            desiredPos.y = Mathf.Clamp(desiredPos.y, -movementRestrict, movementRestrict);
            GetComponent<RectTransform>().anchoredPosition = desiredPos;
        }

        if (Input.GetKeyDown("1")) {
            print(Camera.main.WorldToScreenPoint(transform.position));
            print(Input.mousePosition);
        }
    }

    public RectTransform rt;

    //点在可用范围内
    bool IsInRange(Vector2 pos) {
        return pos.x > -(parentSize.x / 2) && pos.x < parentSize.x / 2 && pos.y > -(parentSize.y / 2) && pos.y < parentSize.y / 2;
    }

    bool moving;

    //鼠标按下和松开本体，移动本体
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        moving = true;

        //offset = GetComponent<RectTransform>().anchoredPosition - (Vector2)Input.mousePosition;
        offset = Camera.main.WorldToScreenPoint(transform.position) - Input.mousePosition;
    }
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        moving = false;
    }

    Vector2 originPos;

    //鼠标按下和松开把手
    void Handle_OnMouseDown(SizeManipulatorHandle handle) {
        currentHandle = handle;

        pressHandle = true;

        Vector2 targetDir = handleDic[handle];
        originPos = GetComponent<RectTransform>().anchoredPosition + -targetDir * size / 2;
        Vector2 targetPos = GetComponent<RectTransform>().anchoredPosition + targetDir * size / 2;

        offset = targetPos - (Vector2)Input.mousePosition;
    }
    void Handle_OnMouseUp(SizeManipulatorHandle handle) {
        pressHandle = false;
    }

    //Vector2 pos;
    //void OnDrawGizmos() {
    //    Vector2 p = pos;
    //    p += (Vector2)GetComponentInParent<Canvas>().transform.localPosition + new Vector2(0, -235.5f);
    //    Gizmos.DrawWireSphere(p, 10f);
    //}
}
