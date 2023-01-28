using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    private Camera camera;
    
    // Fields for dragging
    private bool isDragging;
    [SerializeField] private bool isDraggingEnabled;
    private Vector3 dragCamInitialPos;
    private Vector3 dragMouseInitialPos;

    // Fields for scrolling
    [SerializeField] private bool isScrollingEnabled;
    [SerializeField] private float scrollFactor;
    [SerializeField] private float minScrollSize; // Should be bigger than 0
    [SerializeField] private float maxScrollSize; // Should be bigger than minScrollSize
    private Vector3 pivot;

    void Start()
    {
        camera = Camera.main;
        isDragging = false;
    }

    void Update()
    {
        dragging();
        scrolling();
    }

    #region Dragging

    private void dragging()
    {
        // Do nothing when dragging is not enabled
        if(!isDraggingEnabled)
        {
            return;
        }
        
        // Change KeyCode.Mouse2 to whatever Key you want
        // KeyCode.Mouse2 is the middle mouse button
        bool dragButton = Input.GetKey(KeyCode.Mouse2);

        if(!isDragging)
        {
            if(dragButton)
            {
                // Initialize dragging
                
                dragCamInitialPos = transform.position;
                dragMouseInitialPos = camera.ScreenToViewportPoint(Input.mousePosition);
                isDragging = true;
                isScrollingEnabled = false;
            }
        }
        else
        {
            if(dragButton)
            {
                // Is dragging
                
                Vector3 mouse = camera.ScreenToViewportPoint(Input.mousePosition);
                Vector3 delta = mouse - dragMouseInitialPos;
                float orthSize = camera.orthographicSize;
                
                delta.x *= camera.aspect * orthSize / 0.5f;
                delta.y *= orthSize / 0.5f;
                
                transform.position = dragCamInitialPos - delta;
            }
            else
            {
                // Stopped dragging
                
                isDragging = false;
                isScrollingEnabled = true;
            }
        }
    }
    
    #endregion

    #region Scrolling

    private void scrolling()
    {
        // Do nothing when scrolling is not enabled
        if(!isScrollingEnabled)
        {
            return;
        }
        
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        float cameraSize = camera.orthographicSize;
        
        if(scrollWheel > 0f && cameraSize > minScrollSize) // Zoom In
        {
            doScroll(cameraSize, scrollFactor * -1);
        }
        else if(scrollWheel < 0f && cameraSize < maxScrollSize) // Zoom Out
        {
            doScroll(cameraSize, scrollFactor);
        }
    }

    private void doScroll(float cameraSize, float scrollFactor)
    {
        getPivot();
        // Check if camera goes out of bounds
        if(cameraSize + scrollFactor < minScrollSize)
        {
            camera.orthographicSize = minScrollSize;
        }
        else if(cameraSize + scrollFactor > maxScrollSize)
        {
            camera.orthographicSize = maxScrollSize;
        }
        else
        {
            camera.orthographicSize += scrollFactor;
        }
        moveCamera();
    }

    // Move the camera so that it zooms in and out relative to the cursor
    private void moveCamera()
    {
        Vector3 mouse = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 delta = mouse - pivot;
        delta.z = 0;
        transform.position -= delta;
    }

    // Save the current mouse position in world space, necessary for moveCamera()
    private void getPivot()
    {
        pivot = camera.ScreenToWorldPoint(Input.mousePosition);
    }
    
    #endregion
}
