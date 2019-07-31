using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cloth physics manager that makes use of the mass-spring model
/// to simulate the behaviour of a cloth on a mesh
/// </summary>
public class MassSpringCloth : MonoBehaviour 
{
	/// <summary>
	/// Default constructor.
	/// </summary>
	public MassSpringCloth()
	{
		this.Paused = true;
		this.TimeStep = 0.001f;
		this.Gravity = new Vector3 (0.0f, -9.81f, 0.0f);
		this.IntegrationMethod = Integration.Symplectic;
	}

	/// <summary>
	/// Integration method.
	/// </summary>
	public enum Integration
	{
		Explicit = 0,
		Symplectic = 1,
	};

	#region InEditorVariables

    //Simulation variables
	public bool Paused;
	public float TimeStep;
    public Vector3 Gravity;
	public Integration IntegrationMethod;

    //Mesh component variables
    Mesh mesh;
    Vector3[] vertices;

    //Lists
    List<Spring> springs = new List<Spring>();
    List<Node> nodes = new List<Node>();
    List<Triangle> clothTriangles = new List<Triangle>();

    //Physics constants and properties
    public float nodeMass = 0.5f;
    public float stiffnessFlex = 50.0f;
    public float stiffnessTrac = 500.0f;

    public float dampAlpha = 0.1f;
    public float dampBeta = 0.1f;

    public float friction = 1;
    public Vector3 windVel = Vector3.zero;

    #endregion

    #region OtherVariables

    #endregion


    #region Getters and Setters

    public List<Node> getNodes()
    {
        return nodes;
    }

    #endregion

    #region MonoBehaviour

    //Se utiliza Awake() para que todas las mallas queden inicializadas antes de ser fijadas por el Fixer
    public void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        //Se añaden los nodos a partir de las posiciones de los vertices del mesh
        foreach(Vector3 v in vertices)
        {
            nodes.Add(new Node(transform.TransformPoint(v), Gravity, nodeMass, dampAlpha, friction));
        }

        EdgeEqualityComparer edgeComparer = new EdgeEqualityComparer();

        var edgeDictionary = new Dictionary<Edge, Edge>(edgeComparer);

        for (int i = 0; i < triangles.Length;i += 3)
        {
            //Se intentan añadir 3 aristas de un triangulo al diccionario
            //Arista 1
            addEdge(new Edge(triangles[i], triangles[i + 1], triangles[i + 2]), edgeDictionary);
            //Arista 2
            addEdge(new Edge(triangles[i + 1], triangles[i + 2], triangles[i]), edgeDictionary);
            //Arista 3
            addEdge(new Edge(triangles[i], triangles[i + 2], triangles[i + 1]), edgeDictionary);

            //Añade triangulo a la lista
            clothTriangles.Add(new Triangle(nodes[triangles[i]], nodes[triangles[i + 1]], nodes[triangles[i + 2]]));
        } 
        //Las aristas del diccionario se añaden como muelles a la lista
        foreach(KeyValuePair<Edge, Edge> e in edgeDictionary)
        {
            springs.Add(new Spring(nodes[e.Key.vertexA], nodes[e.Key.vertexB], stiffnessTrac, dampBeta));
        }
    }

	public void Update()
	{
		if (Input.GetKeyUp (KeyCode.P))
			this.Paused = !this.Paused;

        for(int i = 0; i < nodes.Count; i++)
        {
            vertices[i] = transform.InverseTransformPoint(nodes[i].pos);
        }
        mesh.vertices = vertices;
	}

    public void FixedUpdate()
    {
        for(int i = 0; i < 1/(TimeStep*100); i++)
        {
            if (this.Paused)
                return; // Not simulating

            // Select integration method
            switch (this.IntegrationMethod)
            {
                case Integration.Explicit: this.stepExplicit(); break;
                case Integration.Symplectic: this.stepSymplectic(); break;
                default:
                    throw new System.Exception("[ERROR] Should never happen!");
            }
        }

    }

    #endregion

    /// <summary>
    /// Tries to add an edge to the dictionary, if that edge is duplicate, a flexion spring formed by the opposing vertices
    /// is added to the spring list instead
    /// </summary>
    private void addEdge(Edge edge, Dictionary<Edge, Edge> dictionary)
    {
        Edge otherEdge;
        if(dictionary.TryGetValue(edge, out otherEdge))
        {
            springs.Add(new Spring(nodes[edge.vertexC], nodes[otherEdge.vertexC], stiffnessFlex, dampBeta));

        }
        else
        {
            dictionary.Add(edge, edge);
        }
    }
    /// <summary>
    /// Performs a simulation step in 1D using Explicit integration.
    /// </summary>
    private void stepExplicit()
	{
        //Unstable, requires lower timestep
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].force = Vector3.zero;
            nodes[i].computeForce();
        }
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].computeForce();
        }
        for (int i = 0; i < clothTriangles.Count; i++)
        {
            clothTriangles[i].computeWindForce(windVel);
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isFixed)
            {
                nodes[i].pos += TimeStep * nodes[i].vel;
                nodes[i].vel += (TimeStep / nodes[i].mass) * nodes[i].force;
            }
        }
    }

	/// <summary>
	/// Performs a simulation step in 1D using Symplectic integration.
	/// </summary>
	private void stepSymplectic()
	{
        //Se calcula la fuerza de cada nodo
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].force = Vector3.zero;
            nodes[i].computeForce();
        }
        //Se calcula la fuerza de cada muelle
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].computeForce();
        }
        //Se calcula la fuerza de viento aplicada a cada triangulo
        for (int i = 0; i < clothTriangles.Count; i++)
        {
            clothTriangles[i].computeWindForce(windVel);
        }
        //Se aplican las fuerzas para calcular la velocidad y la posicion de cada nodo (A no ser que estén fijados)
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isFixed)
            {
                nodes[i].vel += (TimeStep / nodes[i].mass) * nodes[i].force;
                nodes[i].pos += TimeStep * nodes[i].vel;
            }
        }

    }
}
