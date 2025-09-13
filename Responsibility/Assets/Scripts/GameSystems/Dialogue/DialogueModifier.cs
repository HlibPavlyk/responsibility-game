using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueModifier
{
    protected DialogueManager manager;
    protected string modifierValue;

    public DialogueModifier(DialogueManager manager, string modifierValue)
    {
        this.manager = manager ?? throw new System.ArgumentNullException(nameof(manager));
        this.modifierValue = modifierValue ?? throw new System.ArgumentNullException(nameof(modifierValue));
    }

    public virtual void ModifyDialogue() { }
}


public class ChangeSpeakerNameModifier : DialogueModifier
{
    public ChangeSpeakerNameModifier(DialogueManager manager, string modifierValue) : base(manager, modifierValue) { }
    
    public override void ModifyDialogue()
    {
        try
        {
            manager.SetCharacterName(modifierValue);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting character name '{modifierValue}': {e.Message}");
        }
    }
}

public class ChangeSpeakerPortraitModifier : DialogueModifier
{
    public ChangeSpeakerPortraitModifier(DialogueManager manager, string modifierValue) : base(manager, modifierValue) { }
    
    public override void ModifyDialogue()
    {
        try
        {
            manager.SetCharacterAnimatorState(modifierValue);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting character portrait '{modifierValue}': {e.Message}");
        }
    }
}

public class ChangeSpeakerLayoutPositionModifier : DialogueModifier
{
    public ChangeSpeakerLayoutPositionModifier(DialogueManager manager, string modifierValue) : base(manager, modifierValue) { }
    
    public override void ModifyDialogue()
    {
        try
        {
            manager.SetCharacterLayoutPosition(modifierValue);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting character layout '{modifierValue}': {e.Message}");
        }
    }
}

public class ChangeSpeakerAudioInfoModifier : DialogueModifier
{
    public ChangeSpeakerAudioInfoModifier(DialogueManager manager, string modifierValue) : base(manager, modifierValue) { }
    
    public override void ModifyDialogue()
    {
        try
        {
            manager.SetCurrentAudioInfo(modifierValue);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting audio info '{modifierValue}': {e.Message}");
        }
    }
}