using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager _saveManagerInstance;             //インスタンスを保持する変数

    private Vector2 _savePosition = Vector2.zero;
    private void Awake()
    {
        // インスタンスがまだ存在しない場合、自分自身をインスタンスとする
        if (_saveManagerInstance == null)
        {
            _saveManagerInstance = this;
        }
        else
        {
            // すでにインスタンスが存在する場合は、自分自身を破棄する
            Destroy(gameObject);
        }

    }

    public Vector2 SavePoint
    {
        get { return _savePosition; }
        set { _savePosition = value; }
    }

    private void Start()
    {

    }

    private void Update()
    {

    }
}
