using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security.Permissions;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    
    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;

    public float rocketStrength;
    public float reelStrength;
    public float maxGrappleDistance;
    public float maxMouseGrapple;

    private Rigidbody2D rb;
    private DrawRope drawRope;
    private Vector3 prevPos;
    private Vector3 movementVector;
    private Camera mainCamera;
    private GameObject grapplePoints;

    private PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

        grapplePoints = GameObject.Find("GrapplePoints");
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _distanceJoint.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        drawRope = GetComponent<DrawRope>();
        _lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
            Movement();
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void Movement() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Transform grappled = null;
            Vector2 mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            float closestMouseGrapple = maxMouseGrapple;
            foreach (Transform grapplePoint in grapplePoints.transform)
            {
                if (Vector3.Distance(mousePos, grapplePoint.position) < closestMouseGrapple && Vector3.Distance(this.transform.position, grapplePoint.position) < maxGrappleDistance)
                {
                    closestMouseGrapple = Vector3.Distance(mousePos, grapplePoint.position);
                    grappled = grapplePoint;
                }
            }
            if (closestMouseGrapple < maxMouseGrapple)
            {
                drawRope.segmentLength = (int)(Vector3.Distance(this.transform.position, grappled.position) / (drawRope.ropeSegLen * 2));
                drawRope.reInitializeRope();
                drawRope.EndPoint = grappled;
                _distanceJoint.connectedAnchor = grappled.position;
                _distanceJoint.enabled = true;
                _lineRenderer.enabled = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            drawRope.EndPoint = this.transform;
            _distanceJoint.enabled = false;
            _lineRenderer.enabled = false;
        }
        if (_distanceJoint.enabled)
        {
            _lineRenderer.SetPosition(1, transform.position);
        }
    }

    void FixedUpdate() 
    {
        movementVector = this.transform.position - prevPos;
        prevPos = this.transform.position;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            rb.AddForce(Vector3.Normalize(movementVector) * rocketStrength);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            rb.AddForce(Vector3.Normalize(movementVector) * rocketStrength * -1);
        }
        if (Input.mouseScrollDelta.y != 0)
        {
            _distanceJoint.distance -= Input.mouseScrollDelta.y * reelStrength;
            int segmentLength = (int)(_distanceJoint.distance / drawRope.ropeSegLen);
            drawRope.resizeRope(segmentLength);
        }

    }
}