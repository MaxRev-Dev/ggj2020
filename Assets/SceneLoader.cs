using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{  
    public void OnPlayClick()
    {
        SceneManager.LoadScene("Final", LoadSceneMode.Single);
    }
}
