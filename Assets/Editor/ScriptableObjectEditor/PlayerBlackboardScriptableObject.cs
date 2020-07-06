using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class PlayerBlackboardScriptableObject : Editor
{
    [SerializeField]
    string PrefabName;

    [MenuItem("SciptableObject/Create/HeroBlackboard")]
    public static PlayerBlackboard Create()
    {
        PlayerBlackboard asset = ScriptableObject.CreateInstance<PlayerBlackboard>();
        AssetDatabase.CreateAsset(asset, "Assets/Prefabs/BlackBoard/PlayerBlackboard.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }
}