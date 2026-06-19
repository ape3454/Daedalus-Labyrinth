using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuHandler : MonoBehaviour
{
    UIDocument uiDocument;
    GameObject loading;

    private void Awake()
    {
        loading = GameObject.Find("Loading");
    }

    private void Start()
    {
        loading.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("this");
        loading.SetActive(true);
        SceneManager.LoadSceneAsync("MainScene");
    }
}
