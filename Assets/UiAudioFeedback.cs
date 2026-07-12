using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UiAudioFeedback", menuName = "ScriptableObjects/UiAudioFeedback", order = 1000)]

public class UiAudioFeedback : ScriptableObject
{
    public AudioClip pointedClip, pointedEnter, pointedExit, pointedDown, pointedUp;
    public AudioClip select, deselect, submit;

    public AudioClip GetSound(string type)
    {
        System.Reflection.FieldInfo field = typeof(UiAudioFeedback).GetField(type);
        if (field != null && field.FieldType == typeof(AudioClip))
        {
            AudioClip result = (AudioClip)field.GetValue(this);
            if (result)
            {
                return result;
            }
        }
        return null;
    }
}
