using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField] private Button sceneTest1Button;
    [SerializeField] private Button sceneTest2Button;
    [SerializeField] private Button exitButton;
    
    [Header("Scene Names")]
    [SerializeField] private string sceneTest1Name = "Scene_Test1";
    [SerializeField] private string sceneTest2Name = "Scene_Test2";
    
    void Start()
    {
        // Set up button listeners
        if (sceneTest1Button != null)
        {
            sceneTest1Button.onClick.AddListener(OnSceneTest1ButtonClicked);
        }
        else
        {
            Debug.LogWarning("Scene Test 1 button not assigned in Menu!");
        }
        
        if (sceneTest2Button != null)
        {
            sceneTest2Button.onClick.AddListener(OnSceneTest2ButtonClicked);
        }
        else
        {
            Debug.LogWarning("Scene Test 2 button not assigned in Menu!");
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        else
        {
            Debug.LogWarning("Exit button not assigned in Menu!");
        }
    }
    
    /// <summary>
    /// Load Scene_Test1
    /// </summary>
    public void OnSceneTest1ButtonClicked()
    {
        SceneManager.LoadScene(sceneTest1Name);
    }
    
    /// <summary>
    /// Load Scene_Test2
    /// </summary>
    public void OnSceneTest2ButtonClicked()
    {
        SceneManager.LoadScene(sceneTest2Name);
    }
    
    /// <summary>
    /// Exit the application
    /// </summary>
    public void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
