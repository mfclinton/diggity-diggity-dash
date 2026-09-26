using Game.Input.Data.Interfaces;
using UnityEngine;

namespace Game.Actions.Base
{
    public abstract class AController : MonoBehaviour
    {
        protected IInputProvider inputProvider;

        protected virtual void Awake()
        {
            inputProvider = GetComponentInParent<IInputProvider>();
        }
    }
}