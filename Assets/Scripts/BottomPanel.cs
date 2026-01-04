using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BottomPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backButton;
    
    [Header("Confirmation Panel")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;
    
    [Header("Messages")]
    [SerializeField] private string continueMessage = "Layout Saved";
    [SerializeField] private string backMessage = "Order Reset";
    
    [Header("Settings")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private float messageDuration = 2f;
    
    private BattingOrderManager manager;
    
    void Start()
    {
        manager = BattingOrderManager.Instance;
        
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
        
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }
    
    public void OnContinueButtonClicked()
    {
        if (manager != null)
            manager.SaveOrder();
        
        StartCoroutine(ShowPanelAndReturnToMenu(continueMessage));
    }
    
    public void OnBackButtonClicked()
    {
        if (manager != null)
            manager.DiscardChanges();
        
        StartCoroutine(ShowPanelAndReturnToMenu(backMessage));
    }
    
    private IEnumerator ShowPanelAndReturnToMenu(string message)
    {
        if (confirmationText != null)
            confirmationText.text = message;
        
        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);
        
        if (continueButton != null)
            continueButton.interactable = false;
        if (backButton != null)
            backButton.interactable = false;
        
        yield return new WaitForSeconds(messageDuration);
        
        SceneManager.LoadScene(menuSceneName);
    }
    
    private void OnDestroy()
    {
        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueButtonClicked);
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackButtonClicked);
    }
}
