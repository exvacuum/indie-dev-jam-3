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
    private float _verticalVelocity;
    private bool _grounded;
    private bool _playerTouchingLadder;
    private bool _ladderPlaced;
    private ThrowableObject _touchedObject;
    private GameObject _ladderObject;
    private Animator _animator;
    private Transform _ladder;
    private static readonly int Moving = Animator.StringToHash("Moving");

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
        vector2.y += _verticalVelocity * Time.deltaTime;
        _rigidbody2D.position = vector2;

        //Set value in animator
        _animator.SetBool(Moving, Mathf.Abs(_horizontalVelocity * Time.deltaTime) > 0f);

        //If the player is holding an object, hold it above the player
        if (_heldObject)
        {
            var position = this.transform.position;
            _heldObject.transform.position = new Vector3(position.x, position.y + 1, position.x);
        }

        if (_horizontalVelocity != 0)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Sign(_horizontalVelocity);
            transform.localScale = scale;
            _ladder.localScale = scale;
        }

        if (_ladderPlaced)
        {
            if (_rigidbody2D.position.y > _ladderObject.transform.position.y + 1.5)
            {
                //bring gravity back
                _rigidbody2D.gravityScale = 1;
                _ladderObject.GetComponent<BoxCollider2D>().isTrigger = false; //make the ladder solid
            }
            else
            {
                _ladderObject.GetComponent<BoxCollider2D>().isTrigger = true; //make the ladder trigger
            }
        }
    }

    private void OnWalk(InputValue value)
    {
        var walkValue = value.Get<float>();
        _horizontalVelocity = walkValue * _movementSpeed;
    }

    private void OnClimb(InputValue value)
    {
        //If climb key is pressed, the ladder is placed, and the player is touching the ladder
        if (value.isPressed && _ladderPlaced && _playerTouchingLadder)
        {
            _ladderObject.GetComponent<BoxCollider2D>().isTrigger = true; //make the ladder trigger
            var climbValue = value.Get<float>();

            //Set vertical velocity (1 or -1 * movespeed)
            _verticalVelocity = climbValue * _movementSpeed;

            //Go up/down - also sets player to center of ladder
            _rigidbody2D.position = new Vector2(_ladderObject.gameObject.transform.position.x, _rigidbody2D.position.y);
            //Disable gravity to stop player from falling immediately
            _rigidbody2D.gravityScale = 0;
        }
        else
        {
            //We want the vertical velocity to be 0 if no climbing is occurring
            _verticalVelocity = 0;
        }
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
        //If the throw key is pressed, the player has an object, and the down key is not pressed, throw the object
        if (value.isPressed && _heldObject && !Input.GetKey(KeyCode.DownArrow))
        {
            _heldObject.Throw(Mathf.Sign(_horizontalVelocity));
            _heldObject = null;
        }
    }

    private void OnPlace(InputValue value)
    {
        //If the player is holding an object and is pressing the down arrow, place it
        if (_heldObject )
        {
            Destroy(_heldObject.gameObject); //Delete the held ladder

            _heldObject = null; //The player is no longer holding an object

            //Create a new placeable ladder
            var position = _rigidbody2D.position;
            _ladderObject = Instantiate(_placeableLadder.gameObject, new Vector2(position.x, position.y + (float)0.5), Quaternion.identity);
            //The ladder is placed
            _ladderPlaced = true;
        }
    }

    private void OnPickUp(InputValue value)
    {
        //If the player is touching the ladder, and is not holding an object
        if (_playerTouchingLadder && !_heldObject)
        {
            //If the ladder is placed
            if (_ladderPlaced)
            {
                Destroy(_ladderObject); //delete the placed ladder

                //Make a new ladder to be held
                var position = transform.position;
                _ladderObject = Instantiate(_heldLadder.gameObject, new Vector3(position.x, position.y + 1, position.x), Quaternion.identity);
                //Hold the ladder
                _heldObject = _ladderObject.gameObject.GetComponent<ThrowableObject>();
                //Ladder is not placed anymore
                _ladderPlaced = false;
            }
            else
            {
                //If the ladder isn't placed, we grab the object the player is touching
                _heldObject = _touchedObject;
            }
        }
    }

    //Handles collisions with 2D colliders
    private void OnCollisionEnter2D(Collision2D other)
    {
        foreach (var contact in other.contacts)
        {
            if (!(contact.normal.y > _groundedNormalThreshold))
                continue;
            _grounded = true;
            break;
        }

        //If the touched object is throwable
        if (other.gameObject.GetComponent<ThrowableObject>())
        {
            //The player is touching the ladder
            _playerTouchingLadder = true;

            //Set a reference to the currently touched object
            _touchedObject = other.gameObject.GetComponent<ThrowableObject>();
        }
        else
        {
            
            _playerTouchingLadder = false;
            _touchedObject = null;
        }
    }

    //Handles collisions with 2D colliders - in the case they are triggers
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<ThrowableObject>() != null)
        {
            //The player is touching the ladder
            _playerTouchingLadder = true;
            //The toucher object is the object the player is colliding with
            _touchedObject = other.gameObject.GetComponent<ThrowableObject>();
        }
        else
        {
            //The player is in a trigger, but it is not the ladder
            _playerTouchingLadder = false;
            //The player is not touching the targeted object
            _touchedObject = null;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //When we exit the trigger, we are no longer climbing
        _ladderObject.GetComponent<BoxCollider2D>().isTrigger = false; //make the ladder solid
        _verticalVelocity = 0; // set the vertical velocity to 0
        _rigidbody2D.gravityScale = 1; //bring gravity back       
    }
}
