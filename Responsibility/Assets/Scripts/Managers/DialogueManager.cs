using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "ScriptableObjects/Manager/DialogueManager", order = 1)]
public class DialogueManager : ScriptableObject
{

    [Header("Dialogue UI")]
    private TextMeshProUGUI dialogueText;
    private GameObject dialoguePanel;
    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }


    public void DialogUpdate()
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


    public void InitiateDialogueMenu(GameObject canvas)
    {
        isDialoguePlaying = false;
        UnityEngine.Transform childObject = canvas.transform.Find("DialoguePanel");
        dialoguePanel = childObject.gameObject;
        childObject = dialoguePanel.transform.Find("DialogueText");
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