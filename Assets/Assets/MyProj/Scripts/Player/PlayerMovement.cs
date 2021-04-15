using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float moveSpeed;

    [Header("Global Values")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private Vector2 _movement;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        
        if (_movement.x != 0 && _movement.y != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            _movement.x *= 0.7F;
            _movement.y *= 0.7F;
        }
        
        if (animator != null)
        {
            if (_movement.x != 0F || _movement.y != 0F)
            {

                animator.SetFloat("Hor", _movement.x);
                animator.SetFloat("Ver", _movement.y);
            }

            animator.SetFloat("Speed", _movement.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + _movement * moveSpeed * Time.fixedDeltaTime);
    }
}
