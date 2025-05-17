using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

namespace Prototypes.Intros
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField, Scene] private string _sceneToLoad;

        public void GoToNextScene()
        {
            SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Single);
        }
    }
}
