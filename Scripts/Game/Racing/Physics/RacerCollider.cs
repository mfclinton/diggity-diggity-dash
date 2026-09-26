using UnityEngine;

namespace Game.Racing.Physics
{
    [RequireComponent(typeof(Collider2D))]
    public class RacerCollider : MonoBehaviour
    {
        private Racer racer;
        public Racer Racer => racer;

        private void Awake()
        {
            racer = GetComponentInParent<Racer>();
        }
    }
}