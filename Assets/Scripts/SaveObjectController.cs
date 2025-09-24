using UnityEngine;

public class SaveObjectController : MonoBehaviour
{
    private Animator _animator = default;
    private const string APPLY_SAVE_ANIMATION = "ApplySave";
    private const string BULLET_TAG = "Bullet";

    private void Start()
    {
        _animator = this.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(BULLET_TAG))
        {
            _animator.SetTrigger(APPLY_SAVE_ANIMATION);

            SaveManager._saveManagerInstance.SavePoint = this.transform.position;
        }
    }
}
