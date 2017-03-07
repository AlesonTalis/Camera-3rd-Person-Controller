using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3rdC : MonoBehaviour {
    public Transform pivot;
    public Transform point;
    public Transform camPt;

    public Camera camera;

    Quaternion targetCenter;
    Quaternion targetPivot;

    void Start()
    {
        if (!this.camera)
            this.camera = Camera.main;

        this.targetCenter = this.transform.localRotation;
        this.targetPivot = this.pivot.localRotation;
    }

    void Update()
    {
        Vector2 inputLook = new Vector2(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"));

        this.targetCenter *= Quaternion.Euler(Vector3.up * inputLook.y);
        this.targetPivot *= Quaternion.Euler(Vector3.right * inputLook.x);

        this.transform.localRotation = this.targetCenter;
        this.pivot.localRotation = this.targetPivot;
    }

    void FixedUpdate()
    {
        RaycastHit hitInfo;
        bool see = Physics.Raycast(this.point.position, this.point.forward, out hitInfo, 5.0f, Physics.AllLayers);

        float dstCam = 5.0f;
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
}
