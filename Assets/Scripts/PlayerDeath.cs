using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// �v���C���[�̎��S�������s���X�N���v�g
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject _blood = default;
    private const int MAX_GENERATE_OBJECTS = 75;
    private Vector2 _playerPosition = Vector2.zero;
    private const float MIN_FORCE = 5f;
    private const float MAX_FORCE = 12.5f;
    private const string KILLER_TAG = "Killer";     //�v���C���[���E���I�u�W�F�N�g�̃^�O��
    private bool _isDeath = false;                  //���񂾂��ǂ���
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        DeathPlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�L���[�I�u�W�F�N�g�ɐG�ꂽ��
        if(collision.gameObject.CompareTag(KILLER_TAG))
        {
            //���S�����true�ɕύX
            _isDeath = true;
        }
    }

    /// <summary>
    /// �J�����O�ɏo���珈�����s��
    /// </summary>
    private void OnBecameInvisible()
    {   
        //���S�����true�ɕύX
        _isDeath = true;
    }

    /// <summary>
    /// ���S�����Ƃ��̏������s��
    /// </summary>
    private void DeathPlayer()
    {
        if (_isDeath)
        {
            print("���Ȃ��͎��ɂ܂���");
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
