using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "ScriptableObjects/Manager/DialogueManager", order = 1)]
public class DialogueManager : ScriptableObject
{
    [Header("Dialogue UI")]
    [SerializeField] private float typingSpeed = 0.04f;

    private TextMeshProUGUI dialogueText;
    private GameObject dialoguePanel;
    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }

    private TextMeshProUGUI displayNameText;
    private Animator portraitAnimator;
    private Animator layoutAnimator;

    private List<char> currentReplicaLetters;
    char firstLetter;
    private float timeElapsed = 0f;

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    [SerializeField] private bool makePredictable;
    private DialogueAudioInfoSO currentAudioInfo;
    private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
    private AudioSource audioSource;

    private int letterCounter = 0;
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
        timeElapsed += Time.deltaTime;
        if (currentReplicaLetters.Count > 0 && timeElapsed >= typingSpeed)
        {
            firstLetter = currentReplicaLetters[0];
            currentReplicaLetters.RemoveAt(0);
            dialogueText.text += firstLetter;
            letterCounter++;
            PlayDialogueSound(letterCounter, firstLetter);
            timeElapsed = 0f;
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

        GameObject audioObject = new GameObject("DialogueAudioSource");
        audioObject.transform.parent = canvas.transform;  // Set the provided canvas as the parent
        audioSource = audioObject.AddComponent<AudioSource>();

        currentAudioInfo = defaultAudioInfo;
        InitializeAudioInfoDictionary();
        dialoguePanel.SetActive(false);
        
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        /*GameObject audioObject = new GameObject("DialogueAudioSource");
        audioSource = audioObject.AddComponent<AudioSource>();*/
        dialoguePanel.SetActive(true);
        isDialoguePlaying = true;
        currentStory = new Story(inkJSON.text);

        dialogueText.text = "";

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
            dialogueText.text = "";
            currentReplicaLetters = currentStory.Continue().ToList();
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
                case "audio":
                    SetCurrentAudioInfo(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        if (audioSource == null)
        {
            Debug.LogError("Audio Source is nuuuuuuuuuuuuuuuuuuuuuuuuuuuull");
            return;
        }        // set variables for the below based on our config


        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;


        // play the sound based on the config
        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }
            AudioClip soundClip = null;
            // create predictable audio from hashing
            if (makePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();
                // sound clip
                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];
                // pitch
                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;
                // cannot divide by 0, so if there is no range then skip the selection
                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }
                else
                {
                    audioSource.pitch = minPitch;
                }
            }
            // otherwise, randomize the audio
            else
            {
                // sound clip
                int randomIndex = UnityEngine.Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];
                // pitch
                audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            }

            // play sound
            audioSource.PlayOneShot(soundClip);
        }
    }

   
    private void InitializeAudioInfoDictionary()
    {

        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
        audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
        foreach (DialogueAudioInfoSO audioInfo in audioInfos)
        {
            audioInfoDictionary.Add(audioInfo.id, audioInfo);
        }
    }
    private void SetCurrentAudioInfo(string id)
    {
        DialogueAudioInfoSO audioInfo = null;
        audioInfoDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null)
        {
            this.currentAudioInfo = audioInfo;
        }
        else
        {
            Debug.LogWarning("Failed to find audio info for id: " + id);
        }
    }


}