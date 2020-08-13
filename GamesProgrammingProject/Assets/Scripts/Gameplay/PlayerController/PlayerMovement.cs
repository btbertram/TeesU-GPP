using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Transform _playerModelTransform;
    Transform _playerInteractCollider;
    CharacterController _charController;
    Vector3 _playerVelocity;
    Vector3 _prevPos;
    Camera Camera;
    CameraMovement CamMove;
    public GameObject MovementObject;
    public float jumpStrength = 1;
    public float playerSpeed = 15;
    public float playerRotationSpeed = 5;
    float gravityValue = -9.81f;

    // Start is called before the first frame update
    void Start()
    {
        _playerModelTransform = GameObject.Find("PlayerModel").transform;
        _playerInteractCollider = GameObject.Find("PlayerInteractionCollider").transform;
        _charController = GetComponentInParent<CharacterController>();
        _playerVelocity = Vector3.zero;
        Camera = GameObject.FindObjectOfType<Camera>();
        CamMove = Camera.GetComponentInParent<CameraMovement>();
        _prevPos = this.transform.position;
        StartCoroutine(DistanceTraveledCheck());
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
        Movement();

    }

    void Movement()
    {
        if (Input.GetButtonDown(EInput.Jump.ToString()) && _charController.isGrounded)
        {
            _playerVelocity.y += jumpStrength * 2 * -gravityValue;
        }
            
        if ( Input.GetButton(EInput.Vertical.ToString()) || Input.GetButton(EInput.Horizontal.ToString()))
        {
            //Normalized to avoid root2 movement speed
            var normalizedMove = Vector3.Normalize(MovementObject.transform.forward * Input.GetAxis(EInput.Vertical.ToString()) + 
                MovementObject.transform.right * Input.GetAxis(EInput.Horizontal.ToString()));

            Vector3 move;

            if (Input.GetButton(EInput.Sprint.ToString()))
            {
                move = normalizedMove * (playerSpeed * 2) * Time.deltaTime;
            }
            else
            {
                move = normalizedMove * playerSpeed * Time.deltaTime;
            }
            
            _charController.Move(move);
            _playerModelTransform.rotation = CamMove.PlayerDirectionObject.transform.rotation;
            _playerInteractCollider.rotation = CamMove.PlayerDirectionObject.transform.rotation;
        }
       
        _charController.Move(_playerVelocity * Time.deltaTime);
        
    }

    IEnumerator DistanceTraveledCheck()
    {
        for(; ; )
        {
            CheckDistanceTraveled();
            yield return new WaitForSeconds(1);
        }
    }

    void CheckDistanceTraveled()
    {
        float distance = Vector3.Distance(_prevPos, this.transform.position);
        GameObject.FindObjectOfType<PlayerStats>().SendMessage(EMessagedFunc.UpdateDistanceTotalAsync.ToString(), distance);
        _prevPos = this.transform.position;
    }

    void Gravity()
    {
        //If on the ground "cancel" gravity so it doesn't stack up
        if (_charController.isGrounded && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0;
        }
        _playerVelocity.y += gravityValue * Time.deltaTime;


    }

}
