using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スタート画面のボタン管理
/// </summary>
public class StartMenu : MonoBehaviour
{
    //Canvas
    public Canvas startMenu;
    public Canvas selectStages;
    //ステージ選択画面
    bool selectstageOpen = false;

    //ボタンリスト
    List<Button> startMenuButtons = new List<Button>();
    List<Button> stageButtons = new List<Button>();

    Dictionary<string, Button> startMenusMap;
    Dictionary<string, Button> stagesMap;

    //今選択しているボタン
    Button selectedStartmenu;
    int startmenuIndex;
    Button selectedStage;
    int selectstageIndex;

    //▶マーカー
    public Image StartMenuSelectArray;
    public Image StageSelectArray;
    public Vector3 ArrowOffset = new Vector3(200,0,0);


    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //初期化
    void Start()
    {

        Time.timeScale = 1;
        selectstageOpen = false;
        selectStages.enabled = false;
        startMenusMap = new Dictionary<string, Button>();
        stagesMap = new Dictionary<string, Button>();
        foreach (var button in startMenu.GetComponentsInChildren<Button>())
        {
            startMenuButtons.Add(button);
            startMenusMap.Add(button.name, button);
        }
        foreach (var button in selectStages.GetComponentsInChildren<Button>())
        {
            stageButtons.Add(button);
            stagesMap.Add(button.name, button);
        }
        //startMenuButtons = startMenu.GetComponentsInChildren<Button>();
        //stagebuttons = select.GetComponentsInChildren<Button>();
        //foreach (var button in startMenuButtons)
        //{
        //    startMenus.Add(button.name, button);
        //}
        //foreach (var button in stagebuttons)
        //{
        //    stages.Add(button.name, button);
        //}
        startmenuIndex = 0;
        selectedStartmenu = startMenuButtons[startmenuIndex];
        SwapSpriteState(selectedStartmenu, ButtonState.Stay);
        StartMenuSelectArray.rectTransform.position = selectedStartmenu.GetComponent<RectTransform>().position - ArrowOffset;
        selectstageIndex = 0;
        selectedStage = stageButtons[selectstageIndex];
        StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
    }

    //選択ボタンが変更しましたがのflag
    bool bottonChanged = false;

    void Update()
    {

        //選択ナヴィゲーション
        //スタート画面
        if (!selectstageOpen)
        {
            if (Input.GetButtonDown("Send"))
            {
                selectedStartmenu.onClick.Invoke();
                SwapSpriteState(selectedStartmenu, ButtonState.Release);
                bottonChanged = false;
            }

            if (Input.GetButtonDown("Right"))
            {
                SwapSpriteState(selectedStartmenu, ButtonState.None);
                startmenuIndex = (startmenuIndex + 1) % startMenuButtons.Count;
                selectedStartmenu = startMenuButtons[startmenuIndex];
                StartMenuSelectArray.rectTransform.position = selectedStartmenu.GetComponent<RectTransform>().position - ArrowOffset;
                SwapSpriteState(selectedStartmenu, ButtonState.Stay);
                bottonChanged = true;
            }
            if (Input.GetButtonDown("Left"))
            {
                SwapSpriteState(selectedStartmenu, ButtonState.None);
                startmenuIndex = (startmenuIndex + startMenuButtons.Count - 1) % startMenuButtons.Count;
                selectedStartmenu = startMenuButtons[startmenuIndex];
                StartMenuSelectArray.rectTransform.position = selectedStartmenu.GetComponent<RectTransform>().position - ArrowOffset;
                SwapSpriteState(selectedStartmenu, ButtonState.Stay);
                bottonChanged = true;
            }
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (bottonChanged) return;
                if (Input.GetAxisRaw("Horizontal") >= 0)
                {
                    SwapSpriteState(selectedStartmenu, ButtonState.None);
                    startmenuIndex = (startmenuIndex + 1) % startMenuButtons.Count;
                    selectedStartmenu = startMenuButtons[startmenuIndex];
                    StartMenuSelectArray.rectTransform.position = selectedStartmenu.GetComponent<RectTransform>().position - ArrowOffset;
                    SwapSpriteState(selectedStartmenu, ButtonState.Stay);
                    bottonChanged = true;
                }
                else
                {
                    SwapSpriteState(selectedStartmenu, ButtonState.None);
                    startmenuIndex = (startmenuIndex + startMenuButtons.Count - 1) % startMenuButtons.Count;
                    selectedStartmenu = startMenuButtons[startmenuIndex];
                    StartMenuSelectArray.rectTransform.position = selectedStartmenu.GetComponent<RectTransform>().position - ArrowOffset;
                    SwapSpriteState(selectedStartmenu, ButtonState.Stay);
                    bottonChanged = true;
                }
            }
            if (bottonChanged && Input.GetAxisRaw("Horizontal") == 0)
            {
                bottonChanged = false;
            }

        }
        //ステージ選択画面
        else
        {
            if (Input.GetButtonDown("Send"))
            {
                selectedStage.onClick.Invoke();
                SwapSpriteState(selectedStage, ButtonState.Release);
            }
            if (Input.GetButtonDown("Cancel"))
            {
                cancelSelect();
                SwapSpriteState(selectedStage, ButtonState.None);
            }
            if (Input.GetButtonDown("Down"))
            {
                SwapSpriteState(selectedStage, ButtonState.None);

                selectNextButton(ref selectedStage,ref selectstageIndex,ref stageButtons);
                //selectstageIndex = (selectstageIndex + 1) % stageButtons.Count;
                //selectedStage = stageButtons[selectstageIndex];
                StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
                SwapSpriteState(selectedStage, ButtonState.Stay);
                bottonChanged = true;
            }
            if (Input.GetButtonDown("Up"))
            {
                SwapSpriteState(selectedStage, ButtonState.None);

                selectLastButton(ref selectedStage, ref selectstageIndex, ref stageButtons);
                //selectstageIndex = (selectstageIndex + stageButtons.Count - 1) % stageButtons.Count;
                //selectedStage = stageButtons[selectstageIndex];
                StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
                SwapSpriteState(selectedStage, ButtonState.Stay);
                bottonChanged = true;
            }
            if (Input.GetButtonDown("Right"))
            {
                SwapSpriteState(selectedStage, ButtonState.None);
                selectstageIndex = (selectstageIndex + 4) % stageButtons.Count;
                selectedStage = stageButtons[selectstageIndex];
                StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
                SwapSpriteState(selectedStage, ButtonState.Stay);
                bottonChanged = true;
            }
            if (Input.GetButtonDown("Left"))
            {
                SwapSpriteState(selectedStage, ButtonState.None);
                selectstageIndex = (selectstageIndex + stageButtons.Count - 4) % stageButtons.Count;
                selectedStage = stageButtons[selectstageIndex];
                StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
                SwapSpriteState(selectedStage, ButtonState.Stay);
                bottonChanged = true;
            }


            if (Input.GetAxisRaw("Vertical") != 0)
            {
                if (bottonChanged) return;
                if (Input.GetAxisRaw("Vertical") <= 0)
                {
                    SwapSpriteState(selectedStage, ButtonState.None);
                    selectstageIndex = (selectstageIndex + 1) % stageButtons.Count;
                    selectedStage = stageButtons[selectstageIndex];
                    StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
                    SwapSpriteState(selectedStage, ButtonState.Stay);
                    bottonChanged = true;
                }
                else
                {
                    SwapSpriteState(selectedStage, ButtonState.None);
                    selectstageIndex = (selectstageIndex + stageButtons.Count - 1) % stageButtons.Count;
                    selectedStage = stageButtons[selectstageIndex];
                    StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
                    SwapSpriteState(selectedStage, ButtonState.Stay);
                    bottonChanged = true;
                }
            }
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (bottonChanged) return;
                if (Input.GetAxisRaw("Horizontal") >= 0)
                {
                    SwapSpriteState(selectedStage, ButtonState.None);
                    selectstageIndex = (selectstageIndex + 4) % stageButtons.Count;
                    selectedStage = stageButtons[selectstageIndex];
                    StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
                    SwapSpriteState(selectedStage, ButtonState.Stay);
                    bottonChanged = true;
                }
                else
                {
                    SwapSpriteState(selectedStage, ButtonState.None);
                    selectstageIndex = (selectstageIndex + stageButtons.Count - 4) % stageButtons.Count;
                    selectedStage = stageButtons[selectstageIndex];
                    StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - ArrowOffset;
                    SwapSpriteState(selectedStage, ButtonState.Stay);
                    bottonChanged = true;
                }
            }

            if (bottonChanged && Input.GetAxisRaw("Vertical") ==0 && Input.GetAxisRaw("Horizontal") == 0)
            {
                bottonChanged = false;
            }
        }
    }
  

    public void enableSelect()
    {
        StartCoroutine(SelectstageClick());
    }
    public void cancelSelect()
    {
        SwapSpriteState(selectedStage, ButtonState.None);
        selectStages.enabled = false;
        startMenu.enabled = true;
        startMenu.GetComponent<CanvasGroup>().alpha = 1;
        selectstageOpen = false;
        selectstageIndex = 0;
    }
   
    IEnumerator SelectstageClick()
    {
        selectedStage = stageButtons[selectstageIndex];
        SwapSpriteState(selectedStage, ButtonState.Stay);
        StageSelectArray.rectTransform.position = selectedStage.GetComponent<RectTransform>().position - new Vector3(200, 0);
        IEnumerator startfadeout = fadeout(startMenu.GetComponent<CanvasGroup>(), 0.2f);
        yield return StartCoroutine(startfadeout);
        selectstageOpen = true;
        selectStages.enabled = true;
        SwapSpriteState(selectedStartmenu, ButtonState.None);
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="fadeout_time"></param>
    /// <returns></returns>
    IEnumerator fadeout(CanvasGroup canvas,float fadeout_time)
    {
        float dealpha = canvas.alpha / fadeout_time * Time.smoothDeltaTime;
       
        while (true)
        {
            canvas.alpha -= dealpha;
            //yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            yield return null;
            if(canvas.alpha <= 0)
            {
                canvas.GetComponent<Canvas>().enabled = false;
                yield break;
            }
        }
    }

    void selectNextButton(ref Button selector, ref int buttonIndex, ref List<Button> buttons)
    {
        buttonIndex = (buttonIndex + 1) % buttons.Count;
        selector = buttons[buttonIndex];
    }
    void selectLastButton(ref Button selector, ref int buttonIndex, ref List<Button> buttons)
    {
        buttonIndex = (buttonIndex + buttons.Count - 1) % buttons.Count;
        selector = buttons[buttonIndex];
    }
    void invokeStages(string stageName)
    {
        stagesMap[stageName].onClick.Invoke();
    }
    void invokeStartMenus(string startMenuButtonName)
    {
        startMenusMap[startMenuButtonName].onClick.Invoke();
    }

    /// <summary>
    /// ステージをロードする
    /// </summary>
    /// <param name="stageName"></param>
    public void LoadStage(string stageName)
    {
        LoadScene.loadScene(stageName);
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      　UnityEngine.Application.Quit();
#endif
    }



    /// <summary>
    /// ボタンハイライト状態
    /// </summary>
    /// <param name="button"></param>
    /// <param name="buttonState"></param>
    public void SwapSpriteState(Button button, ButtonState buttonState)
    {
        switch (buttonState)
        {
            //デフォルト
            case ButtonState.None:
                button.GetComponent<Image>().sprite = button.spriteState.disabledSprite;
                break;
            //選択されて
            case ButtonState.Stay:
                button.GetComponent<Image>().sprite = button.spriteState.highlightedSprite;
                break;
            //押されて
            case ButtonState.Release:
                button.GetComponent<Image>().sprite = button.spriteState.pressedSprite;
                break;
            default:
                break;
        }
        
    }
}
