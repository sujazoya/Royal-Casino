using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Object : MonoBehaviour
{
    //Set this to the transform you want to check
    private Transform objectTransfom;

    private float noMovementThreshold = 0.0001f;
    private const int noMovementFrames = 3;
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    private bool isMoving;

    BallController ballController;
    [SerializeField] Transform Base;   
    float currentPose;
    [SerializeField] float fallingTime;
    [SerializeField] Transform ballMesh;
    [HideInInspector]public float totalDistance;
    AudioSource audioSource;
    Rigidbody rb;

    // Desired hovering height.
    float hoverHeight = 4.0f;
    // The force applied per unit of distance below the desired height.
    float hoverForce = 5.0f;

    // The amount that the lifting force is reduced per unit of upward speed.
    // This damping tends to stop the object from bouncing after passing over
    // something.
    float hoverDamp = 0.5f;

    //Let other scripts see if the object is moving
    public bool IsMoving
    {
        get { return isMoving; }
    }



    void Awake()
    {
        objectTransfom = this.transform;
        //For good measure, set the previous locations
        for (int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        ballController = FindObjectOfType<BallController>();
        
    }

    void Update()
    {
       
        currentPose = Vector3.Distance(Base.position, transform.position);       
        //Store the newest vector at the end of the list of vectors
        for (int i = 0; i < previousLocations.Length - 1; i++)
        {
            previousLocations[i] = previousLocations[i + 1];
        }
        previousLocations[previousLocations.Length - 1] = objectTransfom.position;

        //Check the distances between the points in your previous locations
        //If for the past several updates, there are no movements smaller than the threshold,
        //you can most likely assume that the object is not moving
        for (int i = 0; i < previousLocations.Length - 1; i++)
        {
            
            if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
            {
                //The minimum movement has been detected between frames
                isMoving = true;
                break;
            }
            else
            {
                isMoving = false;
            }           
        }
        if (!isMoving)
        {
            ballController.ballStatus = BallController.Ball.Stoped;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        else 
        {
            if (currentPose < oldCountValue)
            {
                ballController.ballStatus = BallController.Ball.Falling;
                fallingTime += Time.deltaTime;
               
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
            else
            {
                ballController.ballStatus = BallController.Ball.OnTheWay;               
                fallingTime = 0;
                ballMesh.Rotate(11, 0, 0);
                totalDistance = Vector3.Distance(Base.position, transform.position);
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            //{

            //}
            //ballController.ballStatus = BallController.Ball.OnTheWay;
        }
        if ( fallingTime > 2f && !ballController.clicking)
        {
            ballController.OnGameover();
        }
        oldCountValue=Vector3.Distance(Base.position, transform.position);


        #region HOVER
        //RaycastHit hit;
        //Ray downRay = new Ray(transform.position, -Vector3.up);
        //if (Physics.Raycast(downRay, out hit))
        //{
        //    // The "error" in height is the difference between the desired height
        //    // and the height measured by the raycast distance.
        //    float hoverError = hoverHeight - hit.distance;

        //    // Only apply a lifting force if the object is too low (ie, let
        //    // gravity pull it downward if it is too high).
        //    if (hoverError > 0)
        //    {
        //        // Subtract the damping from the lifting force and apply it to
        //        // the rigidbody.
        //        float upwardSpeed = rb.velocity.y;
        //        float lift = hoverError * hoverForce - upwardSpeed * hoverDamp;
        //        rb.AddForce(lift * Vector3.up);
        //    }
        //}
        #endregion

    }
    float oldCountValue;
    private void OnDestroy()
    {
        ballController.ballStatus = BallController.Ball.Destroied;
    }
}
