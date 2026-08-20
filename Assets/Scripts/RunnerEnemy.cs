using UnityEngine;

public class RunnerEnemy : Enemy
{
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float collisionDamage = 5f;

    private bool hasSeenPlayer = false;
    private Vector3 moveDirection;

    protected override void Act()
    {
        if (_player == null) return;

        if (!hasSeenPlayer && _distanceToPlayer <= detectionRadius * detectionRadius)
        {
            hasSeenPlayer = true;
            moveDirection = (transform.position - _player.transform.position).normalized;
        }

        if (hasSeenPlayer)
        {
            _agent.SetDestination(transform.position + moveDirection);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player hitPlayer))
        {
            DealDamage(hitPlayer);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
