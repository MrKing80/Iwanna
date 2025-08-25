using UnityEngine;

/// <summary>
/// 銃弾を管理するスクリプト
/// </summary>
public class BulletController : MonoBehaviour
{
    private Rigidbody2D _bulletRigidbody2D = default;
    private float _bulletSpeed = 300f;                  //銃弾の発射速度
    private const string PLAYER_TAG = "Player";
    private void Awake()
    {
        _bulletRigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 銃弾の移動処理
    /// </summary>
    /// <param name="moveX">移動方向(プレイヤーの向き)</param>
    public void BulletMove(float moveX)
    {
        _bulletRigidbody2D.AddForce(Vector2.one * moveX * _bulletSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(PLAYER_TAG))
        {
            return;
        }
        this.gameObject.SetActive(false);
    }
}
