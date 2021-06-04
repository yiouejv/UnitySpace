using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FireBird;


namespace FireBird {

/// <summary>
/// 移动设备输入框的自适应组件
/// </summary>
public class ChatViewAdaptMobileKeyBoard : MonoBehaviour
{
    public InputField _inputField;
    public RectTransform adaptPanelRt;
    public RectTransform adaptPanelRt2;
    public GameObject hideGo;
    public ExtButton sendBtn;
    public int defaultToTop = 300;
    public int hideGoHeight = 170;
    private bool waitMoving = false;

    public float keyboardHeight_local = 0;
    float keyboardHeight_last = 0;
    /// <summary>
    /// 自适应（弹出输入框后整体抬高）的面板的初始位置
    /// </summary>
    private Vector2 _adaptPanelOriginPos;
    private Vector2 _adaptPanelOriginPos2;

    public static ChatViewAdaptMobileKeyBoard Create(GameObject attachRoot, InputField inputField)
    {
        ChatViewAdaptMobileKeyBoard instance = null;
        instance = attachRoot.AddComponent<ChatViewAdaptMobileKeyBoard>();
        instance._inputField = inputField;

        return instance;
    }

    private void Start()
    {
        _inputField.onEndEdit.AddListener(OnEndEdit);
        _inputField.onValueChanged.AddListener(OnValueChanged);
        _adaptPanelOriginPos = adaptPanelRt.anchoredPosition;
        _adaptPanelOriginPos2 = adaptPanelRt2.anchoredPosition;
        _inputField.keyboardType = TouchScreenKeyboardType.Default;
        _inputField.shouldHideMobileInput = true;
    }

    float startTime = 0;
    float startY = 0;
    float endY = 0;
    public float speed = 0.5f;
    private void Update()
    {
        if (waitMoving) {
            float lerpValue = Mathf.Lerp(startY, endY, (Time.time-startTime) * speed);
            Vector3 targetVec3 = new Vector3(0, lerpValue, 0);
            adaptPanelRt.anchoredPosition = targetVec3;
            adaptPanelRt2.anchoredPosition = targetVec3;
            if (floatEqual(endY, lerpValue)) {
                waitMoving = false;
            }
            return;
        }

        if (_inputField.isFocused)
        {
#if UNITY_EDITOR
            keyboardHeight_local = defaultToTop;
#elif UNITY_ANDROID
            keyboardHeight_local = GetKeyboardHeightAndroid() - hideGoHeight;
#elif UNITY_IOS
            keyboardHeight_local =  GetKeyboardHeightIOS() - (hideGoHeight / 2);
#endif
            if (keyboardHeight_last != keyboardHeight_local)
            {
                keyboardHeight_last = keyboardHeight_local;

                float keyboardHeight = keyboardHeight_local * (float)CanvasScaler.designHeight / (float)Screen.height;
                if (keyboardHeight <= 0) {
                    if (!floatEqual(adaptPanelRt.anchoredPosition.y, _adaptPanelOriginPos.y)) {
                        startY = endY;
                        endY = _adaptPanelOriginPos.y;
                        startTime = Time.time;
                        waitMoving = true;
                    }

                    if (hideGo) {
                        hideGo.SetActive(true);
                    }
                } else {
                    if (!floatEqual(adaptPanelRt.anchoredPosition.y, keyboardHeight)) {
                        startY = _adaptPanelOriginPos.y;
                        endY = keyboardHeight;
                        startTime = Time.time;
                        waitMoving = true;
                    }

                    if (hideGo) {
                        hideGo.SetActive(false);
                    }
                }
            }
        }
        else
        {
            keyboardHeight_last = 0;
            if (!floatEqual(adaptPanelRt.anchoredPosition.y, _adaptPanelOriginPos.y)) {
                startY = endY;
                endY = _adaptPanelOriginPos.y;
                startTime = Time.time;
                waitMoving = true;
            }
        }
    }

    private bool floatEqual(float x, float y) {
        float n = x - y;
        return n >= -0.000001 && n <= 0.000001;
    }

    private void OnValueChanged(string arg0) { }

    /// <summary>
    /// 结束编辑事件，TouchScreenKeyboard.isFocused为false的时候
    /// </summary>
    /// <param name="currentInputString"></param>
    private void OnEndEdit(string currentInputString)
    {
        StartCoroutine(DelayMoveBack());
    }


    IEnumerator DelayMoveBack()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        if (sendBtn.gameObject == EventSystem.current.currentSelectedGameObject)
        {
            sendBtn.onClick.Invoke();
        }
        if (hideGo) hideGo.SetActive(true);
    }

#if UNITY_ANDROID
    /// <summary>
    /// 获取安卓平台上键盘的高度
    /// </summary>
    /// <returns></returns>
    public int GetKeyboardHeightAndroid()
    {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
 
            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);
                return Screen.height - Rct.Call<int>("height");
            }
        }
    }
#endif


#if UNITY_IOS
    public float GetKeyboardHeightIOS()
    {
        return (float)TouchScreenKeyboard.area.height / 2;
    }
#endif

}

}