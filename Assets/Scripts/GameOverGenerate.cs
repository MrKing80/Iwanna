using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームオーバーUIを管理するスクリプト
/// </summary>
public class GameOverGenerate : MonoBehaviour
{
    // ゲームオーバー時に表示するUI（Image）
    [SerializeField] private Image _gameOverImage = default;

    /// <summary>
    /// ゲームオーバーUIを表示する処理
    /// </summary>
    public void GameOverUI()
    {
        // Imageコンポーネントを有効化して表示
        _gameOverImage.enabled = true;
    }
}
