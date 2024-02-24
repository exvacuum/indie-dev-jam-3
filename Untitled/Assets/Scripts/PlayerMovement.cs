using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 1.0f;
    [SerializeField]
    private float _jumpImpulse = 5.0f;
    [SerializeField]
    private float _groundedNormalThreshold = 0.8f;
    [SerializeField]
    private ThrowableObject _heldObject;

    private Rigidbody2D _rigidbody2D;
    private float _horizontalVelocity;
    private bool _grounded;
    private Animator _animator;
    private Transform _ladder;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _ladder = GetComponentInChildren<Transform>();
    }

    private void Update()
    {
        var vector2 = _rigidbody2D.position;
        vector2.x += _horizontalVelocity * Time.deltaTime;
        _rigidbody2D.position = vector2;
        _animator.SetFloat("Speed", Mathf.Abs(_horizontalVelocity * Time.deltaTime));

        bool flipped = _horizontalVelocity > 0;
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        _ladder.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));

        if (_heldObject != null)
        {
            _heldObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.x);
        }
    }

    private void OnWalk(InputValue value)
    {
        var walkValue = value.Get<float>();
        _horizontalVelocity = walkValue * _movementSpeed;
    }

    private void OnClimb(InputValue value)
    {
        //TODO
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed && _grounded)
        {
            _rigidbody2D.AddForce(new Vector2(0, _jumpImpulse), ForceMode2D.Impulse);
            _grounded = false;
        }
    }

    private void OnThrow(InputValue value)
    {
        //TODO
        if (value.isPressed)
        {
            _heldObject.Throw(Mathf.Sign(_horizontalVelocity));
            this._heldObject = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        foreach (var contact in other.contacts)
        {
            if (!(contact.normal.y > _groundedNormalThreshold))
                continue;
            _grounded = true;
            break;
        }
    }
}
