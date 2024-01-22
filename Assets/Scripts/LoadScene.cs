using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public void LoadScene(string Game)
    {
        SceneManager.LoadScene(Game)
    }

    public void LoadScene(string menu)
    {
        SceneManager.LoadScene(menu);
    }

    public void LoadScene(string Controls)
    {
        SceneManager.LoadScene(Controls)
    }

    public void QuitGame ()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}