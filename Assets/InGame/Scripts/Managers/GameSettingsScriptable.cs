using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsScriptable : Singleton<GameSettingsScriptable>
{
    public RelationShipUI RelationShipsPrefab;
    public Sprite happySprite,angrySprite,notrSprite,allySprite,warSprite; 
    public Sprite nextRoundSprite,waitingSprite;    

}
