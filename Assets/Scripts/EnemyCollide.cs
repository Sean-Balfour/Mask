using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollide : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Enemy enemy;

    [SerializeField]
    private AudioClip bite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(AttackPlayer());
        }
    }

    private IEnumerator AttackPlayer()
    {
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.1f);

        enemy.FindFurthestNodeFromPlayer();
        AudioController.Instance.Bite();
        player.Die();
    }
}
