using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : NetworkBehaviour
{
    private NavMeshAgent _navMeshAgent;

    #region Server
    [Command]
    public void CmdMove(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }
    #endregion
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

}
