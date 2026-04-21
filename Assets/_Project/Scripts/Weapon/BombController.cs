using UnityEngine;
using Fusion;

public class BombController : NetworkBehaviour
{
    [SerializeField] private float fuseTime = 3f;
    [SerializeField] private int explosionRange = 2;
    [SerializeField] private LayerMask wallLayer;
    
    [Networked] private TickTimer fuseTimer { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            fuseTimer = TickTimer.CreateFromSeconds(Runner, fuseTime);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority && fuseTimer.Expired(Runner))
        {
            Explode();
        }
    }

    private void Explode()
    {
        // 십자 방향 체크 (중심 포함 4방향)
        CheckExplosionDirection(Vector3.zero);
        CheckExplosionDirection(Vector3.forward);
        CheckExplosionDirection(Vector3.back);
        CheckExplosionDirection(Vector3.left);
        CheckExplosionDirection(Vector3.right);

        Runner.Despawn(Object);
    }

    private void CheckExplosionDirection(Vector3 direction)
    {
        for (int i = 1; i <= (direction == Vector3.zero ? 0 : explosionRange); i++)
        {
            Vector3 checkPos = transform.position + (direction * i);
            
            // 벽 충돌 체크
            if (Physics.Raycast(checkPos + Vector3.up, Vector3.down, 1.5f, wallLayer)) break;

            // 실제 데미지 판정 로직 (OverlapSphere 등 활용)
            Collider[] hits = Physics.OverlapSphere(checkPos, 0.4f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    // 플레이어 사망 처리 로직 호출
                    Debug.Log($"{hit.name} Hit!");
                }
            }
            if (direction == Vector3.zero) break;
        }
    }
}