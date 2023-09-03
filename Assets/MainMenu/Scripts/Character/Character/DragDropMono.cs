using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropMono : MonoBehaviour
{
    private Transform dragParent;
    
    [SerializeField] private float dragOffset,currentDragOffset,moveUpOffset,currentMoveUpOffset,addConditionOffset;
    [SerializeField] private bool isCreated,isMoveUp,addCondition;
}
