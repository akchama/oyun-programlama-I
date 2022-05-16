using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    // String değeri bir Id ile tutarak performans kazanımı sağlamaktadır
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKey("left") || Input.GetKey("right"))
            _animator.SetBool(IsMoving, true);
        else
            _animator.SetBool(IsMoving, false);
    }
}