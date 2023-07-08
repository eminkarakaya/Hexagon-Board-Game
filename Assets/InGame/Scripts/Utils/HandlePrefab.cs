using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandlePrefab : MonoBehaviour
{
    public GameObject prefab;
    public UnityEvent<GameObject> createBtn;
}
