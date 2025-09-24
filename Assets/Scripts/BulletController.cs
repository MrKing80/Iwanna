using UnityEngine;

/// <summary>
/// 銃弾を管理するスクリプト
/// ・発射時の移動処理
/// ・衝突時の消滅処理
/// </summary>
public class BulletController : MonoBehaviour
{
    private Rigidbody2D _bulletRigidbody2D = default; // 銃弾の物理挙動
    private float _bulletSpeed = 300f;                // 銃弾の発射速度（AddForceに使用）

    private const string PLAYER_TAG = "Player";       // プレイヤーを識別するタグ

    private void Awake()
    {
        // Rigidbody2Dの参照をキャッシュ
        _bulletRigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 銃弾の移動処理
    /// </summary>
    /// <param name="moveX">移動方向（プレイヤーの向きに合わせるためのX軸値）</param>
    public void BulletMove(float moveX)
    {
        // プレイヤーの向き（moveX）に応じて力を加える
        // Vector2.one = (1,1) なので、X・Y 両方向に力が加わる
        _bulletRigidbody2D.AddForce(Vector2.one * moveX * _bulletSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤー自身には当たり判定を無効化
        if (collision.gameObject.CompareTag(PLAYER_TAG))
        {
            return;
        }

        // それ以外のオブジェクトに当たったら非アクティブ化（プール再利用を想定）
        this.gameObject.SetActive(false);
    }
}
