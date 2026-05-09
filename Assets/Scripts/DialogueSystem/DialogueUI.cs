using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject clickIndicator;

    [Header("Borders")]
    public Image borderImage; // The Image component that shows the border
    public Sprite defaultBorderSprite;
    public Sprite casinoBorderSprite;

    private Coroutine _typingCoroutine;
    private bool _isTyping = false;
    private DialogueData _currentData;
    private int _entryIndex = 0;

    void Awake()
    {
        if (_currentData == null && dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(DialogueData data)
    {
        _currentData = data;
        _entryIndex = 0;
        
        // Setup visual style
        SetupStyle(data.style);
        
        dialoguePanel.SetActive(true);
        DisplayNextEntry();
    }

    private void SetupStyle(DialogueData.DialogueStyle style)
    {
        if (borderImage == null) return;

        switch (style)
        {
            case DialogueData.DialogueStyle.Default:
                if (defaultBorderSprite != null) borderImage.sprite = defaultBorderSprite;
                break;
            case DialogueData.DialogueStyle.Casino:
                if (casinoBorderSprite != null) borderImage.sprite = casinoBorderSprite;
                break;
        }
    }

    public void DisplayNextEntry()
    {
        if (_entryIndex < _currentData.entries.Count)
        {
            StopCurrentTyping();
            _typingCoroutine = StartCoroutine(TypeEntry(_currentData.entries[_entryIndex]));
            _entryIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeEntry(DialogueEntry entry)
    {
        _isTyping = true;
        dialogueText.text = "";
        
        if (clickIndicator != null) clickIndicator.SetActive(false);

        foreach (char letter in entry.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(_currentData.typingSpeed);
        }

        _isTyping = false;
        
        if (_currentData.isCutscene)
        {
            if (clickIndicator != null) clickIndicator.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(_currentData.ambientDisplayDuration);
            DisplayNextEntry();
        }
    }

    private void Update()
    {
        if (_currentData != null && _currentData.isCutscene && !_isTyping)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DisplayNextEntry();
            }
        }
    }

    private void StopCurrentTyping()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }
        _isTyping = false;
    }

    public void EndDialogue()
    {
        StopCurrentTyping();
        if (dialogueText != null) dialogueText.text = "";
        dialoguePanel.SetActive(false);
        _currentData = null;
    }
}
