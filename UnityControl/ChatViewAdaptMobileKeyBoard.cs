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

    public float keyboardHeight_local = 0;
    float keyboardHeight_last = 0;
    /// <summary>
    /// 自适应（弹出输入框后整体抬高）的面板的初始位置
    /// </summary>
    private Vector2 _adaptPanelOriginPos;
    private Vector2 _adaptPanelOriginPos2;
    Vector2 screenPos;

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

    private void Update()
    {
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
                    adaptPanelRt.anchoredPosition = _adaptPanelOriginPos;
                    adaptPanelRt2.anchoredPosition = _adaptPanelOriginPos2;
                    if (hideGo) {
                        hideGo.SetActive(true);
                    }
                } else {
                    adaptPanelRt.anchoredPosition = Vector3.up * keyboardHeight;
                    adaptPanelRt2.anchoredPosition = Vector3.up * keyboardHeight;
                    if (hideGo) {
                        hideGo.SetActive(false);
                    }
                }
            }
        }
        else
        {
            keyboardHeight_last = 0;
        }
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
        adaptPanelRt.anchoredPosition = _adaptPanelOriginPos;
        adaptPanelRt2.anchoredPosition = _adaptPanelOriginPos2;
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