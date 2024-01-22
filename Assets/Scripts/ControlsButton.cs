using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlsButton : MonoBehaviour
{
    public void LoadScene(string Controls)
    {
        SceneManager.LoadScene(Controls)
    }
}