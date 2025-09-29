using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.DI;
using Core.Events;
using TMPro;
using UnityEngine;
using VContainer;
using ResponsibilityGame.Core.Interfaces;

namespace ResponsibilityGame.GameSystems.Dialogue
{
    public class DialogueManagerService : IDialogueManager
    {
        [Inject] private readonly IInputManager inputManager;
        [Inject] private readonly DialogueManagerSettings settings;
        [Inject] private readonly GameState gameState;
       
        private InkTagModifiers inkTagModifiers;
        private TextMeshProUGUI dialogueText;
        private GameObject dialoguePanel;
        private Story currentStory;
        private TextMeshProUGUI displayNameText;
        private Animator portraitAnimator;
        private Animator layoutAnimator;
        private List<char> currentReplicaLetters;
        private StringBuilder dialogueStringBuilder;
        private char firstLetter;
        private float timeElapsed = 0f;
        private DialogueAudioInfoSO currentAudioInfo;
        private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
        private AudioSource audioSource;
        private int letterCounter = 0;

        public void Initialize(GameObject canvas)
        {
            if (canvas == null)
            {
                Debug.LogError("Canvas is null in Initialize!");
                return;
            }

            inkTagModifiers = new InkTagModifiers(this);
            gameState.isDialoguePlaying = false;

            Transform panelTransform = canvas.transform.Find("DialoguePanel");
            if (panelTransform == null)
            {
                Debug.LogError("DialoguePanel not found in canvas!");
                return;
            }
            dialoguePanel = panelTransform.gameObject;

            Transform dialogueTextPanel = dialoguePanel.transform.Find("DialogueText");
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

            Transform speakerFrameTransform = dialoguePanel.transform.Find("SpeakerFrame");
            if (speakerFrameTransform != null)
            {
                Transform textTransform = speakerFrameTransform.Find("DisplayNameText");
                if (textTransform != null)
                {
                    displayNameText = textTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            Transform speakerPortraitTransform = dialoguePanel.transform.Find("SpeakerPortrait");
            if (speakerPortraitTransform != null)
            {
                Transform portraitTransform = speakerPortraitTransform.Find("PortraitImage");
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

            currentAudioInfo = settings.defaultAudioInfo;
            InitializeAudioInfoDictionary();
            dialoguePanel.SetActive(false);

            Debug.Log("DialogueManagerService initialized successfully");
        }

        public void DialogUpdate()
        {
            if (!gameState.isDialoguePlaying)
            {
                return;
            }

            if (inputManager?.GetSubmitPressed() == true)
            {
                ContinueStory();
            }

            timeElapsed += Time.deltaTime;
            if (currentReplicaLetters != null && currentReplicaLetters.Count > 0 && timeElapsed >= settings.typingSpeed)
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
            gameState.isDialoguePlaying = true;
            currentStory = new Story(inkJSON.text);

            if (dialogueStringBuilder == null)
                dialogueStringBuilder = new StringBuilder();
            dialogueStringBuilder.Clear();
            dialogueText.text = "";
            letterCounter = 0;

            GameEvents.Dialogue.OnDialogueStarted?.Invoke();
            ContinueStory();
        }

        public void SetCharacterName(string name)
        {
            if (displayNameText != null)
            {
                displayNameText.text = name;
                GameEvents.Dialogue.OnSpeakerChanged?.Invoke(name);
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

        public void Cleanup()
        {
            if (audioSource != null)
            {
                UnityEngine.Object.DestroyImmediate(audioSource.gameObject);
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
            gameState.isDialoguePlaying = false;
        }

        private void ExitDialogueMode()
        {
            gameState.isDialoguePlaying = false;
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

            GameEvents.Dialogue.OnDialogueEnded?.Invoke();
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
                currentAudioInfo = settings.defaultAudioInfo;
            }

            if (currentAudioInfo == null)
            {
                Debug.LogError("DefaultAudioInfo is also null!");
                return;
            }

            AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
            if (dialogueTypingSoundClips == null || dialogueTypingSoundClips.Length == 0)
            {
                return;
            }

            int frequencyLevel = currentAudioInfo.frequencyLevel;
            float minPitch = currentAudioInfo.minPitch;
            float maxPitch = currentAudioInfo.maxPitch;
            bool stopAudioSource = currentAudioInfo.stopAudioSource;

            if (currentDisplayedCharacterCount % frequencyLevel == 0)
            {
                if (stopAudioSource)
                {
                    audioSource.Stop();
                }
                AudioClip soundClip = null;

                if (settings.makePredictable)
                {
                    int hashCode = currentCharacter.GetHashCode();
                    int predictableIndex = Mathf.Abs(hashCode) % dialogueTypingSoundClips.Length;
                    soundClip = dialogueTypingSoundClips[predictableIndex];
                    int minPitchInt = (int)(minPitch * 100);
                    int maxPitchInt = (int)(maxPitch * 100);
                    int pitchRangeInt = maxPitchInt - minPitchInt;
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
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, dialogueTypingSoundClips.Length);
                    soundClip = dialogueTypingSoundClips[randomIndex];
                    audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                }

                if (soundClip != null)
                {
                    audioSource.PlayOneShot(soundClip);
                }
            }
        }

        private void InitializeAudioInfoDictionary()
        {
            audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();

            if (settings.defaultAudioInfo != null)
            {
                audioInfoDictionary.Add(settings.defaultAudioInfo.id, settings.defaultAudioInfo);
            }
            else
            {
                Debug.LogWarning("DefaultAudioInfo is null!");
            }

            if (settings.audioInfos != null)
            {
                foreach (DialogueAudioInfoSO audioInfo in settings.audioInfos)
                {
                    if (audioInfo != null)
                    {
                        audioInfoDictionary.Add(audioInfo.id, audioInfo);
                    }
                }
            }
        }
    }
}
