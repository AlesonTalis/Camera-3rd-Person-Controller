using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GravityConfig
{
	public float gravityMultiplier = 5.0f;
	public float jumpSpeed = 5.0f;
	public bool airControll = false;

	public GravityConfig() {}
}

public class PlayerC : MonoBehaviour 
{
	public CameraC cameraC;
	public GravityConfig gravity;

	CharacterController character;
	Camera camera;

	Vector2 inputLook;
	Vector2 inputMove;

	bool inputJump;

	Vector3 moveDir;

	bool jumping, beforeGrounded;

	void Start () {
		this.character = this.GetComponent<CharacterController>();
		this.camera = Camera.main;
		this.cameraC.Init(this.transform, this.camera.transform, new Vector3(0f, 1.75f, 0f));
	}

	void Update () {
		this.GetInput1();
		this.Jump1();
		this.cameraC.Rotate(this.transform, this.camera.transform, this.inputLook);
	}

	void FixedUpdate()
	{
		this.GetInput2();
		this.Move();
		this.Gravity();

		this.character.Move(this.moveDir * Time.fixedDeltaTime);
	}



	void Move()
	{
		RaycastHit hitInfo;
		Vector3 desiredMove = this.transform.forward * this.inputMove.y + 
			this.transform.right * this.inputMove.x;
		Physics.SphereCast(this.transform.position, this.character.radius + this.character.skinWidth,
			Vector3.down, out hitInfo, (this.character.height / 2) + this.character.skinWidth, Physics.AllLayers, QueryTriggerInteraction.Ignore);
		desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

		if ((this.gravity.airControll && !this.character.isGrounded) || this.character.isGrounded)
		{
			this.moveDir.x = desiredMove.x * 2.0f;
			//this.moveDir.y = desiredMove.y;
			this.moveDir.z = desiredMove.z * 2.0f;
		}
	}

	void Gravity()
	{
		if (this.character.isGrounded)
		{
			this.moveDir.y = -10f;

			if (this.inputJump)
			{
				this.moveDir.y = this.gravity.jumpSpeed;

				this.jumping = true;
				this.inputJump = false;
			}
		}
		else
		{
			this.moveDir += Physics.gravity * this.gravity.gravityMultiplier * Time.fixedDeltaTime;
		}

		this.inputJump = false;
	}

	void Jump1()
	{
		if (!this.beforeGrounded && this.character.isGrounded && this.jumping)
		{
			this.jumping = false;
			this.moveDir.y = 0f;
		}

		if (!this.character.isGrounded && !this.jumping)
		{
			this.jumping = true;
		}

		this.beforeGrounded = this.character.isGrounded;
	}



	void GetInput1()
	{
		this.inputLook = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

		if (!this.inputJump)
			this.inputJump = Input.GetKeyDown(KeyCode.Space);
	}

	void GetInput2()
	{
		this.inputMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}
}
