using System;

namespace RTS.Combat
{
    public class HqDeathHandler : DeathHandler
    {
        public static event Action<int> ServerOnPlayerDie;
        protected override void ServerHandleDeath()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
            base.ServerHandleDeath();
        }
    }
}