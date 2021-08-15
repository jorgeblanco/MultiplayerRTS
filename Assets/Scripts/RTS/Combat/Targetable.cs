using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class Targetable : NetworkBehaviour
    {
        [SerializeField] private Transform aimOffset;

        public Transform GetAimOffset()
        {
            return aimOffset;
        }
    }
}
