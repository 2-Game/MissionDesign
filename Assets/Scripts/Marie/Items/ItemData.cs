using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    public Guid UID;
    public GameObject prefab;
    
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (UID == Guid.Empty)
        {
            UID = Guid.NewGuid();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
