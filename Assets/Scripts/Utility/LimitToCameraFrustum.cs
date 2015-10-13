﻿using UnityEngine;
using System.Collections;


/// <summary>
/// This script is made espacially for the BasePlayer class.
/// It limits the player to the edge of the camera and pushes the player back if he is on the edge.
/// </summary>
public class LimitToCameraFrustum 
{
    // Determines if the gameobject is in the camera view when the script starts.
    protected bool isInsideCameraOnStart;

    // Reference to the main camera.
    protected Camera cam;

    // Reference to the base player.
    BasePlayer player;

    // The offset of the edgedetection.
    protected float detectionOffset = 0.02f;

    public LimitToCameraFrustum(BasePlayer player, float detectionOffset)
    {
        this.player = player;
        this.detectionOffset = detectionOffset;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    /// <summary>
    /// Checks if the gameobject is inside the camera view.
    /// </summary>
    /// <returns>True when the object is inside the camera view.</returns>
    public void CheckCameraBounding(Vector3 checkPosition)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(checkPosition);

        // Left
        if (viewPos.x < 0f - detectionOffset)
            player.ManipulateMovement(player.MovementSpeed * 2, Vector3.right);
        
        // Bottom
        if (viewPos.y < 0f - detectionOffset)
            player.ManipulateMovement(player.MovementSpeed * 2, Vector3.forward);

        // Right
        if (viewPos.x > 1f + detectionOffset)
            player.ManipulateMovement(player.MovementSpeed * 2, Vector3.left);

        // Top
        if (viewPos.y > 1f + detectionOffset)
            player.ManipulateMovement(player.MovementSpeed * 2, Vector3.back);
    }
}