using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class ScriptableObjectAIBehavior : AIBehavior
{
    [SerializeField] ReusableAIBehavior behavior;
    [SerializeField] AIBehavior behaviorCopy;
    protected override void ActivationAction()
    {
        behaviorCopy = DeepCopy(behavior.savedBehavior);
        SetChildBehavior(behaviorCopy);
    }

    public static T DeepCopy<T>(T obj)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", nameof(obj));
        }

        // Handle null object case
        if (ReferenceEquals(obj, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}
