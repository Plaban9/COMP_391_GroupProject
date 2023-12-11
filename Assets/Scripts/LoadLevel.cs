using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    public void CallSceneManager(string sceneName)
    {
        SceneManager.Instance.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        SceneManager.Instance.QuitRequest();
    }
}
