using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SwimmingController : MonoBehaviour
{
    [SerializeField] private float swimmingForce;
    [SerializeField] private float resistanceForce;
    [SerializeField] private float deadZone;
    [SerializeField] private float interval;
    [SerializeField] Transform trackingSpace;

    private float currentWaitTime;
    private new Rigidbody rigidbody;
    private Vector3 currentDirection;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool rightButtonPressed = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        bool leftButtonPressed = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        currentWaitTime += Time.deltaTime;
        if(rightButtonPressed && leftButtonPressed)
        {
            Vector3 leftHandDirection = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
            Vector3 RightHandDirection = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Vector3 localVelocity = leftHandDirection + RightHandDirection;
            localVelocity *= 1f;
            if (localVelocity.sqrMagnitude > deadZone * deadZone && currentWaitTime > interval)
            {
                AddSwimmingForce(localVelocity);
                currentWaitTime = 0;
            }

            if (rigidbody.velocity.sqrMagnitude > 0.01f && currentDirection != Vector3.zero)
            {
                rigidbody.AddForce(-rigidbody.velocity * resistanceForce, ForceMode.Acceleration);
            }
            else
            {
                currentDirection = Vector3.zero;
            }
        }
    }

    private void AddSwimmingForce(Vector3 localVelocity)
    {
        Vector3 worldSpaceVelocity = trackingSpace.TransformDirection(localVelocity);
        rigidbody.AddForce(worldSpaceVelocity * swimmingForce, ForceMode.Impulse);
        currentDirection = worldSpaceVelocity.normalized;
    }

}
