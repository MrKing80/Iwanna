using UnityEngine;

public class PlayerSpwan : MonoBehaviour
{
    [SerializeField] private GameObject _player = default;
    private Vector2 _myPosition = Vector2.zero;
    private void Awake()
    {
        _myPosition = transform.position;

    }

    private void Start()
    {
        Vector2 spawnPos;

        // SaveManagerに記録がなければ、自分の位置を初期値とする
        if (SaveManager._saveManagerInstance.SavePoint == Vector2.zero)
        {
            spawnPos = _myPosition;
            SaveManager._saveManagerInstance.SavePoint = _myPosition;
        }
        else
        {
            spawnPos = SaveManager._saveManagerInstance.SavePoint;
        }

        Instantiate(_player, spawnPos, Quaternion.identity);
    }
}
