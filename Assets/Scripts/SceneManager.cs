using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private static SceneManager instance;

    public static SceneManager Instance { get => instance; private set => instance = value; }

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Animator _levelObjAnimation;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadLevelCoroutine(2f, sceneName));
    }

    private IEnumerator LoadLevelCoroutine(float delay, string sceneName)
    {
        _animator.SetTrigger("exit");
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitRequest()
    {
        StartCoroutine(nameof(QuitCoroutine), 2f);
    }


    private IEnumerator QuitCoroutine(float delay)
    {
        _animator.SetTrigger("exit");
        yield return new WaitForSeconds(delay);
        Application.Quit();
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);

        _animator = GameObject.Find("Panel").GetComponent<Animator>();

        if (scene.name.Equals("A01L01"))
        {
            try
            {
                _levelObjAnimation = FindObjectOfType<FontHandler>().GetComponent<Animator>();
                StartCoroutine(LevelLoadObjAnimation(2f));
            }
            catch
            {

            }
        }
    }

    IEnumerator LevelLoadObjAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_levelObjAnimation != null)
        {
            _levelObjAnimation.SetTrigger("title");
        }
    }

}
