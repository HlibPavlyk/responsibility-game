using System;
using System.Collections;
using System.Collections.Generic;
using Core.Abstractions;
using UnityEngine;

public abstract class DialogueModifiers
{
    protected IDialogueManager manager;
    protected DialogueModifiers(IDialogueManager manager)
    {
        this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
    }
    public abstract DialogueModifier CreateModifier(string context);
}


public class InkTagModifiers : DialogueModifiers
{
    List<DialogueModifier> modifiers = new List<DialogueModifier>();

    public InkTagModifiers(IDialogueManager manager) : base(manager) { }

    public void AddModifier(DialogueModifier modifier)
    {
        if (modifier != null)
        {
            modifiers.Add(modifier);
        }
        else
        {
            Debug.LogWarning("Attempted to add null modifier to InkTagModifiers");
        }
    }
    
    public void Clear()
    {
        modifiers.Clear();
    }

    public void ApplyAllModifiers()
    {
        foreach (DialogueModifier modifier in modifiers)
        {
            try
            {
                modifier?.ModifyDialogue();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error applying modifier {modifier?.GetType().Name}: {e.Message}");
            }
        }
    }

    public override DialogueModifier CreateModifier(string tag)
    {
        if (string.IsNullOrEmpty(tag))
        {
            Debug.LogError("Tag is null or empty");
            return null;
        }

        // parse the tag
        string[] splitTag = tag.Split(':');
        if (splitTag.Length != 2)
        {
            Debug.LogError($"Tag could not be appropriately parsed: {tag}. Expected format: 'key:value'");
            return null;
        }
        
        string tagKey = splitTag[0].Trim();
        string tagValue = splitTag[1].Trim();

        if (string.IsNullOrEmpty(tagKey) || string.IsNullOrEmpty(tagValue))
        {
            Debug.LogError($"Tag key or value is empty: {tag}");
            return null;
        }

        var tagModifiers = new Dictionary<string, Func<DialogueModifier>>() {
            { "speaker", () => new ChangeSpeakerNameModifier(manager, tagValue) },
            { "portrait", () => new ChangeSpeakerPortraitModifier(manager, tagValue) },
            { "layout", () => new ChangeSpeakerLayoutPositionModifier(manager, tagValue) },
            { "audio", () => new ChangeSpeakerAudioInfoModifier(manager, tagValue) }
        };

        if (tagModifiers.TryGetValue(tagKey, out var createModifier))
        {
            try
            {
                return createModifier();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating modifier for tag '{tag}': {e.Message}");
                return null;
            }
        }

        Debug.LogWarning($"Tag '{tag}' is not currently being handled. Available tags: {string.Join(", ", tagModifiers.Keys)}");
        return null;
    }
}