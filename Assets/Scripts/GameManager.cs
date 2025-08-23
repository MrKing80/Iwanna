using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// �Q�[���̑S�̓I�ȊǗ�������X�N���v�g
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;             //�C���X�^���X��ێ�����ϐ�
    private PlayerInput _playerAction = default;

    private void Awake()
    {
        // �C���X�^���X���܂����݂��Ȃ��ꍇ�A�������g���C���X�^���X�Ƃ���
        if (instance == null)
        {
            instance = this;
            
            // ���̃V�[���Ɉړ����Ă��V���O���g����j�����Ȃ�
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ���łɃC���X�^���X�����݂���ꍇ�́A�������g��j������
            Destroy(gameObject);
        }

        _playerAction = new PlayerInput();
        _playerAction.Enable();

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        Retry();
    }

    private void OnDestroy()
    {
        _playerAction.Disable();
    }

    /// <summary>
    /// ���g���C���s������
    /// </summary>
    private void Retry()
    {
        //���g���C�L�[�������ꂽ��
        if (_playerAction.Player.Retry.WasPressedThisFrame())
        {
            //���݂̃V�[�������[�h
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
