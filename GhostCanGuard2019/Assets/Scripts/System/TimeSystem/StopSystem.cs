using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopSystem : SingletonMonoBehavior<StopSystem>
{

    public enum PauseState
    {
        Normal,
        ObserverMode,
        DirectionSelect,
        DescriptionOpen,
        DescriptionClose,
        SystemPause,
        Resume
    }


    public bool canStop = false;

    [Range(0,1)][Header("観察modeのTimeScale")]
    public float ObserverTimeScale = 0.1f;

    /// <summary>
    /// Highlight
    /// </summary>
    public LayerMask mask;
    OutLineCamera outlineCamera;
    [SerializeField]
    private Color HighlightColor = Color.green;
    public Outline.Mode HighlightMode = Outline.Mode.OutlineAll;
    [Range(0f,10f)]
    public float HighlightWidth = 5f;
    public GameObject selectedObject;
    private GameObject outlineObject;


    /// <summary>
    /// cursor
    /// </summary>
    [SerializeField]
    float speed = 10f;
    public GameObject cursor;
    //public Slider AimSlider;
    public Sprite cursor_first;
    public List<Sprite> cursor_second;


    /// <summary>
    /// PauseState Flag
    /// </summary>
    public bool SecondPhase { get; private set; }
    public bool DescriptionPhase { get; private set; }
    public bool IfSystemPause { get; private set; }
    public PauseState currentstate;
    private float currentTimescale;
    public bool stopped { get; private set; }

    bool getTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        currentTimescale = 1f;
        canStop = false;
        SecondPhase = false;
        DescriptionPhase = false;
        outlineCamera = Camera.main.GetComponent<OutLineCamera>();
        outlineCamera.enabled = false;
        cursor.SetActive (false);
        stopped = false;
        cursor.GetComponent<Image>().sprite = cursor_first;
    }

    // Update is called once per frame
    void Update()
    {
        if (IfSystemPause) return;
        if((Input.GetAxis("Stop") == 0))
        {
            getTrigger = false;
        }
        if (canStop)
        {
            if ((Input.GetButtonDown("Stop")||Input.GetAxis("Stop")>=1)&& !SecondPhase)
            {
                if (getTrigger) return;
                if(currentstate == PauseState.ObserverMode)
                {
                    gamestop(PauseState.Normal);
                    getTrigger = true;
                }
                else if(currentstate == PauseState.Normal)
                {
                    gamestop(PauseState.ObserverMode);
                    getTrigger = true;
                }
            }
        }
       
        if (stopped)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 move = new Vector2(horizontal, vertical);
            if(cursor.activeSelf)
                cursor.transform.Translate(move * speed);
            DragRangeLimit(cursor.transform);
            if (!SecondPhase && !DescriptionPhase)
            {
                getObjectAtPosition();          //selectedObjectの変更、SecondPhaseには変更しません
            }
            if (outlineObject)
            {
                if (outlineObject.GetComponent<Outline>() != null && outlineObject != selectedObject)
                {
                    outlineObject.GetComponent<Outline>().OutlineWidth = 0;                 //アウトライを消す
                }
                if(outlineObject==selectedObject)
                {
                    if (selectedObject.GetComponent<Outline>() == null)
                        addSingleOutline(HighlightMode, HighlightColor, HighlightWidth);
                    else
                    {
                        outlineObject.GetComponent<Outline>().OutlineWidth = HighlightWidth;
                    }
                }
            }
            outlineObject = selectedObject;
            PrepareGimmick(selectedObject);
        }
        //if (SecondPhase && !AimSlider.gameObject.activeSelf)
        //{
        //    AimSlider.gameObject.SetActive(true);
        //}
       
    }

    public void gamestop(PauseState state)
    {
        switch (state)
        {
            case PauseState.Normal:
                gameStopEnd();
                break;
            case PauseState.ObserverMode:
                gameStopStart();
                break;
            case PauseState.DirectionSelect:
                changeToSecondPhase();
                break;
            case PauseState.DescriptionOpen:
                DescriptionOn();
                break;
            case PauseState.DescriptionClose:
                DescriptionClose();
                break;
            case PauseState.SystemPause:
                SystemPause();
                break;
            case PauseState.Resume:
                Resume();
                break;
            default:
                break;
        }
       
    }
    

    void gameStopStart()
    {
        AudioController.PlaySnd("A7_FingerSnapping",Camera.main.transform.position,1f);
        cursor.SetActive(true);
        cursor.transform.position = Camera.main.WorldToScreenPoint(GameManager.Instance.pc.gameObject.transform.position);
        Time.timeScale = ObserverTimeScale;
        currentTimescale = ObserverTimeScale;
        outlineCamera.enabled = true;

        GameManager.Instance.pc.playerAnim.SetGimmickAnimation(GimmickAnimation.Hold);
        //PlayerAnimationController.Instance.SetAnimatorValue(SetPAnimator.Hold);
        stopped = true;
        GameManager.Instance.pc.CanPlayerMove = false;
        currentstate = PauseState.ObserverMode;
    }
    void gameStopEnd()
    {
        Time.timeScale = 1f;
        currentTimescale = 1f;
        cursor.SetActive(false);
        outlineCamera.enabled = false;
        GameManager.Instance.pc.playerAnim.SetGimmickAnimation(GimmickAnimation.Push);
        //PlayerAnimationController.Instance.SetAnimatorValue(SetPAnimator.Push);
        stopped = false;
        GameManager.Instance.pc.CanPlayerMove = true;
        if (outlineObject && outlineObject.GetComponent<Outline>() != null)         //アウトライを消す
        {
            outlineObject.GetComponent<Outline>().OutlineWidth = 0; 
        }
        outlineObject = null;
        SecondPhase = false;
        StopCoroutine(changesprite());
        cursor.GetComponent<Image>().sprite = cursor_first;
        selectedObject = null;
        InputManager.Instance.ClearCurrentButton();
        currentstate = PauseState.Normal;
    }

    void DescriptionOn()
    {
        canStop = false;
        Time.timeScale = 0f;
        currentTimescale = 0f;
        DescriptionPhase = true;
        cursor.SetActive(false);
        //outlineCamera.enabled = false;
        currentstate = PauseState.DescriptionOpen;
    }
    void DescriptionClose()
    {
        canStop = true;
        cursor.SetActive(true);
        //outlineCamera.enabled = true;
        Time.timeScale = ObserverTimeScale;
        currentTimescale = ObserverTimeScale;
        DescriptionPhase = false;
        currentstate = PauseState.ObserverMode;
    }
    void SystemPause()
    {
        IfSystemPause = true;
        canStop = false;
        //cursor.SetActive(false);
        //outlineCamera.enabled = false;
        Time.timeScale = 0f;
        InputManager.Instance.ClearCurrentButton();
    }

    void Resume()
    {
        if (!IfSystemPause) return;
        IfSystemPause = false;
        canStop = true;
        Time.timeScale = currentTimescale;
    }

    void changeToSecondPhase()
    {
        cursor.SetActive(true);
        SecondPhase = true;
        currentstate = PauseState.DirectionSelect;
        StartCoroutine(changesprite());
    }

    
    public void getObjectAtPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(cursor.transform.position);

        RaycastHit[] hit = Physics.RaycastAll(ray, 100);
        
        for (int i = 0; i < hit.Length; i++)
        {
            
                var HitObject = hit[i].collider.gameObject;
                if(HitObject.tag == "Player" && HitObject.GetComponent<GimmickBase>() != null)
                {
                    selectedObject = HitObject;
                    break;
                }
                else if(HitObject.tag == "Gimmik" && HitObject.GetComponent<GimmickBase>() != null)
                {
                    selectedObject = HitObject;
                    break;
                }
                else
                {
                    selectedObject = null;
                }
        }     

        //if (Physics.Raycast(ray, out hit,mask) && hit.collider.gameObject != selectedObject)
        //{
        //    if (hit.collider.tag == "Gimmik")
        //    {
               
        //        //Debug.Log(hit.collider.gameObject.name);
        //        selectedObject = hit.collider.gameObject;
        //    }
        //    else if (hit.collider.tag == "Player")
        //    {
        //        selectedObject = hit.collider.gameObject;
        //    }
        //    else
        //    {
        //        //if (selectedObject != null)
        //            //selectedObject.GetComponent<GimmickBase>().GimmickUIsOnOff(false);
        //        selectedObject = null;
        //    }
        //}
    }

    


    public Vector3 getCursorWorldPosition()
    {

        //Ray ray = Camera.main.ScreenPointToRay(cursor.transform.position);
        //RaycastHit hit;
        //Physics.Raycast(ray, out hit);
        //return new Vector3(hit.point.x,0,hit.point.y);
        Vector3 pos =Camera.main.ScreenToWorldPoint(new Vector3(cursor.transform.position.x, cursor.transform.position.y,Camera.main.transform.position.y));
        //Debug.Log("cursor position = " + pos);
        return pos;
    }
    public Vector3 getCursorScreenPosition()
    {
        return cursor.transform.position;
    }


    /// <summary>
    /// 物を画面内に制限する
    /// </summary>
    /// <param name="tra"></param>
    public void DragRangeLimit(Transform tra)
    {
        var pos = tra.GetComponent<RectTransform>();
        float x = Mathf.Clamp(pos.position.x, 0, Screen.width*0.99f);
        float y = Mathf.Clamp(pos.position.y, Screen.height * 0.01f, Screen.height);
        pos.position = new Vector2(x, y);
    }

    private void PrepareGimmick(GameObject gimmick)
    {

        if (gimmick == null)
        {
            GimmickManager.Instance.ClearGimmick();
            return;
        }
        // UI展開
        //gimmick.GetComponent<GimmickBase>().GimmickUIsOnOff(true);
        // コントローラー入力待ち状態に送る
        if (gimmick.tag == "Gimmik"　|| gimmick.tag == "Player")
        {
            gimmick.GetComponent<GimmickBase>().ClickGimmick(); 
        }
        
    }
    
   

    IEnumerator changesprite()
    {
        int spritenumber = 0;
        Sprite selectedSprite = cursor_second[spritenumber];
        while (true)
        {
            if (!SecondPhase) break;
            spritenumber++;
            spritenumber = spritenumber % cursor_second.Count;
            selectedSprite = cursor_second[spritenumber];
            cursor.GetComponent<Image>().sprite = selectedSprite;
            yield return new WaitForSecondsRealtime(0.125f);
        }
        cursor.GetComponent<Image>().sprite = cursor_first;
    }

    public void addSingleOutline(Outline.Mode mode,Color color,float width)
    {
        if (width >= 10) width = 10;
        if (width <= 0) width = 0;
        var outline = outlineObject.AddComponent<Outline>();
        outline.OutlineMode = mode;
        outline.OutlineColor = color;
        outline.OutlineWidth = width;
    }
    private void hideOutline(Outline outline)
    {
        outline.OutlineWidth = 0;
    }
    public void clearselectobj()
    {
        selectedObject = null;
    }

}
