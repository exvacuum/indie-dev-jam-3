using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private readonly int Moving = Animator.StringToHash("Moving");
    private int GroundLayer;
    private int ClimbableLayer;
    
    private Rigidbody2D _rigidbody2D;
    
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
    [Header("Audio")]
    [SerializeField]
    private AudioManager _audioManager;
    [SerializeField]
    private AudioClip _jumpAudioClip;
    [SerializeField]
    private AudioClip _throwAudioClip;
    [SerializeField]
    private AudioClip _pickUpAudioClip;

    private Vector2 _velocity;
    private bool _grounded;
    private bool _canClimb;
    private bool _ladderPlaced;
    private GameObject _touchedObject;
    private GameObject _ladderObject;
    private Animator _animator;
    private Transform _ladder;

    private bool _isClimbing;
    private float _walkValue;
    private bool _wantsToPlace;
    private bool _wantsToThrow;
    private bool _wantsToPickUp;
    private bool _wantsToJump;
    private float _climbValue;

    private List<Collider2D> _overlappingColliders;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _ladder = GetComponentInChildren<Transform>();

        GroundLayer = LayerMask.NameToLayer("Ground");
        ClimbableLayer = LayerMask.NameToLayer("Climbable");
    }

    private void Update()
    {
        //If the player is holding an object, hold it above the player
        if (_heldObject)
        {
            var position = transform.position;
            _heldObject.throwable.isKinematic = true;
            _heldObject.transform.parent = transform;
            _heldObject.transform.position = new Vector3(position.x, position.y + 1.03f, position.x);
            _heldObject.transform.localRotation = Quaternion.identity;
        }
        
        HandleWalk();
        HandleClimb();
        if(!HandlePlace())
            if (!HandleThrow())
                HandlePickUp();
        HandleGravity();
        HandleJump();
        ApplyMovement();
        
        UpdateVisuals();
        
        _wantsToPlace = false;
        _wantsToThrow = false;
        _wantsToPickUp = false;
        _wantsToJump = false;
    }

    private void HandleWalk()
    {
        _velocity.x = _isClimbing && !_grounded ? 0 : _walkValue * _movementSpeed;
    }

    private bool HandlePlace()
    {
        if (!_wantsToPlace) return false;
        //If the player is holding an object and is pressing the down arrow, place it
        if (_heldObject)
        {
            Destroy(_heldObject.gameObject); //Delete the held ladder

            _heldObject = null; //The player is no longer holding an object

            //Create a new placeable ladder
            var position = transform.position;
            _ladderObject = Instantiate(_placeableLadder.gameObject, new Vector2(position.x, position.y + (float)0.5), Quaternion.identity);

            //The ladder is placed
            _ladderPlaced = true;
            return true;
        }

        return false;
    }

    private bool HandleThrow()
    {
        if (!_wantsToThrow) return false;
        //If the throw key is pressed, the player has an object, and the down key is not pressed, throw the object
        if (_heldObject)
        {
            _heldObject.throwable.isKinematic = false;
            _heldObject.transform.parent = transform.parent;
            _heldObject.Throw(Mathf.Sign(_velocity.x));
            _heldObject = null;
            _audioManager.PlaySoundAt(_throwAudioClip, transform.position, true);
            return true;
        }

        return false;
    }

    private bool HandlePickUp()
    {
        if (!_wantsToPickUp) return false;
        //If the player is touching the ladder, and is not holding an object
        if (_touchedObject && !_heldObject)
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
                _heldObject = _touchedObject.GetComponent<ThrowableObject>();
            }

            _audioManager.PlaySoundAt(_pickUpAudioClip, transform.position, true);
            return true;
        }
        return false;
    }

    private void HandleClimb()
    {
        if(_climbValue == 0) return;
        if(_climbValue < 0 && _grounded) return;
        
        if (_climbValue != 0.0f && _ladderPlaced && _canClimb)
        {
            var vector2 = _rigidbody2D.velocity;
            vector2.y = 0;
            _rigidbody2D.velocity = vector2;
            //Set vertical velocity (1 or -1 * movespeed)
            var position = transform.position;
            position.y += _climbValue * _movementSpeed * Time.deltaTime;
            transform.position = position;

            //Go up/down - also sets player to center of ladder
            //transform.position = new Vector2(_ladderObject.gameObject.transform.position.x, transform.position.y);
            _isClimbing = true;
        }
    }

    private void HandleJump()
    {
        if (_wantsToJump && (_grounded || _isClimbing))
        {
            _rigidbody2D.AddForce(Vector2.up * _jumpImpulse, ForceMode2D.Impulse);
            _audioManager.PlaySoundAt(_jumpAudioClip, transform.position, true);
        }
    }

    private void HandleGravity()
    {
        _rigidbody2D.gravityScale = _isClimbing ? 0 : 1;
    }

    private void ApplyMovement()
    {
        var transform1 = transform;
        var position = transform1.position;
        position += (Vector3)_velocity * Time.deltaTime;
        transform1.position = position;
    }

    private void UpdateVisuals()
    {
        //Set value in animator
        _animator.SetBool(Moving, Mathf.Abs(_velocity.x * Time.deltaTime) > 0f);
        
        if (_velocity.x != 0)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Sign(_velocity.x);
            transform.localScale = scale;
        }
    }

    #region Input Handling

    private void OnWalk(InputValue value)
    {
        _walkValue = value.Get<float>();
    }

    private void OnClimb(InputValue value)
    {
        _climbValue = value.Get<float>();
    }

    private void OnJump(InputValue value)
    {
        _wantsToJump = value.isPressed;
    }

    private void OnThrow(InputValue value)
    {
        _wantsToThrow = value.isPressed;
    }

    private void OnPlace(InputValue value)
    {
        _wantsToPlace = value.isPressed;
    }

    private void OnPickUp(InputValue value)
    {
        _wantsToPickUp = value.isPressed;
    }
    
    #endregion

    #region Unity Collider Events

    //Handles collisions with 2D colliders
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.layer == GroundLayer || other.gameObject.layer == ClimbableLayer)
        {
            foreach (var contact in other.contacts)
            {
                if (!(contact.normal.y > _groundedNormalThreshold))
                    continue;
                _grounded = true;
                break;
            }
        }
        
        if (other.gameObject.layer == ClimbableLayer)
        {
            var below = false;
            foreach (var contact in other.contacts)
            {
                if (!(contact.normal.y > _groundedNormalThreshold))
                    continue;
                below = true;
                break;
            }
            if(!below)
                other.gameObject.GetComponent<Collider2D>().isTrigger = true;
        }
        
        //If the touched object is throwable
        if (other.gameObject.GetComponent<ThrowableObject>())
        {
            _touchedObject = other.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == GroundLayer)
        {
            _grounded = false;
        }
        
        if (other.gameObject == _touchedObject)
        {
            _touchedObject = null;
        }
    }

    //Handles collisions with 2D colliders - in the case they are triggers
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<ThrowableObject>())
        {
            _touchedObject = other.gameObject;
        }

        if (other.gameObject.layer == ClimbableLayer)
        {
            _canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == _touchedObject)
        {
            _touchedObject = null;
        }
        
        if (other.gameObject.layer == ClimbableLayer)
        {
            other.gameObject.GetComponent<Collider2D>().isTrigger = false;
            _isClimbing = false;
            _canClimb = false;
        }
    }
    
    #endregion
}
