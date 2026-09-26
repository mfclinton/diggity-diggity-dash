using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core.Classes
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            // Singleton Exists Check
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            // Singleton Setup
            Instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnEnable()
        {
            if (Instance != this)
                return;
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void OnDisable()
        {
            if (Instance != this)
                return;
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected virtual void OnDestroy()
        {
            if (Instance != this)
                return;
            
            Instance = null;
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
        }
    }
}