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

    private TextMeshProUGUI displayNameText;
    private Animator portraitAnimator;
    private Animator layoutAnimator;
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
        UnityEngine.Transform panelTransform = canvas.transform.Find("DialoguePanel");
        dialoguePanel = panelTransform.gameObject;

        UnityEngine.Transform dialogueTextPanel = dialoguePanel.transform.Find("DialogueText");
        dialogueText = dialogueTextPanel.GetComponent<TextMeshProUGUI>();

        UnityEngine.Transform speakerFrameTransform = dialoguePanel.transform.Find("SpeakerFrame");
        UnityEngine.Transform textTransform = speakerFrameTransform.Find("DisplayNameText");
        displayNameText = textTransform.GetComponent<TextMeshProUGUI>();

        UnityEngine.Transform speakerPortraitTransform = dialoguePanel.transform.Find("SpeakerPortrait");
        UnityEngine.Transform portraitTransform = speakerPortraitTransform.Find("PortraitImage");
        portraitAnimator = portraitTransform.GetComponent<Animator>();

        layoutAnimator = dialoguePanel.GetComponent<Animator>();
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
            HandleTags(currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                case "speaker":
                    displayNameText.text = tagValue;
                    break;
                case "portrait":
                    portraitAnimator.Play(tagValue);
                    break;
                case "layout":
                    layoutAnimator.Play(tagValue);
                    break;
                /*                case "audio":
                                    SetCurrentAudioInfo(tagValue);
                                    break;*/
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }


}