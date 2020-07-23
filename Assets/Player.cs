using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Permissions;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;
    public float rocketStrength;

    private Rigidbody2D rb;
    private Vector3 prevPos;
    private Vector3 movementVector;
    
    // Start is called before the first frame update
    void Start()
    {
        _distanceJoint.enabled = false;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _lineRenderer.SetPosition(0, mousePos);
            _lineRenderer.SetPosition(1, transform.position);
            _distanceJoint.connectedAnchor = mousePos;
            _distanceJoint.enabled = true;
            _lineRenderer.enabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
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
    }
}