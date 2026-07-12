using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(UiScreen), editorForChildClasses: true)]
public class UiScreenEditor : Editor
{
    UiScreen uiScreen;

    void OnEnable()
    {
        uiScreen = (UiScreen)target;
    }

    public override void OnInspectorGUI()
    {
        CanvasGroup group = uiScreen.GetComponent<CanvasGroup>();
        if (group && !Application.isPlaying)
        {
            if (Mathf.Approximately(group.alpha, 0))
            {
                EditorGUILayout.HelpBox("CanvasGroup alpha is 0. This will make the UI screen invisible in the editor.", MessageType.Warning);
            }
            if (!group.blocksRaycasts || !group.interactable)
            {
                EditorGUILayout.HelpBox("CanvasGroup is not interactable or does not block raycasts. This will make the UI screen non-interactive in the editor.", MessageType.Warning);
            }
        }

        base.OnInspectorGUI();

        if (uiScreen.feedbackConfig != null)
        {
            if (GUILayout.Button("Add Audio Feedback Event"))
            {
                RemoveAudioFeedbackEvents(false);
                AddAudioFeedbackEvents();
            }

            if (GUILayout.Button("Remove Audio Feedback Event"))
            {
                RemoveAudioFeedbackEvents();
            }
        }
    }

    protected virtual void AddAudioFeedbackEvents(bool recordHistory = true)
    {
        List<GameObject> modifiedObjects = new List<GameObject>();
        foreach (Selectable select in uiScreen.GetComponentsInChildren<Selectable>(true))
        {
            if (recordHistory)
            {
                Undo.RecordObject(select.gameObject, "Added Audio Feedback Event");
            }

            EventTrigger trigger = select.GetComponent<EventTrigger>() ?? select.gameObject.AddComponent<EventTrigger>();

            FieldInfo[] fieldInfo = typeof(UiAudioFeedback).GetFields();

            foreach (FieldInfo field in fieldInfo)
            {
                EventTriggerType eventType;
                try
                {
                    eventType = (EventTriggerType)System.Enum.Parse(typeof(EventTriggerType), field.Name.Substring(0, 1).ToUpper() + field.Name.Substring(1));
                }
                catch (System.Exception)
                {
                    Debug.LogWarning($"UI Audio Feedback contain a property {field.Name} which cannot be mapped to an Event Trigger event type");
                    continue;
                }

                AudioClip audioClip = (AudioClip)field.GetValue(uiScreen.feedbackConfig);
                EventTrigger.Entry entry = trigger.triggers.Find(e => e.eventID == eventType);

                if (entry == null)
                {
                    entry = new EventTrigger.Entry { eventID = eventType };
                    trigger.triggers.Add(entry);
                }

                if (!HasPersistentListener(entry, "Play Audio Feedback"))
                {
                    UnityEventTools.AddStringPersistentListener(entry.callback, uiScreen.PlayAudioFeedback, field.Name);
                }
                else
                {
                    continue;
                }

                modifiedObjects.Add(select.gameObject);
            }
            EditorUtility.SetDirty(trigger);
        }
        Selection.objects = modifiedObjects.ToArray();
    }

    protected virtual void RemoveAudioFeedbackEvents(bool recordHistory = true)
    {
        List<GameObject> modifiedObjects = new List<GameObject>();
        foreach (Selectable select in uiScreen.GetComponentsInChildren<Selectable>())
        {
            if (recordHistory)
            {
                Undo.RecordObject(select.gameObject, "Remove Audio Feedback Event");
            }

            EventTrigger trig = select.GetComponent<EventTrigger>();
            if (trig)
            {
                RemoveAllPersistentListener(trig, "PlayAudioFeedback");
            }

            modifiedObjects.Add(select.gameObject);

            EditorUtility.SetDirty(select.gameObject);
        }
        Selection.objects = modifiedObjects.ToArray();
    }

    void RemoveAllPersistentListener(EventTrigger t, string methodName, bool removeTriggerifEmpty = true)
    {
        List<EventTrigger.Entry> emptyEntries = new List<EventTrigger.Entry>();
        foreach (EventTrigger.Entry e in t.triggers)
        {
            RemovePersistentListener(e, methodName);

            if (e.callback.GetPersistentEventCount() <= 0)
            {
                emptyEntries.Add(e);
            }
        }
        foreach (EventTrigger.Entry e in emptyEntries)
        {
            t.triggers.Remove(e);
        }

        if (t.triggers.Count > 0)
        {
            Undo.DestroyObjectImmediate(t);
        }
    }
    void RemovePersistentListener(EventTrigger.Entry entry, string methodName)
    {
        for (int i = 0; i < entry.callback.GetPersistentEventCount(); i++)
        {
            if (entry.callback.GetPersistentMethodName(i) == methodName)
            {
                UnityEventTools.RemovePersistentListener(entry.callback, i);
            }
        }
    }
    bool HasPersistentListener(EventTrigger.Entry entry, string methodName)
    {
        for (int i = 0; i < entry.callback.GetPersistentEventCount(); ++i)
        {
            if (entry.callback.GetPersistentMethodName(i) == methodName)
            {
                return true;
            }
        }
        return false;
    }
}
