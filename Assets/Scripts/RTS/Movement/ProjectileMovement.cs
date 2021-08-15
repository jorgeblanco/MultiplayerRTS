using Mirror;
using UnityEngine;

namespace RTS.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileMovement : NetworkBehaviour
    {
        [SerializeField] private float launchForce = 10f;
        [SerializeField] private float timeToLive = 5f;
        
        private Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.velocity = transform.forward * launchForce;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Invoke(nameof(DestroySelf), timeToLive);
        }

        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
