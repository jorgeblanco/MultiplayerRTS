using System;
using Mirror;
using RTS.Combat;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Units
{
    public class UnitAttacking : NetworkBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float fireRange = 5f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float rotateSpeed = 20f;
        
        private Targeter _targeter;
        private float _nextFireTime;

        private void Awake()
        {
            _targeter = GetComponent<Targeter>();
        }

        [ServerCallback]
        private void Update()
        {
            if (!CanFireAtTarget()) return;
            var targetRotation = Quaternion.LookRotation(
                _targeter.Target.transform.position - transform.position
            );
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
            _nextFireTime = Time.time + 1 / fireRate;

            var projectilePosition = projectileSpawnPoint.position;
            var projectileRotation = Quaternion.LookRotation(
                _targeter.Target.GetAimOffset().position - projectilePosition
            );

            var projectileInstance = Instantiate(projectilePrefab, projectilePosition, projectileRotation);
            NetworkServer.Spawn(projectileInstance, connectionToClient);
        }

        [Server]
        private bool CanFireAtTarget()
        {
            if (Time.time < _nextFireTime) return false;
            if (!_targeter.hasTarget) return false;
            if (_targeter.Target == null) return false;
            if ((_targeter.Target.transform.position - transform.position).sqrMagnitude > fireRange * fireRange) return false;
            return true;
        }
    }
}
