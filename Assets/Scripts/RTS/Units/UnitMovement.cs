using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        #region Server

        [ServerCallback]
        private void Update()
        {
            if (!_navMeshAgent.hasPath) return;
            if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance) return;
            _navMeshAgent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 destination)
        {
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
