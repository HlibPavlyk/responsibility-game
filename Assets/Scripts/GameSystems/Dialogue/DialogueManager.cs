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
using System.Text;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "ScriptableObjects/Manager/DialogueManager", order = 1)]
public class DialogueManager : ScriptableObject
{
    [Header("Dialogue UI")]
    [SerializeField] private float typingSpeed = 0.04f;

    private InkTagModifiers inkTagModifiers;

    private TextMeshProUGUI dialogueText;
    private GameObject dialoguePanel;
    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }

    private TextMeshProUGUI displayNameText;
    private Animator portraitAnimator;
    private Animator layoutAnimator;

    private List<char> currentReplicaLetters;
    private StringBuilder dialogueStringBuilder;
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

    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;
    public event Action<string> OnSpeakerChanged;

    public void SetCharacterName(string name)
    {
        if (displayNameText != null)
        {
            displayNameText.text = name;
            OnSpeakerChanged?.Invoke(name);
        }
    }

    public void SetCharacterAnimatorState(string stateName)
    {
        if (portraitAnimator != null)
        {
            portraitAnimator.Play(stateName);
        }
    }

    public void SetCharacterLayoutPosition(string layoutPosition)
    {
        if (layoutAnimator != null)
        {
            layoutAnimator.Play(layoutPosition);
        }
    }

    public void SetCurrentAudioInfo(string id)
    {
        if (audioInfoDictionary == null)
        {
            Debug.LogError("Audio info dictionary is not initialized!");
            return;
        }

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

    public void DialogUpdate()
    {
        if (!isDialoguePlaying)
        {
            return;
        }
        
        /*if (GameManager.Instance?.InputManager?.GetSubmitPressed() == true)
        {
            ContinueStory();
        }*/
        
        timeElapsed += Time.deltaTime;
        if (currentReplicaLetters != null && currentReplicaLetters.Count > 0 && timeElapsed >= typingSpeed)
        {
            firstLetter = currentReplicaLetters[0];
            currentReplicaLetters.RemoveAt(0);
            
            if (dialogueStringBuilder == null)
                dialogueStringBuilder = new StringBuilder();
            
            dialogueStringBuilder.Append(firstLetter);
            dialogueText.text = dialogueStringBuilder.ToString();
            
            letterCounter++;
            PlayDialogueSound(letterCounter, firstLetter);
            timeElapsed = 0f;
        }
    }

    public void InitiateDialogueMenu(GameObject canvas)
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas is null in InitiateDialogueMenu!");
            return;
        }

        //todo inject dialogue managers service
        //inkTagModifiers = new InkTagModifiers(this);
        isDialoguePlaying = false;
        
        UnityEngine.Transform panelTransform = canvas.transform.Find("DialoguePanel");
        if (panelTransform == null)
        {
            Debug.LogError("DialoguePanel not found in canvas!");
            return;
        }
        dialoguePanel = panelTransform.gameObject;

        UnityEngine.Transform dialogueTextPanel = dialoguePanel.transform.Find("DialogueText");
        if (dialogueTextPanel == null)
        {
            Debug.LogError("DialogueText not found in DialoguePanel!");
            return;
        }
        dialogueText = dialogueTextPanel.GetComponent<TextMeshProUGUI>();
        if (dialogueText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on DialogueText!");
            return;
        }

        UnityEngine.Transform speakerFrameTransform = dialoguePanel.transform.Find("SpeakerFrame");
        if (speakerFrameTransform != null)
        {
            UnityEngine.Transform textTransform = speakerFrameTransform.Find("DisplayNameText");
            if (textTransform != null)
            {
                displayNameText = textTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        UnityEngine.Transform speakerPortraitTransform = dialoguePanel.transform.Find("SpeakerPortrait");
        if (speakerPortraitTransform != null)
        {
            UnityEngine.Transform portraitTransform = speakerPortraitTransform.Find("PortraitImage");
            if (portraitTransform != null)
            {
                portraitAnimator = portraitTransform.GetComponent<Animator>();
            }
        }

        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        if (audioSource == null)
        {
            GameObject audioObject = new GameObject("DialogueAudioSource");
            audioObject.transform.parent = canvas.transform;
            audioSource = audioObject.AddComponent<AudioSource>();
        }

        currentAudioInfo = defaultAudioInfo;
        InitializeAudioInfoDictionary();
        dialoguePanel.SetActive(false);
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        if (inkJSON == null)
        {
            Debug.LogError("InkJSON is null!");
            return;
        }

        if (dialoguePanel == null)
        {
            Debug.LogError("DialoguePanel is not initialized!");
            return;
        }

        dialoguePanel.SetActive(true);
        isDialoguePlaying = true;
        currentStory = new Story(inkJSON.text);

        if (dialogueStringBuilder == null)
            dialogueStringBuilder = new StringBuilder();
        dialogueStringBuilder.Clear();
        dialogueText.text = "";
        letterCounter = 0;

        OnDialogueStarted?.Invoke();
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        isDialoguePlaying = false;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
        if (dialogueStringBuilder != null)
        {
            dialogueStringBuilder.Clear();
        }
        letterCounter = 0;
        
        OnDialogueEnded?.Invoke();
    }

    private void ContinueStory()
    {
        if (currentStory == null)
        {
            Debug.LogError("CurrentStory is null!");
            ExitDialogueMode();
            return;
        }

        if (currentStory.canContinue)
        {
            dialogueText.text = "";
            if (dialogueStringBuilder != null)
                dialogueStringBuilder.Clear();
            
            currentReplicaLetters = currentStory.Continue().ToList();
            letterCounter = 0;
            
            FillInkTagModifiers(currentStory.currentTags);
            inkTagModifiers.ApplyAllModifiers();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void FillInkTagModifiers(List<string> currentTags)
    {
        if (inkTagModifiers == null)
        {
            Debug.LogError("InkTagModifiers is not initialized!");
            return;
        }

        inkTagModifiers.Clear();
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            try
            {
                DialogueModifier modifier = inkTagModifiers.CreateModifier(tag);
                inkTagModifiers.AddModifier(modifier);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating modifier for tag '{tag}': {e.Message}");
            }
        }
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        if (audioSource == null)
        {
            Debug.LogError("Audio Source is null");
            return;
        }

        if (currentAudioInfo == null)
        {
            Debug.LogWarning("CurrentAudioInfo is null, using default");
            currentAudioInfo = defaultAudioInfo;
        }

        if (currentAudioInfo == null)
        {
            Debug.LogError("DefaultAudioInfo is also null!");
            return;
        }

        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        if (dialogueTypingSoundClips == null || dialogueTypingSoundClips.Length == 0)
        {
            return; // Немає звуків для відтворення
        }

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
                int predictableIndex = Mathf.Abs(hashCode) % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];
                // pitch
                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;
                // cannot divide by 0, so if there is no range then skip the selection
                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (Mathf.Abs(hashCode) % pitchRangeInt) + minPitchInt;
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
            if (soundClip != null)
            {
                audioSource.PlayOneShot(soundClip);
            }
        }
    }

    private void InitializeAudioInfoDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
        
        if (defaultAudioInfo != null)
        {
            audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
        }
        else
        {
            Debug.LogWarning("DefaultAudioInfo is null!");
        }
        
        if (audioInfos != null)
        {
            foreach (DialogueAudioInfoSO audioInfo in audioInfos)
            {
                if (audioInfo != null)
                {
                    audioInfoDictionary.Add(audioInfo.id, audioInfo);
                }
            }
        }
    }

    public void Cleanup()
    {
        if (audioSource != null)
        {
            DestroyImmediate(audioSource.gameObject);
            audioSource = null;
        }
        
        if (dialogueStringBuilder != null)
        {
            dialogueStringBuilder.Clear();
            dialogueStringBuilder = null;
        }
        
        currentReplicaLetters?.Clear();
        currentReplicaLetters = null;
        
        currentStory = null;
        isDialoguePlaying = false;
    }
}