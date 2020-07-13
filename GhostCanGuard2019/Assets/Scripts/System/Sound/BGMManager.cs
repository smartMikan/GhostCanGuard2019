using UnityEngine;
/// <summary>
/// BGMManager
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    //ヴォリューム
    public float Volume = 0.5f;

    //コンポーネント
    AudioSource BGM;
   
    void Start()
    {
        BGM = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);

    }

   
    void Update()
    {
        if (LoadScene.GetCurrentSceneName() != "TitleScene" && LoadScene.GetCurrentSceneName() != "Video")
        {
            if (GameManager.Instance.gameObject.GetComponent<StopBGMManager>().BGM.isPlaying)
            {
                if (BGM.isPlaying)
                    BGM.Pause();
            }
            else
            {
                if (!BGM.isPlaying)
                    BGM.Play();
            }
        }
        if (LoadScene.GetCurrentSceneName() == "Video")
        {
            if (BGM.isPlaying)
                BGM.Pause();
        }

        //ヴォリュームを変わる
        if (BGM.volume != Volume)
        {
            BGM.volume = Volume;
        }
    }
}
