using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        public void OnPlayClick()
        {
            SceneManager.LoadScene("Final", LoadSceneMode.Single);
        }
    }
}