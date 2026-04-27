using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC2 : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    Vector2 direction = new Vector2(-1f, 0f);
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    public List<Collider2D> inPosition = new List<Collider2D>();
    
    // Di chuyển NPC theo các waypoint đã định sẵn
    public Quest targetQuest;
    public Transform waypointParent;
    private Transform[] waypoints;
    private int currentWaypointIndex;
    // Di chuyển NPC sau khi hoàn thành nhiệm vụ
    //public Quest targetQuest;
    [Header("Quest Completed Waypoints")]
    public Transform questCompletedWaypointParent;
    private Transform[] questCompletedWaypoints;
    private int currentCompletedWaypointIndex;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetBool("isMoving", false);

        if (waypointParent != null)
        {
            waypoints = new Transform[waypointParent.childCount];
            for (int i = 0; i < waypointParent.childCount; i++)
            {
                waypoints[i] = waypointParent.GetChild(i);
            }
        }

        if (questCompletedWaypointParent != null)
        {
            questCompletedWaypoints = new Transform[questCompletedWaypointParent.childCount];
            for (int i = 0; i < questCompletedWaypointParent.childCount; i++)
            {
                questCompletedWaypoints[i] = questCompletedWaypointParent.GetChild(i);
            }
        }
    }
    void FixedUpdate()
    {
        bool isQuestDone = false;

        if (targetQuest == null)
        {
            Debug.LogWarning("NPC2: Biến 'targetQuest' đang bị bỏ trống! Hãy kéo file Quest vào ô này.");
        }
        else if (QuestController.Instance == null)
        {
            Debug.LogWarning("NPC2: Không tìm thấy QuestController trong Scene!");
        }
        else
        {
            // Chỉ trả về true khi nhiệm vụ ĐÃ ĐƯỢC TRẢ (HandIn) thông qua hội thoại
            isQuestDone = QuestController.Instance.IsQuestHandedIn(targetQuest.questID);
        }
        
        //MoveToWaypoint();
        if (isQuestDone)
        {
            MoveToQuestCompletedWaypoint(); // Đi tới điểm mới nếu Quest xong
        }
        else
        {
            MoveToWaypoint(); // Đi tới điểm mặc định nếu Quest chưa xong (hoặc không có Quest)
        }
    }
    void MoveToWaypoint()
    {
        if (waypoints.Length == 0) return;
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
        direction = targetWaypoint.position - transform.position;
        animator.SetBool("isMoving", true);
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            animator.SetBool("isMoving", false);
        }
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
    void MoveToQuestCompletedWaypoint()
    {
        if (questCompletedWaypoints == null || questCompletedWaypoints.Length == 0) return;
        
        Transform targetWaypoint = questCompletedWaypoints[currentCompletedWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
        direction = targetWaypoint.position - transform.position;
        
        animator.SetBool("isMoving", true);
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            animator.SetBool("isMoving", false);
            Destroy(gameObject);
        }

        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
}
