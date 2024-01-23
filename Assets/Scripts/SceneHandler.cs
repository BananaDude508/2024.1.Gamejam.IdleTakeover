using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public void LoadScene(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
    public void Quit()
    {
        Application.Quit();
    }
}