using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject CameraRotationObject;
    public GameObject CameraFollowObject;
    public GameObject PlayerDirectionObject;
    public float CameraRotationSpeed = 50;
    public float CameraFollowSpeed = 20;
    public float CameraDistance = 20;
    public bool smoothCamOn = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Movement();
    }

    void Movement()
    {
        if (Input.GetButton(EInput.Fire2.ToString()))
        {       
            CameraFollowObject.transform.RotateAround(CameraRotationObject.transform.position, Vector3.up, Input.GetAxis(EInput.MouseX.ToString()) * CameraRotationSpeed * Time.deltaTime);

            CameraRotationObject.transform.Rotate(Vector3.up, Input.GetAxis(EInput.MouseX.ToString()) * CameraRotationSpeed * Time.deltaTime);
            PlayerDirectionObject.transform.Rotate(Vector3.up, Input.GetAxis(EInput.MouseX.ToString()) * CameraRotationSpeed * Time.deltaTime);
            CameraFollowObject.transform.RotateAround(CameraRotationObject.transform.position, -CameraRotationObject.transform.right, Input.GetAxis(EInput.MouseY.ToString()) * CameraRotationSpeed * Time.deltaTime);            
        }

        if (smoothCamOn)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, CameraFollowObject.transform.position, CameraFollowSpeed * Time.deltaTime);
        }
        else
        {
            this.transform.position = CameraFollowObject.transform.position;
        }

        this.transform.LookAt(CameraRotationObject.transform);

    }
}

