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
    public void OpenGDD()
    {
        Application.OpenURL("https://docs.google.com/document/d/1kXETfnwbmXMCKj2XOXq0WfkKFSMS_4LuKpK8HVK4_MY/edit?usp=sharing");
    }
}