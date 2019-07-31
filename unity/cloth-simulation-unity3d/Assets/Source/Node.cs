using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///Node should have a PhysicsManager as a parent for proper use

/// <summary>
/// Node script
/// </summary>
public class Node
{
    /// <summary>
    /// Default constructor. 
    /// </summary>
    public Node(Vector3 pos, Vector3 gravity, float mass, float alpha, float friction)
    {
        this.pos = pos;
        this.vel = Vector3.zero;
        this.mass = mass;
        this.gravity = gravity;
        this.alpha = alpha;
        this.friction = friction;
    }


    #region InEditorVariables

    public float mass;
    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public bool isFixed;
    public Vector3 gravity;
    public float alpha;
    public float friction;

    #endregion
    #region OtherVariables
    #endregion
 

    #region OtherFunctions

    /// <summary>
    /// Computes the node's force
    /// </summary>
    public void computeForce()
    {
        force += mass * gravity;
        //Fuerza de amortiguamiento
        force -= alpha * mass * vel;
    }

    #endregion

}
