using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//截取数据
public struct SliceData {
    public Vector2 offset;
    public float zoom;
}

public class PictureSlicer : MonoBehaviour {
    public RectTransform picture;

    //图片尺寸
    Vector2 size;

    //把手
    public PictureSlicerHandle handle_Main;
    public PictureSlicerHandle handle_TopLeft;
    public PictureSlicerHandle handle_TopRight;
    public PictureSlicerHandle handle_BottomLeft;
    public PictureSlicerHandle handle_BottomRight;

    //最小边长
    public int minWidth = 200;

    //private void OnDrawGizmos() {
    //    print(ScreenFloatToWorldFloat(size.x).ToString("f4"));
    //    Gizmos.DrawWireCube((Vector2)transform.position, Vector3.one * 2);
    //}

    //float ScreenFloatToWorldFloat(float screenFloat) {
    //    float ratio = Screen.height / Camera.main.orthographicSize * 2;
    //    //print(ratio);

    //    return screenFloat / ratio;
    //}

    private void Awake() {
        size = picture.sizeDelta * transform.localScale.x;
        //size = picture.sizeDelta;

        InitMoving();
        InitHandles();
    }

    private void Update() {
        UpdateMoving();
        UpdateHandles();
    }

    #region 截取器移动

    //移动中
    bool moving;

    //鼠标和本体的偏移量
    Vector2 mouseOffset;

    //初始化移动数据
    void InitMoving() {
        handle_Main.onMouseDown = handle => {
            moving = true;

            mouseOffset = Camera.main.WorldToScreenPoint(handle_Main.transform.position) - Input.mousePosition;
        };
        handle_Main.onMouseUp = handle => {
            moving = false;
        };
    }

    //更新移动
    void UpdateMoving() {
        if (moving) {
            //移动的目标位置
            Vector2 desiredPos = (Vector2)Input.mousePosition + mouseOffset;

            TryMove(desiredPos);
        }
    }

    //尝试移动，不会移动到范围外
    void TryMove(Vector2 uiPos) {
        //将目标位置限制在可动范围内
        uiPos = GetClampedPos(uiPos);

        //移动
        handle_Main.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(uiPos);
    }

    //将目标位置限制在可动范围内
    Vector2 GetClampedPos(Vector2 uiPos) {
        //获取可动范围内
        Vector2 center = Camera.main.WorldToScreenPoint(picture.transform.position);
        Vector2 movementAreaTopRight = center + size / 2 - mainHandleSize / 2;
        Vector2 movementAreaBottomLeft = center - size / 2 + mainHandleSize / 2;

        //将目标位置限制在可动范围内
        uiPos.x = Mathf.Clamp(uiPos.x, movementAreaBottomLeft.x, movementAreaTopRight.x);
        uiPos.y = Mathf.Clamp(uiPos.y, movementAreaBottomLeft.y, movementAreaTopRight.y);

        return uiPos;
    }

    #endregion

    #region 缩放把手们

    Dictionary<PictureSlicerHandle, Vector2> handleDic = new Dictionary<PictureSlicerHandle, Vector2>();

    PictureSlicerHandle currentHandle;

    void InitHandles() {
        //
        handleDic.Add(handle_TopLeft, new Vector2(-1, 1));
        handleDic.Add(handle_TopRight, new Vector2(1, 1));
        handleDic.Add(handle_BottomLeft, new Vector2(-1, -1));
        handleDic.Add(handle_BottomRight, new Vector2(1, -1));

        //把手点击事件
        foreach (var item in handleDic) {
            item.Key.onMouseDown = Handle_OnMouseDown;
            item.Key.onMouseUp = Handle_OnMouseUp;
        }
    }

    //更新把手位置
    void UpdateHandles() {
        if(currentHandle != null) {
            Vector2 targetDir = handleDic[currentHandle];

            //不动点位置
            Vector2 originPos = (Vector2)Camera.main.WorldToScreenPoint(handle_Main.transform.position) - targetDir * mainHandleSize / 2;

            //动点位置
            Vector2 desiredPos = (Vector2)Input.mousePosition + mouseOffset;

            //限制在范围内
            desiredPos = GetClampedHandlePos(desiredPos);

            //修正为方形
            //获取最短边
            Vector2 desiredPosOffset = desiredPos - originPos;
            float shortest = Mathf.Min(Mathf.Abs(desiredPosOffset.x), Mathf.Abs(desiredPosOffset.y));
            shortest = Mathf.Max(shortest, minWidth);
            desiredPos = originPos + targetDir * shortest;

            //如果抵达边缘，改为移动不动点
            //if (!IsWithinArea(desiredPos)) {
            //    originPos = desiredPos - targetDir * (desiredPos - originPos);
            //}

            //修改截取器尺寸位置
            Set(originPos, desiredPos);
        }
    }

    bool IsWithinArea(Vector2 uiPos) {
        //获取可动范围内
        Vector2 center = Camera.main.WorldToScreenPoint(picture.transform.position);
        Vector2 movementAreaTopRight = center + size / 2;
        Vector2 movementAreaBottomLeft = center - size / 2;

        return uiPos.x > movementAreaBottomLeft.x && uiPos.x < movementAreaTopRight.x && uiPos.y > movementAreaBottomLeft.y && uiPos.y < movementAreaTopRight.y;
    }

    //将目标位置限制在可动范围内
    Vector2 GetClampedHandlePos(Vector2 uiPos) {
        //获取可动范围内
        Vector2 center = Camera.main.WorldToScreenPoint(picture.transform.position);
        Vector2 movementAreaTopRight = center + size / 2;
        Vector2 movementAreaBottomLeft = center - size / 2;

        //将目标位置限制在可动范围内
        uiPos.x = Mathf.Clamp(uiPos.x, movementAreaBottomLeft.x, movementAreaTopRight.x);
        uiPos.y = Mathf.Clamp(uiPos.y, movementAreaBottomLeft.y, movementAreaTopRight.y);

        return uiPos;
    }

    //鼠标按下和松开把手
    void Handle_OnMouseDown(PictureSlicerHandle handle) {
        currentHandle = handle;

        mouseOffset = Camera.main.WorldToScreenPoint(handle.transform.position) - Input.mousePosition;
    }
    void Handle_OnMouseUp(PictureSlicerHandle handle) {
        currentHandle = null;
    }

    #endregion

    #region 截取器尺寸

    //截取器尺寸
    Vector2 mainHandleSize {
        get {
            return handle_Main.GetComponent<RectTransform>().sizeDelta;
        }
    }

    //设置截取器尺寸
    void SetSize(Vector2 size) {
        handle_Main.GetComponent<RectTransform>().sizeDelta = size;

        //修改把手尺寸
        foreach (var handle in handleDic.Keys) {
            handle.transform.localScale = size / picture.sizeDelta;
        }
    }

    #endregion

    #region 其他

    //将图片裁剪器填满整张图
    void Fill() {
        //设置尺寸为图片尺寸
        SetSize(size);

        //移动到中心
        TryMove(transform.position);
    }

    //通过两个点设置截取器位置和尺寸
    void Set(Vector2 pos1, Vector2 pos2) {
        //尺寸
        Vector2 size = new Vector2(Mathf.Abs(pos2.x - pos1.x), Mathf.Abs(pos2.y - pos1.y));
        SetSize(size);

        //中心
        Vector2 center = pos1 + (pos2 - pos1) / 2;
        TryMove(center);
    }

    //获取截图数据
    public SliceData GetSliceData() {
        SliceData sliceData = new SliceData();

        Vector2 size2 = Camera.main.ScreenToWorldPoint((Vector2)Camera.main.WorldToScreenPoint(picture.transform.position) + size / 2);
        sliceData.offset = handle_Main.transform.position - picture.transform.position;
        //print(sliceData.offset);
        sliceData.offset /= size2.x;
        //print(sliceData.offset);

        sliceData.zoom = mainHandleSize.x / size.x;

        //print("offset: " + sliceData.offset.ToString("f4") + ", zoom: " + sliceData.zoom);

        return sliceData;
    }

    #endregion
}
