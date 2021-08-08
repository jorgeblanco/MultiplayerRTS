using Mirror;
using TMPro;
using UnityEngine;

public class NetworkedPlayerData : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))]
    [SerializeField]
    private string playerName;
    [SyncVar(hook = nameof(HandleDisplayColorUpdated))]
    [SerializeField]
    private Color playerColor;

    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private MeshRenderer meshRenderer;
    
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    #region Server
    [Server]
    public void SetDisplayName(string displayName)
    {
        playerName = displayName;
    }

    [Server]
    public void SetDisplayColor(Color color)
    {
        playerColor = color;
    }

    [Command]
    private void CmdSetDisplayName(string newName)
    {
        if (newName.Length < 5)
        {
            Debug.Log($"'{newName}' is too short");
            return;
        }
        SetDisplayName(newName);
    }
    #endregion

    #region Client
    private void HandleDisplayColorUpdated(Color prevColor, Color nextColor)
    {
        meshRenderer.material.SetColor(BaseColor, nextColor);
    }

    private void HandleDisplayNameUpdated(string prevName, string nextName)
    {
        displayNameText.SetText(nextName);
    }

    [ContextMenu("Set my name")]
    public void SetPlayerName()
    {
        CmdSetDisplayName("My");
    }
    
    [ClientRpc]
    private void RpcKillPlayer()
    {
        Destroy(gameObject);
    }
    #endregion
}
