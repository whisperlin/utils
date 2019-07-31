using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle {

    public Triangle(Node nodeA, Node nodeB, Node nodeC)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        this.nodeC = nodeC;
    }

    #region Variables

    Node nodeA;
    Node nodeB;
    Node nodeC;

    #endregion
    #region Functions

    /// <summary>
    /// Returns normal vector
    /// </summary>
    /// <returns></returns>
    public Vector3 getNormal()
    {
        return (Vector3.Cross((nodeB.pos - nodeA.pos), (nodeC.pos - nodeA.pos))).normalized;
    }

    /// <summary>
    /// Returns area of the surface
    /// </summary>
    /// <returns></returns>
    public float getArea()
    {
        return Vector3.Cross((nodeB.pos - nodeA.pos), (nodeC.pos - nodeA.pos)).magnitude * 0.5f;
    }

    /// <summary>
    /// Returns the average velocity of its nodes
    /// </summary>
    /// <returns></returns>
    public Vector3 getVel()
    {
        return (nodeA.vel + nodeB.vel + nodeC.vel) / 3.0f;
    }
    /// <summary>
    /// Computes the triangle's wind force and applies it to its nodes
    /// </summary>
    /// <param name="friction"></param>
    /// <param name="windVel"></param>
    public void computeWindForce(Vector3 windVel)
    {
        //Fuerza de viento
        Vector3 force = nodeA.friction * getArea() * Vector3.Dot(getNormal(), (windVel - getVel())) * getNormal();

        nodeA.force += force/3;
        nodeB.force += force/3;
        nodeC.force += force/3;
    }
    #endregion
}
