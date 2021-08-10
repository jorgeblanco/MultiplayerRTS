using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Movement
{
    [RequireComponent (typeof (NavMeshAgent))]
    [RequireComponent (typeof (Animator))]
    public class CharacterLocomotion : NetworkBehaviour
    {
        [SerializeField] private float moveThreshold = 0.5f;
        [SerializeField] private float smoothFactor = 0.15f;
        // [SerializeField] private int velocitySmoothSteps = 10;
        [SerializeField] private float rotationSpeed = 2f;
        // [SerializeField] private float rotationTolerance;
        
        private Animator _animator;
        private NavMeshAgent _agent;
        // private CharacterLookAt _characterLookAt;
        private Vector3 _worldDeltaPosition;
        private Vector2 _deltaPosition;
        private Vector2 _smoothDeltaPosition = Vector2.zero;
        [SyncVar] private Vector2 _velocity = Vector2.zero;
        [SyncVar] private bool _shouldMove;
        private bool _shouldRotate;
        private Vector3 _lookAt;

        private float _sqrAgentRadius;
        private float _sqrMoveThreshold;
        
        private static readonly int Move = Animator.StringToHash("isMoving");
        private static readonly int VelX = Animator.StringToHash("velX");
        private static readonly int VelY = Animator.StringToHash("velY");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        // private static readonly int IsSearching = Animator.StringToHash("isSearching");
        private static readonly int IsDead = Animator.StringToHash("isDead");

        private void Awake ()
        {
            _animator = GetComponent<Animator> ();
            _agent = GetComponent<NavMeshAgent> ();
            // _characterLookAt = GetComponent<CharacterLookAt>();
            
            // Don’t update position automatically
            _agent.updatePosition = false;
            _agent.updateRotation = false;

            _sqrAgentRadius = _agent.radius * _agent.radius;
            _sqrMoveThreshold = moveThreshold * moveThreshold;
        }

        private void OnEnable()
        {
            _lookAt = transform.forward;
        }

        private void Update ()
        {
            if (_animator.GetBool(IsDead))
            {
                _agent.isStopped = true;
                return;
            }

            if (isServer)  // TODO: Extract to a ServerUpdate method
            {
                UpdatePositionDeltas();

                UpdateVelocity();
            
                _shouldMove = !_agent.isStopped && _velocity.sqrMagnitude > _sqrMoveThreshold && _agent.remainingDistance > _agent.radius;
                // TODO: Update animator to have a base Move state and attacks are overrides
                // TODO: Remove the animator check here and disable the agent when IsAttacking==true instead
                _shouldMove = _shouldMove && !_animator.GetBool(IsAttacking);

                if (_shouldMove)
                {
                    _lookAt = _agent.steeringTarget;
                }

                // Rotate agent
                RotateToward(_lookAt);

                // if (_characterLookAt)
                //     _characterLookAt.lookAtTargetPosition = _agent.steeringTarget + transform.forward;
                
                // Pull character towards agent
                if (_worldDeltaPosition.sqrMagnitude > _sqrAgentRadius)
                {
                    transform.position = _agent.nextPosition - 0.9f * _worldDeltaPosition;
                    _shouldMove = true;
                }
            }

            // Update animation parameters
            _animator.SetBool(Move, _shouldMove);
            _animator.SetFloat (VelX, _velocity.x);
            _animator.SetFloat (VelY, _velocity.y);
        }

        private void UpdateVelocity()
        {
            // Update velocity only if time advances
            if (!(Time.deltaTime > Mathf.Epsilon)) return;
            
            var smooth = Mathf.Min(1.0f, Time.deltaTime / smoothFactor);
            _velocity = Vector2.Lerp(_velocity, _smoothDeltaPosition / Time.deltaTime, smooth);
        }

        private void UpdatePositionDeltas()
        {
            var thisTransform = transform;
            _worldDeltaPosition = _agent.nextPosition - thisTransform.position;

            // Map 'worldDeltaPosition' to local space
            _deltaPosition.x = Vector3.Dot(thisTransform.right, _worldDeltaPosition);
            _deltaPosition.y = Vector3.Dot(thisTransform.forward, _worldDeltaPosition);

            // Low-pass filter the deltaMove
            var smooth = Mathf.Min(1.0f, Time.deltaTime / smoothFactor);
            _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, _deltaPosition, smooth);
        }

        private void OnAnimatorMove ()
        {
            if (isServer)
            {
                // Update position to agent position
                transform.position = _agent.nextPosition;
                
                // Update position based on animation movement using navigation surface height
                // var position = _animator.rootPosition;
                // position.y = _agent.nextPosition.y;
                // transform.position = position;
            }
        }

        public void SetRotationToward(Vector3 point)
        {
            _lookAt = point;
        }

        public void SetRotationToward(Transform target)
        {
            SetRotationToward(target.position);
        }

        private bool RotateToward(Vector3 point)
        {
            var thisTransform = transform;
            var direction = (point - thisTransform.position);

            if (direction == Vector3.zero)
            {
                return false;
            }
            
            thisTransform.rotation = Quaternion.Slerp(
                thisTransform.rotation,
                Quaternion.LookRotation(direction), 
                Time.deltaTime * rotationSpeed
            );

            return true;
        }

        // public void Search(bool isSearching)
        // {
        //     _animator.SetBool(IsSearching, isSearching);
        // }
    }
}