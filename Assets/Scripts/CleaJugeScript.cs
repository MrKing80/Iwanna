using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// クリア判定を行うスクリプト
/// プレイヤーがゴール地点に触れるとクリアシーンへ移行する
/// </summary>
public class CleaJugeScript : MonoBehaviour
{
    private const string CLEAR_SCENE_NAME = "ClearScene"; // 遷移先シーン名
    private const string PLAYER_TAG = "Player";           // プレイヤー識別用のタグ

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 衝突したオブジェクトがプレイヤーか確認
        if (collision.gameObject.CompareTag(PLAYER_TAG))
        {
            // プレイヤーならクリアシーンへ移行
            SceneManager.LoadScene(CLEAR_SCENE_NAME);
        }
    }
}
