using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CrabController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public float animationSpeed = 2f;

    private CharacterController controller;
    private Animator animator;
    private InputAction moveAction;
    private Vector2 moveInput;
    private bool isMoving;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.speed = animationSpeed;
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
    }

    void Update()
    {
        if (moveAction != null)
            moveInput = moveAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        bool wasMoving = isMoving;
        isMoving = move.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            // Rotate to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move
            controller.Move(move.normalized * moveSpeed * Time.deltaTime);
        }

        // Animation transitions
        if (animator != null)
        {
            animator.speed = animationSpeed;
            if (isMoving && !wasMoving)
                animator.SetTrigger("Walk_Cycle_1");
            else if (!isMoving && wasMoving)
                animator.SetTrigger("Rest_1");
        }

        // Apply gravity
        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }
}
