using Mirror;
using RTS.Combat;
using UnityEngine;

namespace RTS.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileMovement : NetworkBehaviour
    {
        [SerializeField] private float launchForce = 10f;
        [SerializeField] private float timeToLive = 5f;
        [SerializeField] private int damageToDeal = 20;
        
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

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NetworkIdentity>(out var networkIdentity)
                && networkIdentity.connectionToClient == connectionToClient) return;
            if (other.TryGetComponent<Health>(out var health))
            {
                health.DealDamage(damageToDeal);
            }
            DestroySelf();
        }

        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
