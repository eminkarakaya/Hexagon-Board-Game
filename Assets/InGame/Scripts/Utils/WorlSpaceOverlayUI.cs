using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceOverlayUI : MonoBehaviour {
 
    public UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;
 
    public bool apply = false;
 
    private void Start()
    {
        if (apply)
        {
            apply = false;
            Graphic[] images = GetComponentsInChildren<Graphic>();
            foreach (var image in images)
            {
                Material existingGlobalMat = image.materialForRendering;
                Material updatedMaterial = new Material(existingGlobalMat);
                updatedMaterial.SetInt("unity_GUIZTestMode", (int) comparison);
                image.material = updatedMaterial;
            }
        }
    }
}
