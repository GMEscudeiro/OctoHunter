using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static bool IsDialogueActive { get; private set; }
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
    private int _stopAtIndex = 0;

    void Awake()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (cutsceneImageContainer != null) cutsceneImageContainer.gameObject.SetActive(false);
    }

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Hard-reset: don't fire OnDialogueEnded callbacks here — callers that
        // triggered a scene load via a callback already completed their flow.
        StopCurrentTyping();
        _isTyping   = false;
        _currentData = null;
        _entryIndex  = 0;
        _stopAtIndex = 0;
        OnDialogueEnded = null;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (cutsceneImageContainer != null) cutsceneImageContainer.gameObject.SetActive(false);
    }

    public void ShowDialogue(DialogueData data)
    {
        _currentData = data;

        if (data.randomEntry && data.entries.Count > 0)
        {
            _entryIndex  = Random.Range(0, data.entries.Count);
            _stopAtIndex = _entryIndex + 1;
        }
        else
        {
            _entryIndex  = 0;
            _stopAtIndex = data.entries.Count;
        }
        
        // Setup visual style
        SetupStyle(data.style);
        
        IsDialogueActive = true;

        if (data.isCutscene) OnCutsceneStarted?.Invoke(data.hideHUD);

        dialoguePanel.SetActive(true);

        if (cutsceneImageContainer != null)
            cutsceneImageContainer.gameObject.SetActive(false);

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
        if (_entryIndex < _stopAtIndex)
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
                cutsceneImageContainer.sprite = entry.displayImage;
                cutsceneImageContainer.gameObject.SetActive(true);
            }
            else
            {
                cutsceneImageContainer.gameObject.SetActive(false);
            }
        }
        else if (entry.displayImage != null)
        {
            Debug.LogWarning("[DialogueUI] cutsceneImageContainer é null mas a entrada tem imagem.");
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

    // bool = hideHUD: listeners decidem se devem esconder a HUD
    public static event System.Action<bool> OnCutsceneStarted;
    public static event System.Action<bool> OnCutsceneEnded;

    public void EndDialogue()
    {
        StopCurrentTyping();
        if (dialogueText != null) dialogueText.text = "";
        dialoguePanel.SetActive(false);
        if (cutsceneImageContainer != null) cutsceneImageContainer.gameObject.SetActive(false);
        IsDialogueActive = false;

        bool wasCutscene = _currentData != null && _currentData.isCutscene;
        bool wasHideHUD  = wasCutscene && _currentData.hideHUD;
        _currentData = null;

        if (wasCutscene) OnCutsceneEnded?.Invoke(wasHideHUD);

        System.Action tempAction = OnDialogueEnded;
        OnDialogueEnded = null;
        tempAction?.Invoke();
    }
}
