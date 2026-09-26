using EasyTransition;
using Game.Core.Classes;
using UnityEngine;

namespace Game.Managers
{
    public class SceneManager : SingletonBehaviour<SceneManager>
    {
        [SerializeField] private TransitionSettings transitionSettings;
        [SerializeField] private float transitionDuration = 1.0f;
    
        public void LoadScene(string sceneName)
        {
            TransitionManager.Instance().Transition(sceneName, transitionSettings, transitionDuration);
        }
    }
}