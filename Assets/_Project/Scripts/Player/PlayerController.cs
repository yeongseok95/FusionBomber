using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private NetworkPrefabRef bombPrefab;
    
    private Vector3 _targetPosition;
    private bool _isMoving;

    public override void Spawned()
    {
        _targetPosition = transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // 이동 처리 (격자 스냅핑)
            if (!_isMoving && data.direction.magnitude > 0)
            {
                Vector3 moveDir = new Vector3(Mathf.Round(data.direction.x), 0, Mathf.Round(data.direction.y));
                if (moveDir.magnitude > 0)
                {
                    _targetPosition = transform.position + moveDir;
                    _isMoving = true;
                }
            }

            // 폭탄 설치
            if (data.isBombPressed)
            {
                Rpc_SpawnBomb(new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z)));
            }
        }

        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Runner.DeltaTime);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                transform.position = _targetPosition;
                _isMoving = false;
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_SpawnBomb(Vector3 pos)
    {
        Runner.Spawn(bombPrefab, pos, Quaternion.identity, Object.InputAuthority);
    }
}

public struct NetworkInputData : INetworkInput
{
    public Vector2 direction;
    public NetworkBool isBombPressed;
}