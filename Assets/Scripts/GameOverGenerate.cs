using UnityEngine;
using UnityEngine.UI;

public class GameOverGenerate : MonoBehaviour
{
    [SerializeField] private Image _gameOverImage = default;

    public void GameOverUI()
    {
        _gameOverImage.enabled = true;
    }
}
