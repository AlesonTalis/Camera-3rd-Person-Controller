using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3rdC : MonoBehaviour {
    public bool rotateTarget;
    public bool moveWithTarget;

    public bool hideMouse;

    public bool invertMouse;

    public bool clampRotation;
    public Vector2 clampRotationLimit;

    public float smoothTime = 0f;

    public float mouseSensitivity;

    public float maxDistance;

    public Transform pivot;
    public Transform point;
    public Transform camPt;

    public Camera camera;

    public GameObject targetRotate;

    Quaternion targetCenter;
    Quaternion targetPivot;

    Vector2 inputLook;

    void Start()
    {
        if (!this.camera)
            this.camera = Camera.main;

        this.targetCenter = this.transform.localRotation;
        this.targetPivot = this.pivot.localRotation;

        if (this.hideMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        this.inputLook = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        this.inputLook *= this.mouseSensitivity;
        this.inputLook.x *= 0.5f;

        if (!this.invertMouse)
            this.inputLook.x *= -1f;

        this.targetCenter *= Quaternion.Euler(Vector3.up * inputLook.y);
        this.targetPivot *= Quaternion.Euler(Vector3.right * inputLook.x);

        if (this.clampRotation)
            this.targetPivot = this.ClampRotation(this.targetPivot);

        if (this.smoothTime == 0f)
        {
            this.transform.localRotation = this.targetCenter;
            this.pivot.localRotation = this.targetPivot;
        }
        else
        {
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, this.targetCenter, Time.deltaTime * this.smoothTime);
            this.pivot.localRotation = Quaternion.Slerp(this.pivot.localRotation, this.targetPivot, Time.deltaTime * this.smoothTime);
        }

        if (this.hideMouse && !Cursor.visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (this.hideMouse && Cursor.visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void FixedUpdate()
    {
        this.CameraRaycast();

        if (this.rotateTarget)
        {
            Vector3 targetR = this.targetRotate.transform.localEulerAngles;
            targetR.y = this.transform.localEulerAngles.y;

            this.targetRotate.transform.localEulerAngles = targetR;
        }

        if (this.moveWithTarget)
            this.transform.position = this.targetRotate.transform.position;
    }

    void CameraRaycast()
    {
        RaycastHit hitInfo;
        bool see = Physics.Raycast(this.point.position, this.point.forward, out hitInfo, this.maxDistance + 0.25f, Physics.AllLayers);

        float dstCam = this.maxDistance;
        float vltCam = 5.0f;

        if (see)
        {
            dstCam = hitInfo.distance - 0.25f;
            vltCam = 20f;
        }

        Vector3 pst = Vector3.forward * (-dstCam) + this.point.localPosition;

        this.camPt.localPosition = Vector3.Slerp(this.camPt.localPosition, pst, Time.fixedDeltaTime * vltCam);

        this.camera.transform.position = this.camPt.position;
        this.camera.transform.rotation = this.camPt.rotation;
    }

    Quaternion ClampRotation(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -this.clampRotationLimit.x, this.clampRotationLimit.y);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
