using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 1.0f;
    [SerializeField]
    private float _jumpVelocity = 5.0f;

    private CharacterController _characterController;
    private Vector2 _velocity;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
