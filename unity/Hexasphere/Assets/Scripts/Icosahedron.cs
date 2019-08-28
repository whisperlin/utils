using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Icosahedron : MonoBehaviour
{
    public delegate Point Checker(Point p);

    [SerializeField]
    private MeshFilter mFilter;
    [SerializeField]
    private MeshRenderer mRenderer;
    [SerializeField]
    private MeshCollider mCollider;

    private List<Face> mFaces;
    private Point[] mCorners;
    private HashSet<Point> mPoints;
    private List<Tile> mTiles;
    private Dictionary<Vector3, Tile> mTileLookup;

    public IEnumerable<Tile>Tiles
    {
        get
        {
            foreach(var tile in mTiles)
            {
                yield return tile;
            }
        }
    }

    public void GenerateIcosahedron(float diameter, int numDivisions)
    {
        // this is not magic but science
        var tao = 1.61803399f;

        //Icosahedron vertices
        mCorners = new Point[12]
        {
            new Point(diameter, tao * diameter, 0),
            new Point(-diameter, tao * diameter, 0),
            new Point(diameter,-tao * diameter,0),
            new Point(-diameter,-tao * diameter,0),
            new Point(0,diameter,tao * diameter),
            new Point(0,-diameter,tao * diameter),
            new Point(0,diameter,-tao * diameter),
            new Point(0,-diameter,-tao * diameter),
            new Point(tao * diameter,0,diameter),
            new Point(-tao * diameter,0,diameter),
            new Point(tao * diameter,0,-diameter),
            new Point(-tao * diameter,0,-diameter)
        };

        mPoints = new HashSet<Point>();
        foreach (var corner in mCorners)
        {
            mPoints.Add(corner);
        }

        //Icosahedron faces
        var fArr = new Face[]
        {
            new Face(mCorners[0], mCorners[1], mCorners[4]),
            new Face(mCorners[1], mCorners[9], mCorners[4]),
            new Face(mCorners[4], mCorners[9], mCorners[5]),
            new Face(mCorners[5], mCorners[9], mCorners[3]),
            new Face(mCorners[2], mCorners[3], mCorners[7]),
            new Face(mCorners[3], mCorners[2], mCorners[5]),
            new Face(mCorners[7], mCorners[10], mCorners[2]),
            new Face(mCorners[0], mCorners[8], mCorners[10]),
            new Face(mCorners[0], mCorners[4], mCorners[8]),
            new Face(mCorners[8], mCorners[2], mCorners[10]),
            new Face(mCorners[8], mCorners[4], mCorners[5]),
            new Face(mCorners[8], mCorners[5], mCorners[2]),
            new Face(mCorners[1], mCorners[0], mCorners[6]),
            new Face(mCorners[11], mCorners[1], mCorners[6]),
            new Face(mCorners[3], mCorners[9], mCorners[11]),
            new Face(mCorners[6], mCorners[10], mCorners[7]),
            new Face(mCorners[3], mCorners[11], mCorners[7]),
            new Face(mCorners[11], mCorners[6], mCorners[7]),
            new Face(mCorners[6], mCorners[0], mCorners[10]),
            new Face(mCorners[9], mCorners[1], mCorners[11])
        };

        mFaces = new List<Face>();
        mFaces.AddRange(fArr);

        Checker getPointIfExists = (point) =>
        {
            if (mPoints.Contains(point))
            {
                return point;
            }
            else
            {
                mPoints.Add(point);
                return point;
            }
        };

        //Prepare faces
        List<Face> newFaces = new List<Face>();
        for (var f = 0; f < mFaces.Count; f++)
        {
            List<Point> prev = null;
            var bottom = new List<Point>();
            bottom.Add(mFaces[f].Points[0]);
            var left = mFaces[f].Points[0].Subdivide(mFaces[f].Points[1], numDivisions, getPointIfExists);
            var right = mFaces[f].Points[0].Subdivide(mFaces[f].Points[2], numDivisions, getPointIfExists);
            for (var i = 1; i <= numDivisions; i++)
            {
                prev = bottom;
                bottom = left[i].Subdivide(right[i], i, getPointIfExists);
                for (var j = 0; j < i; j++)
                {
                    var nf = new Face(prev[j], bottom[j], bottom[j + 1], true);
                    newFaces.Add(nf);

                    if (j > 0)
                    {
                        nf = new Face(prev[j - 1], prev[j], bottom[j], true);
                        newFaces.Add(nf);
                    }
                }
            }
        }
        mFaces = newFaces;

        //Prepare points
        HashSet<Point> newPoints = new HashSet<Point>();
        foreach (var point in mPoints)
        {
            Vector3 vec = point.ToVector3();
            vec.Normalize();
            point.Set(vec * diameter);
            newPoints.Add(point);
        }
        mPoints = newPoints;
        mTiles = new List<Tile>();
        mTileLookup = new Dictionary<Vector3, Tile>();

        foreach (var p in mPoints)
        {
            var newTile = new Tile(p, 0.85f);
            var key = newTile.CenterPoint.ToVector3();
            if (!mTileLookup.ContainsKey(newTile.CenterPoint.ToVector3()))
            {
                mTileLookup[key] = newTile;
                mTiles.Add(newTile);
            }
            else
            {
                mTileLookup[key].Boundary.AddRange(newTile.Boundary); //stitching
            }
        }

        foreach (var t in this.mTiles)
        {
            var ret = t.NeighborIds.Select(f => mTileLookup[f.ToVector3()]);
            t.Neighbors = ret.ToList();
        }
    }

    private void TestRender()
    {
        // TEST VISUALIZATION----------------------------------------------
        //*
        mFaces = new List<Face>();
        var indFrom = 50;
        var indTo = 51;

        indFrom = 0;
        indTo = mTiles.Count;

        for (int i = indFrom; i < indTo; i++)
        {
            CreateTestSpheres(mTiles, i);

            new GameObject().AddComponent<Hexagon>().Init(mTiles[i]);

            var b = mTiles[i].Boundary;
            for (int j = 0; j < b.Count - 2; j++)
            {
                mFaces.Add(new Face(b[0], b[j + 1], b[j + 2]));
            }
        }

        Mesh mesh = new Mesh();

        //verts
        var verts = new Vector3[3 * mFaces.Count];
        for (int i = 0; i < mFaces.Count; i++)
        {
            verts[i * 3] = mFaces[i].Points[0].ToVector3();
            verts[i * 3 + 1] = mFaces[i].Points[1].ToVector3();
            verts[i * 3 + 2] = mFaces[i].Points[2].ToVector3();
        }
        mesh.vertices = verts;

        //indices;
        var indices = new int[3 * mFaces.Count];
        for (int i = 0; i < indices.Length; ++i)
        {
            indices[i] = i;
        }

        mesh.triangles = indices;

        mesh.RecalculateNormals();
        mFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
        // ---------------------------------------------------------------------- */
    }

    private void CreateTestSpheres(IList<Tile> tiles, int i)
    {
        // Test Spheres to check hexagons
        foreach (var p in tiles[i].Boundary)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = i.ToString();
            go.transform.position = p.ToVector3();
            go.transform.localScale = Vector3.one * 0.05f;
        }
        foreach (var n in tiles[i].Neighbors)
        {
            foreach (var po in n.Boundary)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = po.ToVector3();
                go.transform.localScale = Vector3.one * 0.05f;
            }
        }
    }
}
