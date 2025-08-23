using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// プレイヤーの死亡処理を行うスクリプト
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject _blood = default;
    private const int MAX_GENERATE_OBJECTS = 75;
    private Vector2 _playerPosition = Vector2.zero;
    private const float MIN_FORCE = 5f;
    private const float MAX_FORCE = 12.5f;
    private const string KILLER_TAG = "Killer";     //プレイヤーを殺すオブジェクトのタグ名
    private bool _isDeath = false;                  //死んだかどうか
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        DeathPlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //キラーオブジェクトに触れたら
        if(collision.gameObject.CompareTag(KILLER_TAG))
        {
            //死亡判定をtrueに変更
            _isDeath = true;
        }
    }

    /// <summary>
    /// カメラ外に出たら処理を行う
    /// </summary>
    private void OnBecameInvisible()
    {   
        //死亡判定をtrueに変更
        _isDeath = true;
    }

    /// <summary>
    /// 死亡したときの処理を行う
    /// </summary>
    private void DeathPlayer()
    {
        if (_isDeath)
        {
            print("あなたは死にました");
            GenetateBlood();
            this.gameObject.SetActive(false);
        }
    }

    private void GenetateBlood()
    {
        GameObject generatedBlood = default;

        for (int i = 0; i < MAX_GENERATE_OBJECTS; i++)
        {
            _playerPosition = transform.position;

            generatedBlood = Instantiate(_blood, _playerPosition,Quaternion.identity);

            Rigidbody2D bloodrig2D = generatedBlood.GetComponent<Rigidbody2D>();

            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            float randomSpeed = Random.Range(MIN_FORCE,MAX_FORCE);

            bloodrig2D.AddForce(randomDirection * randomSpeed, ForceMode2D.Impulse);
        }
    }
}
