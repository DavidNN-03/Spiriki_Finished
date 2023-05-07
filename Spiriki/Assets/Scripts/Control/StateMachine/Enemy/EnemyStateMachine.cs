using UnityEngine;
using UnityEngine.AI;
using Spiriki.UI;
using Spiriki.Objective;

namespace Spiriki.Control
{
    public class EnemyStateMachine : StateMachine /*The enemy statemachine. This class stores the various cariables used in the enemy's states.*/
    {
        [field: SerializeField] public float RotationSpeed { get; private set; } /*The enemy's rotation speed.*/
        [field: SerializeField] public float GuardIdleTime { get; private set; } /*How long the guard spends idle at a waypoint before moving to the next.*/
        [field: SerializeField] public float SpottingRange { get; private set; } /*How far the guard can spot the player.*/
        [field: SerializeField] public float MaxSpotAngle { get; private set; } /*The max angle at which the guard can spot the player.*/
        [field: SerializeField] public float SpottingSpeedMaxDistance { get; private set; } /*How quickly the player will be spotted when the distance between the enemy and player is equal to SpottingRange.*/
        [field: SerializeField] public float SpottingSpeedMinDistance { get; private set; } /*How quickly the player would be spotted, if the distance between the enemy and player was 0.*/
        [field: SerializeField] public float SpottingProgressLossSpeed { get; private set; } /*How quickly the player will be un-spotted when out of sight.*/
        [field: SerializeField] public Animator Animator { get; private set; } /*Reference to the enemy's Animator.*/
        [field: SerializeField] public LayerMask spottingLayerMask { get; private set; } /*The layers that the enemy can see. This should include the player and the environment.*/
        [field: SerializeField] public NavMeshAgent Agent { get; private set; } /*Reference to the enemy's navmesh agent.*/
        [field: SerializeField] public Transform EyesTransform { get; private set; } /*Transform of the GameObject marking the enemy's eyes.*/
        [field: SerializeField] public PatrolPath PatrolPath { get; private set; } /*The PatrolPath the enemy will be guarding.*/
        public ObjectiveManager ObjectiveManager { get; private set; } /*Reference to the player character.*/
        public PlayerStateMachine Player { get; private set; } /*Reference to the player character.*/
        public SpottingCanvasManager SpottingCanvasManager { get; private set; } /*Reference to the spotting canvas to display UI.*/
        public float SpottingProgressNormalized { get; private set; } /*Defines how much the player has been spotted. When 0, the player has not been spotted at all. At 1, the player has been spotted and the enemy will enter EnemySuspicionState.*/
        public int CurrentGuardPointIndex { get; set; } /*The index of the current guard-point in PatrolPath.*/

        private void Awake() /*Assigns values to Animator, Player, and SpottingCanvasManager.*/
        {
            Animator = GetComponentInChildren<Animator>();
            Player = FindObjectOfType<PlayerStateMachine>();
            SpottingCanvasManager = FindObjectOfType<SpottingCanvasManager>();
            ObjectiveManager = FindObjectOfType<ObjectiveManager>();
        }

        private void Start() /*Resets SpottingProgressNormalized and CurrentGuardPointIndex to 0. Enemy enters EnemyGuardMovingState to move to the first guard-point in PatrolPath.*/
        {
            SpottingProgressNormalized = 0f;
            CurrentGuardPointIndex = 0;

            SwitchState(new EnemyGuardMovingState(this));
        }

        public void AttemptSpotPlayer(float deltaTime) /*The enemy will attempt to spot the player by calling CheckSpotPlayer(). 
        If that function returns true, SpottingProgressNormalized is increased. If SpottingProgressNormalized is more than or equal to 1, enter EnemySuspicionState.
        Also, updates the UI.*/
        {
            if (CheckSpotPlayer())
            {
                SpottingProgressNormalized += CalculateSpottingChange(deltaTime);

                if (SpottingProgressNormalized >= 1f)
                {
                    SwitchState(new EnemySuspicionState(this));
                }
            }
            else
            {
                SpottingProgressNormalized = Mathf.Max(SpottingProgressNormalized -= SpottingProgressLossSpeed * deltaTime, 0f);
            }
            //Debug.Log(SpottingProgressNormalized);
            SpottingCanvasManager.UpdatePointer(gameObject, SpottingProgressNormalized, Color.yellow);
        }

        public bool CheckSpotPlayer() /*First it will check if the player is in range, 
        secondly it will check if the angle between the enemy's eyes and the player is small enough, 
        and lastly, the enemy will raycast towards the player and see if it hits. Returns whether the player is seen.*/
        {
            if (Vector3.Distance(EyesTransform.transform.position, Player.transform.position) > SpottingRange)
            {
                Debug.Log("Out of range");
                return false;
            }

            Vector3 toPlayer = Player.transform.position - EyesTransform.position;

            if (Vector3.Angle(EyesTransform.forward, toPlayer) > MaxSpotAngle)
            {
                Debug.Log("Angle too large");
                return false;
            }

            if (Physics.Raycast(EyesTransform.position, toPlayer, out RaycastHit hit, SpottingRange, spottingLayerMask))
            {
                if (hit.collider.gameObject != Player.gameObject)
                {
                    Debug.Log($"Raycast did not hit player. Raycast hit {hit.collider.gameObject.name}");
                }

                return hit.collider.gameObject == Player.gameObject;
            }

            return false;
        }

        public float CalculateSpottingChange(float deltaTime) /*Returns how much SpottingProgressNormalized should be incresed when the player is spotted,
        based on the distance between the player and the enemy.*/
        {
            Vector3 toPlayer = Player.transform.position - EyesTransform.position;

            float distanceNormalized = toPlayer.magnitude / SpottingRange;

            float spottingSpeed = Mathf.Lerp(SpottingSpeedMinDistance, SpottingSpeedMaxDistance, distanceNormalized) * Player.GetSpottingSpeedMultiplier();

            return spottingSpeed * deltaTime;
        }
    }
}