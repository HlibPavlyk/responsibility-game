using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueModifiers
{
    protected DialogueManager manager;
    public DialogueModifiers(DialogueManager manager)
    {
        this.manager = manager;
    }
    public abstract DialogueModifier CreateModifier(string context);
}


public class InkTagModifiers : DialogueModifiers
{
    List<DialogueModifier> modifiers = new List<DialogueModifier>();

    public InkTagModifiers(DialogueManager manager) : base(manager) { }

    public void AddModifier(DialogueModifier modifier)
    {
        modifiers.Add(modifier);
    }
    public void Clear()
    {
        modifiers.Clear();
    }

    public void ApplyAllModifiers()
    {
        foreach (DialogueModifier modifier in modifiers)
        {
            modifier.ModifyDialogue();
        }
    }

    public override DialogueModifier CreateModifier(string tag)
    {
        // parse the tag
        string[] splitTag = tag.Split(':');
        if (splitTag.Length != 2)
        {
            Debug.LogError("Tag could not be appropriately parsed: " + tag);
        }
        string tagKey = splitTag[0].Trim();
        string tagValue = splitTag[1].Trim();

        var tagModifiers = new Dictionary<string, Func<DialogueModifier>>() {
            { "speaker", () => new ChangeSpeakerNameModifier(manager, tagValue) },
            { "portrait", () => new ChangeSpeakerPortraitModifier(manager, tagValue) },
            { "layout", () => new ChangeSpeakerLayoutPositionModifier(manager, tagValue) },
            { "audio", () => new ChangeSpeakerAudioInfoModifier(manager, tagValue) }
        };

        if (tagModifiers.TryGetValue(tagKey, out var createModifier))
        {
            return createModifier();
        }

        throw new KeyNotFoundException($"Tag came in but is not currently being handled: {tag}");
    }
}