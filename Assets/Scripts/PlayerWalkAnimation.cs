using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerWalkAnimation : MonoBehaviour
{
    [Header("Drag the limb objects here")]
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;

    [Header("Walking Animation")]
    [Tooltip("Number of walking cycles per second.")]
    [SerializeField, Min(0.1f)]
    private float walkFrequency = 1.5f;

    [SerializeField, Range(0f, 60f)]
    private float legSwingAngle = 25f;

    [SerializeField, Range(0f, 60f)]
    private float armSwingAngle = 20f;

    [SerializeField, Min(0.1f)]
    private float smoothness = 12f;

    [Header("Local Swing Axes")]
    [Tooltip("Change these if a limb swings sideways.")]
    [SerializeField] private Vector3 leftLegAxis = Vector3.right;
    [SerializeField] private Vector3 rightLegAxis = Vector3.right;
    [SerializeField] private Vector3 leftArmAxis = Vector3.right;
    [SerializeField] private Vector3 rightArmAxis = Vector3.right;

    private CharacterController controller;

    private Quaternion leftLegRest;
    private Quaternion rightLegRest;
    private Quaternion leftArmRest;
    private Quaternion rightArmRest;

    private float walkPhase;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Remember the starting pose.
        leftLegRest = GetRestRotation(leftLeg);
        rightLegRest = GetRestRotation(rightLeg);
        leftArmRest = GetRestRotation(leftArm);
        rightArmRest = GetRestRotation(rightArm);
    }

    private Quaternion GetRestRotation(Transform limb)
    {
        return limb != null
            ? limb.localRotation
            : Quaternion.identity;
    }

    private void LateUpdate()
    {
        // Use actual movement, not mouse input.
        Vector3 horizontalVelocity = controller.enabled
            ? controller.velocity
            : Vector3.zero;

        horizontalVelocity.y = 0f;

        bool isWalking =
            controller.enabled &&
            controller.isGrounded &&
            horizontalVelocity.sqrMagnitude > 0.01f;

        float swing = 0f;

        if (isWalking)
        {
            walkPhase +=
                Time.deltaTime * walkFrequency * Mathf.PI * 2f;

            swing = Mathf.Sin(walkPhase);
        }
        else
        {
            // Return to the standing pose when not walking.
            walkPhase = 0f;
        }

        float blend =
            1f - Mathf.Exp(-smoothness * Time.deltaTime);

        // Legs move in opposite directions.
        AnimateLimb(
            leftLeg,
            leftLegRest,
            leftLegAxis,
            swing * legSwingAngle,
            blend
        );

        AnimateLimb(
            rightLeg,
            rightLegRest,
            rightLegAxis,
            -swing * legSwingAngle,
            blend
        );

        // Arms move opposite to the leg on the same side.
        AnimateLimb(
            leftArm,
            leftArmRest,
            leftArmAxis,
            -swing * armSwingAngle,
            blend
        );

        AnimateLimb(
            rightArm,
            rightArmRest,
            rightArmAxis,
            swing * armSwingAngle,
            blend
        );
    }

    private void AnimateLimb(
        Transform limb,
        Quaternion restRotation,
        Vector3 localAxis,
        float angle,
        float blend)
    {
        if (limb == null)
        {
            return;
        }

        if (localAxis.sqrMagnitude < 0.001f)
        {
            localAxis = Vector3.right;
        }

        Quaternion targetRotation =
            restRotation *
            Quaternion.AngleAxis(angle, localAxis.normalized);

        limb.localRotation = Quaternion.Slerp(
            limb.localRotation,
            targetRotation,
            blend
        );
    }
}