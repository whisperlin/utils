using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    #region InEditorVariables

    #endregion

    #region OtherVariables
    public int vertexA;
    public int vertexB;
    public int vertexC;
    #endregion

    public Edge(int vertexA, int vertexB, int vertexC)
    {
        if(vertexA < vertexB)
        {
            this.vertexA = vertexA;
            this.vertexB = vertexB;
        } else
        {
            this.vertexA = vertexB;
            this.vertexB = vertexA;
        }
        this.vertexC = vertexC;
    }
}
