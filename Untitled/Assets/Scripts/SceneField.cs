using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SceneField
{
    [SerializeField]
    private Object _sceneAsset;
    
    [SerializeField]
    private string _sceneName;
    public string SceneName => _sceneName;

    public static implicit operator string(SceneField obj)
    {
        return obj.SceneName;
    }
}
