using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public void LoadScene(string StartGame)
    {
        SceneManager.LoadScene(StartGame);
    }
}