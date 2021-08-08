using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NetworkedPlayerMovement : NetworkBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Camera mainCamera;

    #region Server
    [Command]
    private void CmdMove(Vector3 destination)
    {
        if (!NavMesh.SamplePosition(destination, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }
        _navMeshAgent.SetDestination(hit.position);
    }
    #endregion

    #region Client
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!hasAuthority) return;
        if (!Input.GetMouseButton(1)) return;
        
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;
        CmdMove(hit.point);
    }
    #endregion

}
