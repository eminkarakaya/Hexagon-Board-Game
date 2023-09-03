using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScriptable<T> : ScriptableObject where T: SingletonScriptable<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                T[] assets = Resources.LoadAll<T>("");
                foreach (var item in assets)
                {
                    
                }
                if(assets == null || assets.Length < 1)
                {
                    throw new System.Exception("Could not any sinleton scriptable object instances in the resources");
                }
                else if(assets.Length>1)
                {
                    Debug.LogWarning("Muitiple instances found");
                }                
                instance = assets[0];
            }
            return instance;
        }
    }
}
