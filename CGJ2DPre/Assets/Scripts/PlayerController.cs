using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Vector2 input;
    private bool isMoving;
    private Coroutine currentMoveCoroutine;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        Debug.Log("[PlayerController] 玩家控制器初始化完成");
    }
    
    private void Update()
    {
        // 获取输入
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        Debug.Log("input.x:" + input.x);
        Debug.Log("input.y:" + input.y);

        
        // 如果有输入且当前没有在移动
        if (input != Vector2.zero && !isMoving)
        {
            animator.SetFloat("moveX", input.x);
            animator.SetFloat("moveY", input.y);

            Vector3 targetPos = transform.position;
            targetPos.x += input.x;
            targetPos.y += input.y;
            
            if (showDebugInfo)
            {
                Debug.Log($"[PlayerController] 开始移动: {transform.position} -> {targetPos}");
            }
            
            // 停止之前的协程（如果有的话）
            if (currentMoveCoroutine != null)
            {
                StopCoroutine(currentMoveCoroutine);
            }
            
            // 启动新的移动协程
            currentMoveCoroutine = StartCoroutine(Move(targetPos));
        }

        if (input == Vector2.zero && isMoving)
        {
            StopMovement();
        }

        animator.SetBool("isMoving", isMoving);
    }
    
    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"[PlayerController] 移动协程开始: {targetPos}");
        }
        
        // 移动到目标位置
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        // 确保精确到达目标位置
        transform.position = targetPos;
        
        isMoving = false;
        currentMoveCoroutine = null;
        
        if (showDebugInfo)
        {
            Debug.Log($"[PlayerController] 移动完成: {transform.position}");
        }
    }
    
    /// <summary>
    /// 停止当前移动
    /// </summary>
    public void StopMovement()
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }
        isMoving = false;
        
        if (showDebugInfo)
        {
            Debug.Log("[PlayerController] 移动已停止");
        }
    }
    
    /// <summary>
    /// 设置移动速度
    /// </summary>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        
        if (showDebugInfo)
        {
            Debug.Log($"[PlayerController] 移动速度设置为: {moveSpeed}");
        }
    }
    
    /// <summary>
    /// 获取当前是否在移动
    /// </summary>
    public bool IsMoving()
    {
        return isMoving;
    }
    
    /// <summary>
    /// 获取当前输入
    /// </summary>
    public Vector2 GetInput()
    {
        return input;
    }
}
