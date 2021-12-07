using System.Collections;
using System.Collections.Generic;
using UnityEngine;
static public class GameObjectExtensions
{
    static public GameObject FindChildGameObjectWithTag(this GameObject fromGameObject, string tag)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
            if (t.gameObject.CompareTag(tag))
                return t.gameObject;
        return null;
    }
}
