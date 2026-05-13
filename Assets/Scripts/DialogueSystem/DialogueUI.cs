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
    public Image cutsceneImageContainer;

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

        // Garante que o container começa oculto até a primeira entrada definir sua visibilidade
        if (cutsceneImageContainer != null)
            cutsceneImageContainer.gameObject.SetActive(false);
        else
            Debug.LogWarning("[DialogueUI] cutsceneImageContainer não está referenciado no Inspector!");

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

        // Troca a borda de acordo com o personagem da entrada (ou volta ao padrão do estilo)
        if (borderImage != null)
        {
            if (entry.borderSprite != null)
                borderImage.sprite = entry.borderSprite;
            else
                SetupStyle(_currentData.style);
        }

        // Update image
        if (cutsceneImageContainer != null)
        {
            if (entry.displayImage != null)
            {
                Debug.Log($"[DialogueUI] Exibindo imagem: {entry.displayImage.name}");
                cutsceneImageContainer.sprite = entry.displayImage;
                cutsceneImageContainer.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("[DialogueUI] Entrada sem imagem — ocultando container.");
                cutsceneImageContainer.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("[DialogueUI] cutsceneImageContainer é null durante TypeEntry!");
        }

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

    public System.Action OnDialogueEnded;

    public void EndDialogue()
    {
        StopCurrentTyping();
        if (dialogueText != null) dialogueText.text = "";
        dialoguePanel.SetActive(false);
        if (cutsceneImageContainer != null) cutsceneImageContainer.gameObject.SetActive(false);
        _currentData = null;

        System.Action tempAction = OnDialogueEnded;
        OnDialogueEnded = null; // Clear before invoking to prevent wiping out new chained subscriptions
        tempAction?.Invoke();
    }
}
