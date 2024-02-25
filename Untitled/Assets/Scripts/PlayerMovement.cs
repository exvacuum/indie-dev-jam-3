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
    [SerializeField]
    private ThrowableObject _heldLadder;
    [SerializeField]
    private GameObject _placeableLadder;

    private Rigidbody2D _rigidbody2D;
    private float _horizontalVelocity;
    private bool _grounded;
    private bool _playerTouchingLadder;
    private bool _ladderPlaced = false;
    private ThrowableObject _touchedObject;
    private GameObject _ladderObject;
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
        if (value.isPressed && _heldObject != null && !Input.GetKey(KeyCode.DownArrow))
        {
            _heldObject.Throw(Mathf.Sign(_horizontalVelocity));
            _heldObject = null;
        }
        else if (_playerTouchingLadder && _heldObject == null)
        {
            if (_ladderPlaced)
            {
                _ladderPlaced = false;
                Destroy(_ladderObject);
                _ladderObject = Instantiate(_heldLadder.gameObject, new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.x), Quaternion.identity);
                _heldObject = _ladderObject.gameObject.GetComponent<ThrowableObject>();
            }
            else
            {
                _heldObject = _touchedObject;
            }
        }
        else if (_heldObject && Input.GetKey(KeyCode.DownArrow))
        {
            Destroy(_heldObject.gameObject);
            _heldObject = null;
            _ladderObject = Instantiate(_placeableLadder.gameObject, new Vector2(_rigidbody2D.position.x, _rigidbody2D.position.y + (float)0.5), Quaternion.identity);
            _ladderPlaced = true;
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

        if (other.gameObject.GetComponent<ThrowableObject>() != null)
        {
            _playerTouchingLadder = true;
            _touchedObject = other.gameObject.GetComponent<ThrowableObject>();
        }
        else
        {
            _playerTouchingLadder = false;
            _touchedObject = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<ThrowableObject>() != null)
        {
            _playerTouchingLadder = true;
            _touchedObject = other.gameObject.GetComponent<ThrowableObject>();
        }
        else
        {
            _playerTouchingLadder = false;
            _touchedObject = null;
        }
    }
}
