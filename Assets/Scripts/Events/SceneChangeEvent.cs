using UnityEngine;

public struct SceneChangeEvent
{
    public string scene;
    public SceneChangeEvent(string targetScene)
    {
        scene = targetScene;
    }
}
