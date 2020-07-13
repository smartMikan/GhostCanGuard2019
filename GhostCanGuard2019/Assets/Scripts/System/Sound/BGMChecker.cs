using UnityEngine;

/// <summary>
/// BGMInstanceを確報
/// </summary>
public class BGMChecker : MonoBehaviour
{
    public GameObject BGM;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.FindObjectOfType<BGMManager>() == null)
        {
            Instantiate(BGM);
        }
    }

    
}
