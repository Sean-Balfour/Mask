using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Node startNode;
    [SerializeField] private float patrolMoveSpeed = 2.0f;
    [SerializeField] private float chaseMoveSpeed = 4.0f;
    [SerializeField] private float rotationSpeed = 15.0f;
    [SerializeField] private bool chasingPlayer = false;

    [Header("Detection")]
    [SerializeField] private float minDetectRadius = 1.5f;
    [SerializeField] private float maxDetectRadius = 6.0f;
    [SerializeField] private float guaranteedDetectRadius = 1.0f;
    [SerializeField] private float loseRadiusBonus = 1.25f;

    [Header("Gizmos")]
    [SerializeField] private bool drawDetectionGizmos = true;

    private Node currentNode = null;
    private Node targetNode = null;

    private float targetRotation;

    [SerializeField] private Animator animator;
    [SerializeField] private Player player;

    public bool isMoving = true;

    private void Start()
    {
        currentNode = startNode;

        if (currentNode != null)
        {
            transform.position = currentNode.transform.position;

            if (currentNode.nextNodes != null && currentNode.nextNodes.Count > 0)
                targetNode = currentNode.nextNodes[Random.Range(0, currentNode.nextNodes.Count)];
        }
    }

    private void Update()
    {
        if (!isMoving)
            return;

        if (currentNode == null || currentNode.nextNodes == null || currentNode.nextNodes.Count == 0 || targetNode == null)
            return;

        if (Time.timeScale == 0)
            return;

        UpdateChaseState();

        Vector3 targetPos = targetNode.transform.position;
        Vector3 beforeMove = transform.position;

        if (chasingPlayer && !player.dead)
        {
            if (player != null)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    player.transform.position,
                    chaseMoveSpeed * Time.deltaTime
                );
            }
            else
            {
                chasingPlayer = false;
                FindNearestNode();

                if (targetNode != null)
                {
                    targetPos = targetNode.transform.position;
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetPos,
                        patrolMoveSpeed * Time.deltaTime
                    );
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                patrolMoveSpeed * Time.deltaTime
            );
        }

        UpdateFacing(transform.position - beforeMove);

        if (!chasingPlayer && Vector3.Distance(transform.position, targetPos) <= 0.001f)
        {
            currentNode = targetNode;

            if (currentNode.nextNodes != null && currentNode.nextNodes.Count > 0)
                targetNode = currentNode.nextNodes[Random.Range(0, currentNode.nextNodes.Count)];
        }
    }

    private void UpdateChaseState()
    {
        if (player == null || (player != null && player.immune))
        {
            if (chasingPlayer)
            {
                chasingPlayer = false;
                FindNearestNode();
            }
            return;
        }

        Vector3 toPlayer = player.transform.position - transform.position;
        float distSq = toPlayer.sqrMagnitude;

        float guaranteedSq = guaranteedDetectRadius * guaranteedDetectRadius;
        if (distSq <= guaranteedSq)
        {
            chasingPlayer = true;
            return;
        }

        float meter01 = player.MeterNormalized;

        if (meter01 <= 0f)
        {
            if (chasingPlayer)
            {
                chasingPlayer = false;
                FindNearestNode();
            }
            return;
        }

        float detectRadius = Mathf.Lerp(minDetectRadius, maxDetectRadius, meter01);
        float loseRadius = detectRadius + loseRadiusBonus;

        float detectSq = detectRadius * detectRadius;
        float loseSq = loseRadius * loseRadius;

        if (chasingPlayer)
        {
            if (distSq > loseSq)
            {
                chasingPlayer = false;
                FindNearestNode();
            }
        }
        else
        {
            if (distSq <= detectSq)
                chasingPlayer = true;
        }
    }

    private void UpdateFacing(Vector3 moveDelta)
    {
        if (moveDelta.sqrMagnitude <= 0.0000001f)
            return;

        float angle = Mathf.Atan2(moveDelta.y, moveDelta.x) * Mathf.Rad2Deg;
        targetRotation = angle;

        float newRotation = Mathf.LerpAngle(
            transform.eulerAngles.z,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(0f, 0f, newRotation);
    }

    private void FindNearestNode()
    {
        Node[] allNodes = Resources.FindObjectsOfTypeAll<Node>();
        if (allNodes == null || allNodes.Length == 0)
            return;

        Node nearestNode = allNodes[0];
        float nearestNodeDistance = float.MaxValue;

        for (int i = 0; i < allNodes.Length; i++)
        {
            if (allNodes[i] == null) continue;

            float dist = Vector3.Distance(transform.position, allNodes[i].transform.position);
            if (dist < nearestNodeDistance)
            {
                nearestNode = allNodes[i];
                nearestNodeDistance = dist;
            }
        }

        targetNode = nearestNode;
    }

    public void FindFurthestNodeFromPlayer()
    {
        Node[] allNodes = Resources.FindObjectsOfTypeAll<Node>();
        if (allNodes == null || allNodes.Length == 0)
            return;

        Node furthestNode = allNodes[0];
        float furthestNodeDistance = float.MinValue;

        for (int i = 0; i < allNodes.Length; i++)
        {
            if (allNodes[i] == null) continue;

            float dist = Vector3.Distance(player.gameObject.transform.position, allNodes[i].transform.position);
            if (dist > furthestNodeDistance)
            {
                furthestNode = allNodes[i];
                furthestNodeDistance = dist;
            }
        }

        targetNode = furthestNode;
        transform.position = targetNode.transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDetectionGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, guaranteedDetectRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDetectRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDetectRadius);

        if (player != null)
        {
            float meter01 = player.MeterNormalized;

            float currentDetectRadius = (meter01 <= 0f)
                ? guaranteedDetectRadius
                : Mathf.Lerp(minDetectRadius, maxDetectRadius, meter01);

            Gizmos.color = chasingPlayer ? Color.cyan : Color.blue;
            Gizmos.DrawWireSphere(transform.position, currentDetectRadius);

            Gizmos.color = chasingPlayer ? Color.red : Color.gray;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }
}
