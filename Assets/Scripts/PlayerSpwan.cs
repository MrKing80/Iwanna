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
        Instantiate(_player,_myPosition,Quaternion.identity);
    }
}
