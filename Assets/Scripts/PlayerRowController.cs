using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Component attached to the PlayerRow prefab that binds UI elements
/// and forwards button events to the UI controller.
/// </summary>
public class PlayerRowController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;

    private int currentIndex;
    private int playerPosition;
    private string currentPlayerID; // Track which player this row is displaying
    private Action<int> onMoveUp;
    private Action<int> onMoveDown;

    /// <summary>
    /// Get the player ID currently displayed by this row
    /// </summary>
    public string GetCurrentPlayerID() => currentPlayerID;

    /// <summary>
    /// Set the data for this player row
    /// </summary>
    public void SetData(int position, PlayerData player, Sprite avatar, Action<int> onUpClick, Action<int> onDownClick)
    {
        playerPosition = position;
        currentPlayerID = player.ID; // Store the player ID
        onMoveUp = onUpClick;
        onMoveDown = onDownClick;

        // Display position number (1-indexed for user)
        numberText.text = (position + 1).ToString();

        // Set avatar
        if (avatarImage != null)
        {
            avatarImage.sprite = avatar;
        }
        
        // Set up button listeners
        upButton.onClick.RemoveAllListeners();
        downButton.onClick.RemoveAllListeners();

        upButton.onClick.AddListener(OnUpButtonClicked);
        downButton.onClick.AddListener(OnDownButtonClicked);
    }

    /// <summary>
    /// Update only the position number (for swaps - keeps avatar unchanged)
    /// </summary>
    public void UpdatePositionOnly(int newPosition, Action<int> onUpClick, Action<int> onDownClick)
    {
        playerPosition = newPosition;
        onMoveUp = onUpClick;
        onMoveDown = onDownClick;

        // Update position number display (1-indexed for user)
        numberText.text = (newPosition + 1).ToString();
        
        // Update button listeners with new position
        upButton.onClick.RemoveAllListeners();
        downButton.onClick.RemoveAllListeners();

        upButton.onClick.AddListener(OnUpButtonClicked);
        downButton.onClick.AddListener(OnDownButtonClicked);
    }

    /// <summary>
    /// Set button interactable states
    /// </summary>
    public void SetButtonStates(bool canMoveUp, bool canMoveDown)
    {
        upButton.interactable = canMoveUp;
        downButton.interactable = canMoveDown;
    }

    private void OnUpButtonClicked()
    {
        onMoveUp?.Invoke(playerPosition);
    }

    private void OnDownButtonClicked()
    {
        onMoveDown?.Invoke(playerPosition);
    }
}
