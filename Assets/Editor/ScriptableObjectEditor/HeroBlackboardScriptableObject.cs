using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class HeroBlackboardScriptableObject : Editor
{ 
    [MenuItem("SciptableObject/Create/HeroBlackboard")]
    public static HeroBlackboard Create()
    {
        HeroBlackboard asset = ScriptableObject.CreateInstance<HeroBlackboard>();
        AssetDatabase.CreateAsset(asset, "Assets/Resources/BB/HeroBlackboard.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }
}