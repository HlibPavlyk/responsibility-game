using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueModifier
{
    protected DialogueManager manager;
    protected string modifierValue;

    public DialogueModifier(DialogueManager manager, string modifierValue)
    {
        this.manager = manager;
        this.modifierValue = modifierValue;
    }

    public virtual void ModifyDialogue() { }
}


public class ChangeSpeakerNameModifier : DialogueModifier
{
    public ChangeSpeakerNameModifier(DialogueManager manager, string modifierValue) : base(manager, modifierValue) { }
    public override void ModifyDialogue()
    {
        manager.SetCharacterName(modifierValue);
    }
}

public class ChangeSpeakerPortraitModifier : DialogueModifier
{
    public ChangeSpeakerPortraitModifier(DialogueManager manager, string modifierValue) : base(manager, modifierValue) { }
    public override void ModifyDialogue()
    {
        manager.SetCharacterAnimatorState(modifierValue);
    }
}

public class ChangeSpeakerLayoutPositionModifier : DialogueModifier
{
    public ChangeSpeakerLayoutPositionModifier(DialogueManager manager, string modifierValue) : base(manager, modifierValue) { }
    public override void ModifyDialogue()
    {
        manager.SetCharacterLayoutPosition(modifierValue);
    }
}

public class ChangeSpeakerAudioInfoModifier : DialogueModifier
{
    public ChangeSpeakerAudioInfoModifier(DialogueManager manager, string modifierValue) : base(manager, modifierValue) { }
    public override void ModifyDialogue()
    {
        manager.SetCurrentAudioInfo(modifierValue);
    }
}