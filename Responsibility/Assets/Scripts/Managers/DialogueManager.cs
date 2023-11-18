using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "ScriptableObjects/Manager/DialogueManager", order = 1)]
public class DialogueManager : ScriptableObject
{

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanelPrefab;
    private TextMeshProUGUI dialogueText;
    private GameObject dialoguePanel;
    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }
   
    private void OnEnable()
    {
        DialogEvents.onDialogSpawned += SpawnDialogMenu;
        DialogEvents.onDioalogUpdate += DialogUpdate;
    }

    private void OnDisable()
    {
        DialogEvents.onDialogSpawned -= SpawnDialogMenu;
        DialogEvents.onDioalogUpdate -= DialogUpdate;
    }

    private void DialogUpdate()
    {
        if (!isDialoguePlaying)
        {
            return;
        }
        if (GameManager.Instance.InputManager.GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    private void SpawnDialogMenu(GameObject canvas)
    {
        isDialoguePlaying = false;
        dialoguePanel = Instantiate(dialoguePanelPrefab);
        dialoguePanel.transform.SetParent(canvas.transform, false);
        Transform childObject = dialoguePanel.transform.Find("DialogueText");
        dialogueText = childObject.GetComponent<TextMeshProUGUI>();
        dialoguePanel.SetActive(false);
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {

        dialoguePanel.SetActive(true);
        isDialoguePlaying = true;
        currentStory = new Story(inkJSON.text);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }

}