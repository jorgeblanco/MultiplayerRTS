using Mirror;
using RTS.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private float chaseRange = 2f;
        
        private NavMeshAgent _navMeshAgent;
        private Targeter _targeter;  // This shouldn't be a direct reference. Better use events here
        private bool _hasTargeter;
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _targeter = GetComponent<Targeter>();
            _hasTargeter = _targeter != null;
        }

        #region Server

        [ServerCallback]
        private void Update()
        {
            if (_targeter.hasTarget)
            {
                if (_targeter.Target == null)
                {
                    _targeter.ClearTarget();
                    return;
                }
                
                var targetPosition = _targeter.Target.transform.position;
                if ((targetPosition - transform.position).sqrMagnitude > chaseRange*chaseRange)
                {
                    _navMeshAgent.SetDestination(targetPosition);
                }
                else if(_navMeshAgent.hasPath)
                {
                    _navMeshAgent.ResetPath();
                }
                return;
            }
            
            if (!_navMeshAgent.hasPath) return;
            if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance) return;
            _navMeshAgent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 destination)
        {
            if (_hasTargeter) _targeter.ClearTarget();
            
            if (!NavMesh.SamplePosition(destination, out var hit, 1f, NavMesh.AllAreas))
            {
                return;
            }
            _navMeshAgent.SetDestination(hit.position);
        }
        #endregion

        #region Client
        #endregion

    }
}
