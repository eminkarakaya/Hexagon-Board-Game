using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScriptableObjectSinleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                T [] results = Resources.FindObjectsOfTypeAll<T>();
                if(results.Length == 0)
                {
                    Debug.LogError("Singleton lengt = 0");
                    return null;
                }
                if(results.Length > 1)
                {
                    Debug.LogError("Singleton lengt > 1");
                    return null;
                }
                _instance = results[0];
                _instance.hideFlags = HideFlags.DontUnloadUnusedAsset; 
                
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
