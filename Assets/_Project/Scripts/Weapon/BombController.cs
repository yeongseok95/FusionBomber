using UnityEngine;
using Fusion;

public class BombController : NetworkBehaviour
{
    [SerializeField] private float fuseTime = 3f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask destructibleLayer;
    
    [Networked] private TickTimer fuseTimer { get; set; }
    private int _currentPower;
    private PlayerController _owner;

    public void Setup(int power, PlayerController owner)
    {
        _currentPower = power;
        _owner = owner;
    }

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
        // 십자 방향 체크
        CheckExplosionDirection(Vector3.zero);
        CheckExplosionDirection(Vector3.forward);
        CheckExplosionDirection(Vector3.back);
        CheckExplosionDirection(Vector3.left);
        CheckExplosionDirection(Vector3.right);

        if (_owner != null) _owner.OnBombExploded();
        Runner.Despawn(Object);
    }

    private void CheckExplosionDirection(Vector3 direction)
    {
        int range = direction == Vector3.zero ? 0 : _currentPower;
        
        for (int i = 0; i <= range; i++)
        {
            if (direction == Vector3.zero && i > 0) break;
            
            Vector3 checkPos = transform.position + (direction * i);
            
            // 일반 벽(파괴 불가) 체크
            if (i > 0 && Physics.Raycast(checkPos - direction + Vector3.up, direction, 1f, wallLayer)) break;

            // 파괴 가능한 벽 체크
            RaycastHit hitInfo;
            if (i > 0 && Physics.Raycast(checkPos - direction + Vector3.up, direction, out hitInfo, 1f, destructibleLayer))
            {
                var destructible = hitInfo.collider.GetComponent<DestructibleBlock>();
                if (destructible != null) destructible.RequestDestruction();
                break;
            }

            // 데미지 판정 (플레이어 등)
            Collider[] hits = Physics.OverlapSphere(checkPos, 0.4f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    var player = hit.GetComponent<PlayerController>();
                    if (player != null) player.OnExplosionHit();
                }
            }
        }
    }
}