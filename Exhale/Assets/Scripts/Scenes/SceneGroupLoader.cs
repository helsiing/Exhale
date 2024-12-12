using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Exhale.Scenes
{
    public class SceneGroupLoader : MonoBehaviour
    {
        [SerializeField] private List<SceneData> sceneList = new();

        private void Start()
        {
            foreach (SceneData sceneData in sceneList)
            {
                bool alreadyLoaded = false;
                for (int i = 0; i < SceneManager.sceneCount; i++)
                    if (SceneManager.GetSceneAt(i).name + ".unity" == sceneData.Name)
                        alreadyLoaded = true;

                if (!alreadyLoaded)
                {
                    SceneManager.LoadScene(sceneData.Path, LoadSceneMode.Additive);
                }
            }
        }
        
#if UNITY_EDITOR
        [Button("Update")]
        public void UpdateSceneSetup()
        {
            var currentScenes = EditorSceneManager.GetSceneManagerSetup();

            foreach (var sceneSetup in currentScenes)
            {
                // check if scene already exists
                if (sceneList.Exists(x => x.Path == sceneSetup.path)) continue;
                
                var sceneData = new SceneData {
                    Name = Path.GetFileName(sceneSetup.path),
                    Path = sceneSetup.path,
                    Setup = sceneSetup
                };
                sceneList.Add(sceneData);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button("Load")]
        public void RestoreSceneSetup()
        {
            EditorSceneManager.RestoreSceneManagerSetup(sceneList.Select(sceneData => sceneData.Setup).ToArray());
        }
#endif
    }
}
