using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Photon.Pun;
using System;
using System.Runtime.CompilerServices;

public class DrawRope : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;

    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    public float ropeSegLen = 0.3f;
    public int segmentLength = 35;
    private float lineWidth = 0.035f;
    private float[] ropeFloatArray;
    private bool hasPrinted = false;

    private PhotonView PV;

    // Use this for initialization
    void Start()
    {
        PV = GetComponent<PhotonView>();
        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = StartPoint.position;

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
        RopeSegmentsToFloatArray(this.ropeSegments);
        
    }

    // Update is called once per frame
    void Update()
    {
        RopeSegmentsToFloatArray(this.ropeSegments);
        this.Draw(this.lineWidth, this.segmentLength, this.ropeSegments);
        //DrawLines();
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    public void reInitializeRope() 
    {
        Vector3 ropeStartPoint = StartPoint.position;

        ropeSegments = new List<RopeSegment>();
        for (int i = 0; i < segmentLength; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    public void resizeRope(int segmentLength) 
    {
        Vector3 ropeStartPoint = StartPoint.position;

        if (segmentLength < this.segmentLength && segmentLength > 1)
        {
            ropeSegments.RemoveRange(1, this.segmentLength - segmentLength);
            this.segmentLength = segmentLength;
        }
        while (segmentLength > this.segmentLength)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            this.segmentLength++;
        }
       
    }

    private void Simulate()
    {
        // SIMULATION
        Vector2 forceGravity = new Vector2(0f, -1f);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        //Constrant to First Point 
        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = this.StartPoint.position;
        this.ropeSegments[0] = firstSegment;


        //Constrant to Second Point 
        RopeSegment endSegment = this.ropeSegments[this.ropeSegments.Count - 1];
        endSegment.posNow = this.EndPoint.position;
        this.ropeSegments[this.ropeSegments.Count - 1] = endSegment;

        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void RopeSegmentsToFloatArray(List<RopeSegment> ropeSegments)
    {
        this.ropeFloatArray = new float[this.segmentLength * 2];
        for (int i = 0; i < this.segmentLength * 2; i += 2)
        {
            ropeFloatArray[i] = ropeSegments[i / 2].posNow.x;
            ropeFloatArray[i + 1] = ropeSegments[i / 2].posNow.y;
            //print(ropeArray[i]);
            //print(ropeArray[i + 1]);
        }
    }

    private List<RopeSegment> FloatArrayToRopeSemgents(float[] floatArray) 
    {
        List<RopeSegment> ropeSegments = new List<RopeSegment>();
        for (int i = 0; i < floatArray.Length; i += 2) 
        {
            ropeSegments.Add(new RopeSegment(new Vector3(floatArray[i], floatArray[i + 1])));
                Debug.Log($"x: {floatArray[i]}, y: {floatArray[i + 1]}");
                Debug.Log($"x: {this.ropeSegments[i / 2].posNow.x}, y: {this.ropeSegments[i / 2].posNow.y}");
                Debug.Log("\n\n\n\n");
        }
        return ropeSegments;
    }


    [PunRPC]
    private void Draw(float lineWidth, int segmentLength, float[] ropeSegmentsFloat)
    {
        GameObject player = GameObject.Find("Players").transform.GetChild(1).gameObject;
        LineRenderer lineRenderer = player.GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        /*List<RopeSegment> ropeSegments = FloatArrayToRopeSemgents(ropeSegmentsFloat);
        Vector3[] ropePositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }*/
        Vector3[] ropePositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength * 2; i += 2)
        {
            ropePositions[i / 2].x = ropeSegmentsFloat[i];
            ropePositions[i / 2].y= ropeSegmentsFloat[i + 1];
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    private void Draw(float lineWidth, int segmentLength, List<RopeSegment> ropeSegments)
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength; i ++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public void DrawLines()
    {
        // Call the other party's function (in this case, RpcDrawLines function)
        PV.RPC("Draw", RpcTarget.All, this.lineWidth, this.segmentLength, this.ropeFloatArray);
    }

    public class RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;


        public RopeSegment(float x1, float y1, float x2, float y2) 
        {
            this.posNow.x = x1;
            this.posNow.y = y1;
            this.posOld.x = x2;
            this.posOld.y = y2;
        }

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }

        public static RopeSegment Deserialize(float[] data)
        {
            RopeSegment result = new RopeSegment(data[0], data[1], data[2], data[3]);
            return result;
        }
        public static float[] Serialize(RopeSegment ropeSegment)
        {
            float[] data = { ropeSegment.posNow.x, ropeSegment.posNow.y, ropeSegment.posOld.x, ropeSegment.posOld.y };
            return data;
        }

    }
}
