using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    [Header("角色属性")]
    public float speed = 200f;
    public float jumpForce = 15f;

    [Header("角色状态")]
    public bool isGrounded = false;
    public bool isLanded = false;
    public bool isJumping = false;
    public float coyoteTime = 0.1f;
    public float coyoteTimeCounter = 0f;
    public float jumpBufferTime = 0.2f;
    public float jumpBufferTimeCounter = 0f;

    [Header("检测参数")]
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public Vector2 groundCheckOffset;
    private Vector2 footPos;

    private void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 触发跳跃
        playerInput.Player.Jump.started += Jump;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void Start()
    {
        
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        // 检测是否
        moveInput = playerInput.Player.Move.ReadValue<Vector2>(); // 获取移动输入
    }

    private void FixedUpdate()
    {
        Move();
        CheckGround();
        // Fall();
        JumpBuffer();
    }

    #region 角色移动
    private void Move()
    {
        // 移动
        rb.velocity = new Vector2(moveInput.x * speed * Time.fixedDeltaTime, rb.velocity.y);

        // 翻转
        if (moveInput.x > 0)
        {
            Flip(true);
        }
        else if (moveInput.x < 0)
        {
            Flip(false);
        }
    }

    private void Flip(bool faceRight)
    {
        Vector3 scale = transform.localScale;

        if (faceRight)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }
    #endregion

    #region 角色跳跃
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isLanded && isGrounded)
        {
            Debug.Log("地面跳跃");
            OnJump();
        }
        else if(isLanded && !isJumping && coyoteTimeCounter > 0)
        {
            Debug.Log("土狼跳跃");
            OnJump();
        }
        else if(rb.velocity.y < 0)
        {
            jumpBufferTimeCounter = Time.fixedTime + jumpBufferTime;
        }
        
        DrawFootPos();
    }

    private void OnJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteTimeCounter = 0;
        isJumping = true;
        isLanded = false;
        jumpBufferTimeCounter = 0;
    }

    private void DrawFootPos()
    {
        // 绘制角色脚下的点
        footPos = new Vector2(transform.position.x, transform.position.y - 0.4f);
    }

    private void JumpBuffer()
    {
        if (jumpBufferTimeCounter > Time.fixedTime && isGrounded)
        {
            Debug.Log("缓冲跳跃");
            OnJump();
        }
    }

    // private void Fall()
    // {
        // if (rb.velocity.y < 0)
        // {
        //     rb.gravityScale = 2f;
        // }
        // else
        // {
        //     rb.gravityScale = 1f;
        // }
    // }

    private void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + groundCheckOffset, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            isLanded = true;
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawWireSphere(footPos, 0.05f);
    }
    #endregion
}
