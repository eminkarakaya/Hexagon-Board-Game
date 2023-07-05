using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;

public class CivDataUI : NetworkBehaviour
{
    public Image civImage;
    public CivManager civManager;
    public CivData civData;

    internal IEnumerable<object> ToList()
    {
        throw new NotImplementedException();
    }
}
