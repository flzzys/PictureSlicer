using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//截取数据
public struct SliceData {
    public Vector2 offset;
    public float zoom;

    public override string ToString() {
        return offset + ", " + zoom;
    }
}

public class PictureSlicer : MonoBehaviour {
    public Image picture;
    public SlicedPictureDisplay slicedPictureDisplay;

    //图片尺寸
    Vector2 pictureRectSize { get { return picture.rectTransform.rect.size; } }
    Vector2 pictureSize { get { return new Vector2(picture.sprite.texture.width, picture.sprite.texture.height); } }

    //截取器尺寸
    Vector2 mainHandleSize { get { return handle_Main.GetComponent<RectTransform>().rect.size; } }

    //把手
    public PictureSlicerHandle handle_Main;
    public PictureSlicerHandle handle_TopLeft;
    public PictureSlicerHandle handle_TopRight;
    public PictureSlicerHandle handle_BottomLeft;
    public PictureSlicerHandle handle_BottomRight;

    //最小截图器宽度
    public float minWidth = 200;
    public float minHeight { get { return minWidth / ratio; } }

    [Header("宽高比")]
    public float ratio = 1;

    Vector2 canvasMovementRate;

    Vector2 mouseStartingPos;

    Vector3 prevMousePos;

    private void Awake() {
        InitMoving();
        InitHandles();

        Center();
    }

    private void Start() {
        slicedPictureDisplay.Set(GetSliceData());
    }

    private void Update() {
        if(currentHandle != null || moving) {
            if(Input.mousePosition != prevMousePos) {
                //鼠标相对点击位置的位移
                Vector2 mouseOffset = (Vector2)Input.mousePosition - mouseStartingPos;

                //鼠标位移相对屏幕的移动率
                Vector2 mouseMovementRate = new Vector2(mouseOffset.x / Screen.width, mouseOffset.y / Screen.height);

                //Canvas尺寸
                Vector2 canvasSize = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.size;

                //鼠标移动率换算到该Canvas移动率
                canvasMovementRate = new Vector2(mouseMovementRate.x * canvasSize.x, mouseMovementRate.y * canvasSize.y);

                prevMousePos = Input.mousePosition;

                if(slicedPictureDisplay)
                    slicedPictureDisplay.Set(GetSliceData());
            }
        }

        UpdateMoving();
        UpdateHandles();
    }

    #region 截取器移动

    //移动中
    bool moving;

    Vector2 selectorStartingPos;

    //初始化移动数据
    void InitMoving() {
        handle_Main.onMouseDown = handle => {
            moving = true;

            mouseStartingPos = Input.mousePosition;
            selectorStartingPos = handle_Main.GetComponent<RectTransform>().anchoredPosition;
        };
        handle_Main.onMouseUp = handle => {
            moving = false;
        };
    }

    //更新移动
    void UpdateMoving() {
        if (moving) {
            Vector2 desiredPos = selectorStartingPos + canvasMovementRate;
            //移动尺寸操控器
            TryMove(desiredPos);
        }
    }

    //尝试移动，不会移动到范围外
    void TryMove(Vector2 uiPos) {
        //将目标位置限制在可动范围内
        uiPos = GetClampedPos(uiPos);

        //移动
        handle_Main.GetComponent<RectTransform>().anchoredPosition = uiPos;
    }

    //将目标位置限制在可动范围内
    Vector2 GetClampedPos(Vector2 uiPos) {
        //获取可动范围内
        Vector2 movementAreaTopRight = pictureRectSize / 2 - mainHandleSize / 2;
        Vector2 movementAreaBottomLeft = -pictureRectSize / 2 + mainHandleSize / 2;

        //将目标位置限制在可动范围内
        uiPos.x = Mathf.Clamp(uiPos.x, movementAreaBottomLeft.x, movementAreaTopRight.x);
        uiPos.y = Mathf.Clamp(uiPos.y, movementAreaBottomLeft.y, movementAreaTopRight.y);

        return uiPos;
    }

    #endregion

    #region 缩放把手们

    Dictionary<PictureSlicerHandle, Vector2> handleDic = new Dictionary<PictureSlicerHandle, Vector2>();

    PictureSlicerHandle currentHandle;

    Vector2 handleStartingPos;

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

    //鼠标按下和松开把手
    void Handle_OnMouseDown(PictureSlicerHandle handle) {
        currentHandle = handle;

        mouseStartingPos = Input.mousePosition;

        Vector2 targetDir = handleDic[currentHandle];
        handleStartingPos = handle_Main.GetComponent<RectTransform>().anchoredPosition + targetDir * mainHandleSize / 2;
    }
    void Handle_OnMouseUp(PictureSlicerHandle handle) {
        currentHandle = null;
    }

    //更新把手位置
    void UpdateHandles() {
        if(currentHandle != null) {
            Vector2 targetDir = handleDic[currentHandle];

            //相对的固定把手位置
            Vector2 originPos = handle_Main.GetComponent<RectTransform>().anchoredPosition - targetDir * mainHandleSize / 2;

            //移动后的把手位置
            Vector2 desiredPos = handleStartingPos + canvasMovementRate;

            //移动到了固定把手背后，忽略
            Vector2 handlePos = handle_Main.GetComponent<RectTransform>().anchoredPosition;

            //限制范围
            if(desiredPos.x > pictureRectSize.x / 2 * targetDir.x || (desiredPos.x - originPos.x) < minWidth) {
                return;
            }

            //if (desiredPos.x * targetDir.x < originPos.x || (desiredPos.y - handlePos.y) * targetDir.y < (originPos.y - handlePos.y)) {
            //    return;
            //}

            print(desiredPos.y + ", " + originPos.y);


            //修正宽高比
            Vector2 desiredPosOffset = desiredPos - originPos;
            float desiredRatio = Mathf.Abs(desiredPosOffset.x / desiredPosOffset.y);

            float width;
            float height;

            if (desiredRatio < ratio) {
                height = Mathf.Abs(desiredPosOffset.y);
                width = height * ratio;

            } else {
                width = Mathf.Abs(desiredPosOffset.x);
                height = width / ratio;
            }
            desiredPos = new Vector2(originPos.x + targetDir.x * width, originPos.y + targetDir.y * height);

            //限制在范围内
            //desiredPos = GetClampedHandlePos(desiredPos);

            //修改截取器尺寸位置
            Set(originPos, desiredPos);
        }
    }

    bool IsWithinArea(Vector2 uiPos) {
        //获取可动范围内
        Vector2 center = Camera.main.WorldToScreenPoint(picture.transform.position);
        Vector2 movementAreaTopRight = center + pictureRectSize / 2;
        Vector2 movementAreaBottomLeft = center - pictureRectSize / 2;

        return uiPos.x > movementAreaBottomLeft.x && uiPos.x < movementAreaTopRight.x && uiPos.y > movementAreaBottomLeft.y && uiPos.y < movementAreaTopRight.y;
    }

    //将目标位置限制在可动范围内
    Vector2 GetClampedHandlePos(Vector2 uiPos) {
        Vector2 targetDir = handleDic[currentHandle];

        //相对的固定把手位置
        Vector2 originPos = handle_Main.GetComponent<RectTransform>().anchoredPosition - targetDir * mainHandleSize / 2;

        //获取目前尺寸
        Vector2 movementAreaTopRight = pictureRectSize / 2;
        Vector2 desiredSize = new Vector2(Mathf.Abs(uiPos.x - originPos.x), Mathf.Abs(uiPos.y - originPos.y));

        //限制最小尺寸
        desiredSize.x = Mathf.Max(minWidth, desiredSize.x);
        desiredSize.y = Mathf.Max(minHeight, desiredSize.y);

        uiPos = originPos + desiredSize * targetDir;

        //将目标位置限制在可动范围内
        //XY都超标，先重新设置X，然后重置Y
        if (Mathf.Abs(uiPos.x) - movementAreaTopRight.x > 0 && Mathf.Abs(uiPos.y) - movementAreaTopRight.y > 0) {
            desiredSize.x = Mathf.Min(desiredSize.x, Mathf.Abs(pictureRectSize.x / 2 * targetDir.x - originPos.x));
            desiredSize.y = desiredSize.x / ratio;

            if(desiredSize.y > movementAreaTopRight.y) {
                desiredSize.y = Mathf.Min(desiredSize.y, Mathf.Abs(pictureRectSize.y / 2 * targetDir.y - originPos.y));
                desiredSize.x = desiredSize.y * ratio;
            }

            uiPos = originPos + desiredSize * targetDir;
        }
        //X超标，重新设置X
        else if (Mathf.Abs(uiPos.x) - movementAreaTopRight.x > 0) {
            desiredSize.x = Mathf.Min(desiredSize.x, Mathf.Abs(pictureRectSize.x / 2 * targetDir.x - originPos.x));
            desiredSize.y = desiredSize.x / ratio;

            uiPos = originPos + desiredSize * targetDir;
        }
        //Y超标，重新设置Y
        else if (Mathf.Abs(uiPos.y) - movementAreaTopRight.y > 0) {
            desiredSize.y = Mathf.Min(desiredSize.y, Mathf.Abs(pictureRectSize.y / 2 * targetDir.y - originPos.y));
            desiredSize.x = desiredSize.y * ratio;

            uiPos = originPos + desiredSize * targetDir;
        }

        return uiPos;
    }

    #endregion

    #region 截取器尺寸

    //设置截取器尺寸
    void SetSize(Vector2 size) {
        if(size.x / size.y != ratio) {
            size.x = size.y * ratio;
        }

        handle_Main.GetComponent<RectTransform>().sizeDelta = size;
    }

    #endregion

    #region 其他

    //居中
    public void Center() {
        //设置尺寸为图片尺寸
        SetSize(pictureRectSize / 2);

        //移动到中心
        TryMove(Vector2.zero);
    }

    //将图片裁剪器填满整张图
    void Fill() {
        //设置尺寸为图片尺寸
        SetSize(pictureRectSize);

        //移动到中心
        TryMove(Camera.main.WorldToScreenPoint(transform.position));
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

        //截图器位置
        Vector2 pos = handle_Main.GetComponent<RectTransform>().anchoredPosition;

        //位移
        sliceData.offset = pos / pictureRectSize.x;

        sliceData.offset *= 2;

        sliceData.offset.y *= -1;
        sliceData.offset = (sliceData.offset + Vector2.one) / 2;
        sliceData.offset *= pictureSize.x;

        //缩放
        if(ratio < 1) {
            sliceData.zoom = pictureRectSize.y / mainHandleSize.y;
        } else {
            sliceData.zoom = pictureRectSize.x / mainHandleSize.x;
        }

        return sliceData;
    }

    #endregion
}
