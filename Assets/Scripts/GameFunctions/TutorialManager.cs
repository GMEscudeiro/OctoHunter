using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialAction { None, Move, Shoot, SwapWeapon }

    [System.Serializable]
    public class TutorialStep
    {
        public DialogueData dialogue;
        public TutorialAction requiredAction = TutorialAction.None;
        [Tooltip("Text shown in hint UI while waiting for the action")]
        public string actionHintText;
    }

    [Header("References")]
    public DialogueUI dialogueUI;

    [Header("Hint UI")]
    public GameObject actionHintPanel;
    public TextMeshProUGUI actionHintLabel;

    [Header("Steps")]
    public List<TutorialStep> steps;

    [Header("On Complete")]
    [Tooltip("Fallback: cena carregada se GameFlowManager não estiver disponível")]
    public string fallbackSceneName = "StartMenu";

    private int _stepIndex = 0;
    private bool _waitingForAction = false;
    private TutorialAction _pendingAction;

    // Move tracking — all four keys must be pressed at least once
    private bool _pressedW, _pressedA, _pressedS, _pressedD;

    // SwapWeapon tracking — need two distinct slot clicks
    private int _firstSwapSlot = -1;

    void OnEnable()
    {
        WeaponBarUI.OnWeaponSlotClicked += OnWeaponSlotClicked;
    }

    void OnDisable()
    {
        WeaponBarUI.OnWeaponSlotClicked -= OnWeaponSlotClicked;
    }

    void Start()
    {
        HideHintPanel();
        PlayNextStep();
    }

    void Update()
    {
        if (!_waitingForAction) return;

        switch (_pendingAction)
        {
            case TutorialAction.Move:
                if (Input.GetKeyDown(KeyCode.W)) _pressedW = true;
                if (Input.GetKeyDown(KeyCode.A)) _pressedA = true;
                if (Input.GetKeyDown(KeyCode.S)) _pressedS = true;
                if (Input.GetKeyDown(KeyCode.D)) _pressedD = true;
                if (_pressedW && _pressedA && _pressedS && _pressedD) ActionCompleted();
                break;
            case TutorialAction.Shoot:
                if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
                    ActionCompleted();
                break;
        }
    }

    private void PlayNextStep()
    {
        HideHintPanel();

        if (_stepIndex >= steps.Count)
        {
            TutorialComplete();
            return;
        }

        TutorialStep step = steps[_stepIndex];
        _stepIndex++;

        if (step.dialogue != null && dialogueUI != null)
        {
            dialogueUI.OnDialogueEnded += OnStepDialogueEnded;
            dialogueUI.ShowDialogue(step.dialogue);
        }
        else
        {
            OnStepDialogueEnded();
        }
    }

    private void OnStepDialogueEnded()
    {
        // OnDialogueEnded is already cleared by DialogueUI.EndDialogue before invoking
        TutorialStep justPlayed = steps[_stepIndex - 1];

        if (justPlayed.requiredAction == TutorialAction.None)
        {
            PlayNextStep();
        }
        else
        {
            StartWaitingForAction(justPlayed.requiredAction, justPlayed.actionHintText);
        }
    }

    private void StartWaitingForAction(TutorialAction action, string hint)
    {
        _pendingAction = action;
        _waitingForAction = true;

        // Reset per-action state
        _pressedW = _pressedA = _pressedS = _pressedD = false;
        _firstSwapSlot = -1;

        ShowHintPanel(hint);
    }

    private void ActionCompleted()
    {
        _waitingForAction = false;
        HideHintPanel();
        PlayNextStep();
    }

    private void ShowHintPanel(string hint)
    {
        if (actionHintPanel != null) actionHintPanel.SetActive(true);
        if (actionHintLabel != null)
        {
            actionHintLabel.gameObject.SetActive(true);
            actionHintLabel.text = hint;
        }
    }

    private void HideHintPanel()
    {
        if (actionHintPanel != null) actionHintPanel.SetActive(false);
        if (actionHintLabel != null) actionHintLabel.gameObject.SetActive(false);
    }

    private void OnWeaponSlotClicked(int index)
    {
        if (!_waitingForAction || _pendingAction != TutorialAction.SwapWeapon) return;

        if (_firstSwapSlot == -1)
        {
            _firstSwapSlot = index;
        }
        else if (index != _firstSwapSlot)
        {
            ActionCompleted();
        }
    }

    private void TutorialComplete()
    {
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.ReturnToStartMenu();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(fallbackSceneName);
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
