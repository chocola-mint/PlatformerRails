﻿using UnityEngine;
using PlatformerRails;

[RequireComponent(typeof(MoverOnRails))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float Accelaration = 20f;
    [SerializeField]
    float Drag = 5f;
    [SerializeField]
    float JumpSpeed = 5f;
    [SerializeField]
    float Gravity = 15f;

    [SerializeField, Space]
    float GroundDistance = 0.5f;
    [SerializeField]
    float GroundCheckLength = 0.05f;

    MoverOnRails Controller;
    void Awake()
    {
        Controller = GetComponent<MoverOnRails>();
    }

	private void OnEnable()
	{
        Controller.OnLocalPositionUpdated += CorrectGroundDistance;
	}

	private void OnDisable()
	{
        Controller.OnLocalPositionUpdated -= CorrectGroundDistance;
    }
    void Update() 
    {
        //GetButtonDown is polled per-frame. That's why we have to move the jump check here.
        var distance = CheckGroundDistance();
        if (distance != null && Input.GetButtonDown("Jump"))
            Controller.Velocity.y = JumpSpeed;

    }
	void FixedUpdate()
    {
        //To make X value 0 means locate the character just above the rail
        Controller.Velocity.x = -Controller.Position.x * 5f;
        //Changing Z value in local position means moving toward rail direction
        Controller.Velocity.z += Input.GetAxisRaw("Horizontal") * Accelaration * Time.fixedDeltaTime;
        Controller.Velocity.z -= Controller.Velocity.z * Drag * Time.fixedDeltaTime;
        //Y+ axis = Upwoard (depends on rail rotation)
        var distance = CheckGroundDistance();
        if (distance != null)
        {
            //Controller.Velocity.y = (GroundDistance - distance.Value) / Time.fixedDeltaTime; //ths results for smooth move on slopes
            // Controller.Velocity.y = 0f;
        }
        else
            Controller.Velocity.y -= Gravity * Time.fixedDeltaTime;
    }

    void CorrectGroundDistance()
	{
        var distance = CheckGroundDistance();
        if (distance != null)
            Controller.Position += Vector3.up * (GroundDistance - distance.Value);
    }

    float? CheckGroundDistance()
    {
        RaycastHit info;
        var hit = Physics.Raycast(transform.position, -transform.up, out info, GroundDistance + GroundCheckLength);
        if (hit)
            return info.distance;
        else
            return null;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, Vector3.down * (GroundDistance + GroundCheckLength));
        Gizmos.DrawWireCube(Vector3.down * GroundDistance, Vector3.right / 2 + Vector3.forward / 2);
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}
