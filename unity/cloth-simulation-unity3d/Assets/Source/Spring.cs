using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spring script
/// </summary>
public class Spring
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Spring(Node nodeA, Node nodeB, float stiffness, float beta)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        length0 = Vector3.Distance(nodeA.pos, nodeB.pos);
        this.stiffness = stiffness;
        this.beta = beta;
    }


    #region InEditorVariables

    public Node nodeA;
    public Node nodeB;

    public float stiffness;
    public float beta;


    #endregion

    #region OtherVariables

    public float length0;


    #endregion

    #region OtherFunctions

    /// <summary>
    /// Computes the spring's force and applies it to its nodes
    /// </summary>
    public void computeForce()
    {
        Vector3 force = - stiffness * (getLength() - length0) * getUnitVector();
        //Fuerza de amortiguamiento
        force -= (beta * stiffness) * Vector3.Dot(getUnitVector(), (nodeA.vel - nodeB.vel)) * getUnitVector();
        nodeA.force += force;
        nodeB.force -= force;
    }
    
    /// <summary>
    /// Returns current spring direction as a Unit Vector
    /// </summary>
    /// <returns></returns>
    public Vector3 getUnitVector()
    {
        return (nodeA.pos - nodeB.pos) / getLength();
    }

    /// <summary>
    /// Returns current spring length
    /// </summary>
    /// <returns></returns>
    public float getLength()
    {
        return Vector3.Distance(nodeA.pos, nodeB.pos);
    }

    #endregion



}
