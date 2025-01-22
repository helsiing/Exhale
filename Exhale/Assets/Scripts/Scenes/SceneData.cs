using System;
using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Exhale.Scenes
{
    [Serializable]
    public class SceneData
    {
        [ReadOnly] public string Name;

        [ReadOnly] public string Path;

#if UNITY_EDITOR
        [HideInInspector] public SceneSetup Setup;
#endif
    }
}