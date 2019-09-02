﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Canvas startMenu;
    public Canvas selectStages;

    bool selectstageOpen = false;


    List<Button> startMenuButtons = new List<Button>();
    List<Button> stageButtons = new List<Button>();
    //Button[] startMenuButtons;
    //Button[] stagebuttons;
    Dictionary<string, Button> startMenusMap;
    Dictionary<string, Button> stagesMap;

    Button selectStartmenu;
    int startmenuIndex;
    Button selectStage;
    int selectstageIndex;
    public Image StartMenuSelectArray;
    public Image StageSelectArray;

    

    // Start is called before the first frame update
    void Start()
    {
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
        selectStartmenu = startMenuButtons[startmenuIndex];
        StartMenuSelectArray.rectTransform.position = selectStartmenu.GetComponent<RectTransform>().position - new Vector3(300, 0);
        selectstageIndex = 0;
        selectStage = stageButtons[selectstageIndex];
        StageSelectArray.rectTransform.position = selectStage.GetComponent<RectTransform>().position - new Vector3(200, 0);
    }


    bool changed = false;
    // Update is called once per frame
    void Update()
    {
        if (!selectstageOpen)
        {
            if (Input.GetButtonDown("Send"))
            {
                selectStartmenu.onClick.Invoke();
                changed = false;
            }

            if (Input.GetButtonDown("Right"))
            {
                startmenuIndex = (startmenuIndex + 1) % startMenuButtons.Count;
                selectStartmenu = startMenuButtons[startmenuIndex];
                StartMenuSelectArray.rectTransform.position = selectStartmenu.GetComponent<RectTransform>().position - new Vector3(300, 0);
            }
            if (Input.GetButtonDown("Left"))
            {
                startmenuIndex = (startmenuIndex + startMenuButtons.Count - 1) % startMenuButtons.Count;
                selectStartmenu = startMenuButtons[startmenuIndex];
                StartMenuSelectArray.rectTransform.position = selectStartmenu.GetComponent<RectTransform>().position - new Vector3(300, 0);
            }
            if (Input.GetAxisRaw("Right") != 0)
            {
                if (changed) return;
                if (Input.GetAxisRaw("Right") >= 0)
                {
                    startmenuIndex = (startmenuIndex + 1) % startMenuButtons.Count;
                    selectStartmenu = startMenuButtons[startmenuIndex];
                    StartMenuSelectArray.rectTransform.position = selectStartmenu.GetComponent<RectTransform>().position - new Vector3(300, 0);
                    changed = true;
                }
                else
                {
                    startmenuIndex = (startmenuIndex + startMenuButtons.Count - 1) % startMenuButtons.Count;
                    selectStartmenu = startMenuButtons[startmenuIndex];
                    StartMenuSelectArray.rectTransform.position = selectStartmenu.GetComponent<RectTransform>().position - new Vector3(300, 0);
                    changed = true;
                }
            }
            if (changed && Input.GetAxisRaw("Right") == 0)
            {
                changed = false;
            }

        }
        else
        {
            if (Input.GetButtonDown("Send"))
            {
                selectStage.onClick.Invoke();
            }
            if (Input.GetButtonDown("Cancel"))
            {
                cancelSelect();
            }
            if (Input.GetButtonDown("Down"))
            {
                selectstageIndex = (selectstageIndex + 1) % stageButtons.Count;
                selectStage = stageButtons[selectstageIndex];
                StageSelectArray.rectTransform.position = selectStage.GetComponent<RectTransform>().position - new Vector3(200, 0);
            }
            if (Input.GetButtonDown("Up"))
            {
                selectstageIndex = (selectstageIndex + stageButtons.Count - 1) % stageButtons.Count;
                selectStage = stageButtons[selectstageIndex];
                StageSelectArray.rectTransform.position = selectStage.GetComponent<RectTransform>().position - new Vector3(200, 0);
            }

            if (Input.GetAxisRaw("Down") != 0)
            {
                if (changed) return;
                if (Input.GetAxisRaw("Down") <= 0)
                {
                    selectstageIndex = (selectstageIndex + 1) % stageButtons.Count;
                    selectStage = stageButtons[selectstageIndex];
                    StageSelectArray.rectTransform.position = selectStage.GetComponent<RectTransform>().position - new Vector3(200, 0);
                    changed = true;
                }
                else
                {
                    selectstageIndex = (selectstageIndex + stageButtons.Count - 1) % stageButtons.Count;
                    selectStage = stageButtons[selectstageIndex];
                    StageSelectArray.rectTransform.position = selectStage.GetComponent<RectTransform>().position - new Vector3(200, 0);
                    changed = true;
                }
            }
            if (changed && Input.GetAxisRaw("Down") == 0)
            {
                changed = false;
            }
        }
    }
  

    public void enableSelect()
    {
        StartCoroutine(SelectstageClick());
    }
    public void cancelSelect()
    {
        selectStages.enabled = false;
        startMenu.enabled = true;
        startMenu.GetComponent<CanvasGroup>().alpha = 1;
        selectstageOpen = false;
    }
   
    IEnumerator SelectstageClick()
    {
        IEnumerator startfadeout = fadeout(startMenu.GetComponent<CanvasGroup>(), 0.2f);
        yield return StartCoroutine(startfadeout);
        selectstageOpen = true;
        selectStages.enabled = true;
    }


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

    void selectNextButton(Button selector,int buttonIndex,List<Button> buttons)
    {
        buttonIndex = (buttonIndex + 1) % buttons.Count;
        selector = buttons[buttonIndex];
    }
    void selectLastButton(Button selector, int buttonIndex, List<Button> buttons)
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

}