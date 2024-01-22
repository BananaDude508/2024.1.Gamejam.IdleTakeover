using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour

public void LoadScene(string nextScene)
{
  SceneManager.LoadScene(nextScene);
}