using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClampRotation
{
	public Vector2 clampX;

	public ClampRotation()
	{
		this.clampX = Vector2.zero;
	}

	public Quaternion Clamp(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		if (this.clampX != Vector2.zero)
		{
			q.x = this.Clamp_(q.x, this.clampX);
		}

		return q;
	}

	private float Clamp_(float angle, Vector2 limits)
	{
		angle = 2.0f * Mathf.Rad2Deg * Mathf.Atan(angle);
		angle = Mathf.Clamp(angle, -limits.x, limits.y);
		return Mathf.Tan(Mathf.Deg2Rad * 0.5f * angle);
	}
}

[System.Serializable]
public class Smooth
{
	public float smoothTime;

	public Smooth()
	{
		this.smoothTime = 0f;
	}

	public Quaternion SmoothRotate(Quaternion a, Quaternion b)
	{
		if (this.smoothTime != 0)
		{
			a = Quaternion.Slerp(a, b, this.smoothTime * Time.deltaTime);
		}
		return a;
	}
}

[System.Serializable]
public class CameraC
{
	public bool invertMouse = false;
    public bool useCamera1RD;
	[Range(0.5f, 5.0f)] public float mouseSensitivity = 2.0f;
	public ClampRotation clamp;
	public Smooth smooth;

	Quaternion targetCharacter;
	Quaternion targetCamera;

	Vector2 inputLook;

	Vector3 cameraPosition;

	public void Init(Transform character, Transform camera)
	{
		this.targetCharacter = character.localRotation;
		this.targetCamera = camera.localRotation;

		this.cameraPosition = Vector3.one;
	}

	public void Init(Transform character, Transform camera, Vector3 cameraPosition)
	{
		this.targetCharacter = character.localRotation;
		this.targetCamera = camera.localRotation;

		this.cameraPosition = cameraPosition;

		this.SetCameraPosition(character, camera);
	}

	public void Rotate(Transform character, Transform camera, Vector2 input)
	{
		this.inputLook = input * this.mouseSensitivity;

		if (this.invertMouse)
		{
			this.inputLook.x *= -1f;
		}
        
        if (this.useCamera1RD)
            this.Camera1RDRotate(character, camera);
	}

    private void Camera1RDRotate(Transform character, Transform camera)
    {
        this.targetCamera *= Quaternion.Euler(this.inputLook.x, 0f, 0f);
        this.targetCharacter *= Quaternion.Euler(0f, this.inputLook.y, 0f);

        this.targetCamera = this.clamp.Clamp(this.targetCamera);

        character.localRotation = this.smooth.SmoothRotate(character.localRotation, this.targetCharacter);
        camera.localRotation = this.smooth.SmoothRotate(camera.localRotation, this.targetCamera);

        this.SetCameraPosition(character, camera);
    }

	void SetCameraPosition(Transform character, Transform camera)
	{
        if (this.cameraPosition != Vector3.one)
        {
            camera.position = character.position - Vector3.up + this.cameraPosition;
            camera.eulerAngles = new Vector3(camera.eulerAngles.x, character.eulerAngles.y, character.eulerAngles.z);
        }
	}
}
