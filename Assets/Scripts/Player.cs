using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviour
{
    
    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;

    public float rocketStrength;
    public float reelStrength;
    public float maxGrappleDistance;
    public float maxMouseGrapple;
    public float gasUsageSpeed;
    public float gasRegenSpeed;
    public float brakeUsageSpeed;
    public float brakeRegenSpeed;
    public float groundSpeed;
    public float dashSpeed;
    public float dashDistance;


    private Rigidbody2D rb;
    public GameObject dashParticles;
    private DrawRope drawRope;
    private Vector3 prevPos;
    private Vector3 movementVector;
    private Camera mainCamera;
    private GameObject grapplePoints;
    private float boostGas = 1f;
    private float brakeGas = 1f;
    private GameObject BoostEncapsulator;
    private GameObject BoostBar;
    private GameObject BrakeEncapsulator;
    private GameObject BrakeBar;
    private bool boostOnCooldown = false;
    private bool brakeOnCooldown = false;
    public bool isDashing = false;
    private Vector3 mPos;

    public float dashStrength;
    public float dashCooldown;
    private bool dashOnCooldown;
    private float dashCounter = 0f;
    private Vector2 dashPoint;

    private bool e = false;
    private bool space = false;
    private bool mouse1 = false;
    private bool w = false;

    private Text scoreText;
    private int score = 0;

    private TrailRenderer playerTrail;

    private PhotonView PV;

    private float maxSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        transform.SetParent(GameObject.Find("Players").transform);
        grapplePoints = GameObject.Find("GrapplePoints");
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _distanceJoint.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        drawRope = GetComponent<DrawRope>();
        _lineRenderer.enabled = true;
        BoostEncapsulator = GameObject.Find("BoostBar");
        BoostBar = BoostEncapsulator.transform.GetChild(1).gameObject;
        BrakeEncapsulator = GameObject.Find("BrakeBar");
        BrakeBar = BrakeEncapsulator.transform.GetChild(1).gameObject;
        playerTrail = transform.GetChild(0).GetComponent<TrailRenderer>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + score;
       // if (PV.IsMine)
            Movement();
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void Movement() 
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = new Vector3(0, 0, 0);
            rb.velocity = new Vector3(0, 0, 0);
        }
        Transform grappled = null;
        Vector2 mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float closestMouseGrapple = maxMouseGrapple;
        foreach (Transform grapplePoint in grapplePoints.transform)
        {
            grapplePoint.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 1);
            if (Vector3.Distance(mousePos, grapplePoint.position) < closestMouseGrapple && Vector3.Distance(this.transform.position, grapplePoint.position) < maxGrappleDistance)
            {
                closestMouseGrapple = Vector3.Distance(mousePos, grapplePoint.position);
                grappled = grapplePoint;
            }
        }
        if (closestMouseGrapple < maxMouseGrapple)
        {
            grappled.GetComponent<SpriteRenderer>().color = new Color(255, 255, 0, 1);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _lineRenderer.enabled = true;
                drawRope.enabled = true;
                drawRope.segmentLength = (int)(Vector3.Distance(this.transform.position, grappled.position) / (drawRope.ropeSegLen * 2));
                drawRope.reInitializeRope();
                drawRope.EndPoint = grappled;
                _distanceJoint.connectedAnchor = grappled.position;
                _distanceJoint.enabled = true;
                _lineRenderer.enabled = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            drawRope.EndPoint = this.transform;
            _distanceJoint.enabled = false;
            //_lineRenderer.enabled = false;
        }
        if (_distanceJoint.enabled)
        {
            _lineRenderer.SetPosition(1, transform.position);
        }
        ManageInputs();
        

    }

    void FixedUpdate() 
    {
        playerTrail.time = rb.velocity.magnitude * (0.5f / 40);   //max range value / max speed value * 2 (give or take) adjusted to make trail look better according to player speed.
        movementVector = this.transform.position - prevPos;
        prevPos = this.transform.position;
        ManageMeters();
        if (space && boostGas > 0 && !boostOnCooldown)
        {
            rb.AddForce(Vector3.Normalize(movementVector) * rocketStrength);
            boostGas -= gasUsageSpeed;
        }
        if (mouse1 && brakeGas > 0 && !brakeOnCooldown)
        {
            rb.AddForce(Vector3.Normalize(movementVector) * rocketStrength * -1);
            brakeGas -= brakeUsageSpeed;
        }
        if (Input.mouseScrollDelta.y != 0 && false)
        {
            _distanceJoint.distance -= Input.mouseScrollDelta.y * reelStrength;
            int segmentLength = (int)(_distanceJoint.distance / drawRope.ropeSegLen);
            drawRope.resizeRope(segmentLength);
        }
        if (e && !dashOnCooldown && !_distanceJoint.enabled)
        {
            //Instantiate(dashParticles, transform.position, Quaternion.identity).transform.SetParent(transform);
            e = false;
            float magnitude = rb.velocity.magnitude;
            mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.velocity = Vector2.zero;
            dashPoint = transform.position + ((mPos - transform.position).normalized) * dashDistance;
            rb.velocity = (mPos - transform.position).normalized * magnitude;
            rb.AddForce(rb.velocity.normalized * dashStrength);
            dashOnCooldown = true;
            isDashing = true;
        }
        if (dashOnCooldown)
        {
            e = false;
            dashCounter += Time.deltaTime;
            if (dashCounter >= dashCooldown)
            {
                dashCounter = 0;
                dashOnCooldown = false;
            }
        }
        if (!_distanceJoint.enabled)
        {
            _lineRenderer.enabled = false;
            drawRope.enabled = false;
            if (!isDashing)
            {
            
                if (w)
                {
                    w = false;
                    rb.AddForce(new Vector2(0f, 10f), ForceMode2D.Impulse);
                }
            }
            if (isDashing)
            {
                transform.position = Vector2.MoveTowards(transform.position, dashPoint, dashSpeed);
                if (Vector2.Distance(transform.position, dashPoint) < 1f)
                {
                    isDashing = false;
                }

            }
        }
        BoostBar.transform.localScale = new Vector3(boostGas, BoostBar.transform.localScale.y, 1);
        BrakeBar.transform.localScale = new Vector3(brakeGas, BrakeBar.transform.localScale.y, 1);
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, 0);

        rb.AddForce(movement * groundSpeed);
    }

    private void ManageMeters()
    {
        boostGas += gasRegenSpeed;
        brakeGas += brakeRegenSpeed;
        if (boostGas >= 1)
        {
            boostGas = 1;
            boostOnCooldown = false;
        }
        if (boostGas <= 0)
        {
            boostGas = 0;
            boostOnCooldown = true;
        }
        if (brakeGas >= 1)
        {
            brakeGas = 1;
            brakeOnCooldown = false;
        }
        if (brakeGas <= 0)
        {
            brakeGas = 0;
            brakeOnCooldown = true;
        }

    }

    private void ManageInputs()
    {
        if (Input.GetKeyDown(KeyCode.E) && !_distanceJoint.enabled)
        {
            Debug.Log("E was pressed");
            e = true;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            w = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            space = true;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            mouse1 = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            space = false;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            mouse1 = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "coin")
        {
            score += 1;
            Destroy(collision.gameObject);
        }
        if (collision.tag == "movingCoin")
        {
            score += 3;
            Destroy(collision.gameObject);
        }
    }
}