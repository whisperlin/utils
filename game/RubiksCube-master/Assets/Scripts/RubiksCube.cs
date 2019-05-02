using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RubiksCube : MonoBehaviour
{
    public  bool simple = false;
    public static int CCount = 9;
    public int randomStep = 5;
    public float Alpha = 0.5f;
	public Texture bump;
    public void SetSimple(bool b)
    {
        simple = b;
    }

    public void isFinish()
    {
        /*for (int i = 0, len = planes.Count; i < len; i++)
        {
            Transform t = planes[i];

            Ray ray = new Ray(t.position + t.up*0.1f, t.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("is finish = " + false);
                return;
            }

        }*/
        Debug.Log("is finish = " + isComplete());


    }
    public static bool operating = false;
    public GameObject CubeletPrefab;
    public int speed = 5;

    List<Transform> planes = new List<Transform>();
    List<GameObject> Cubelets = new List<GameObject>();
    List<List<GameObject>> movableSlices = new List<List<GameObject>>();
    List<GameObject> movingSlice = new List<GameObject>();
    Vector3 position = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    
    bool isNew = false;
    bool isRotating = false;
    bool isShuffling = false;
    bool isExploded = false;
    
    Vector3[] RotationVectors = 
    {
        Vector3.right, Vector3.up, Vector3.forward,
        Vector3.left, Vector3.down, Vector3.back
    };

    void Start()
    {
        CreateCube();
    }
    
    void Update()
    {
        Shader.SetGlobalFloat("_CubAlpha",Alpha);
		Shader.SetGlobalTexture("_CubBump",bump);
        if (Application.platform == RuntimePlatform.Android)
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        if (!isRotating && !isShuffling)
        {
            CheckInput();
            if (!isNew && isComplete() && !isExploded && !isRotating && Input.touchCount == 0)
                Explode(3.0f, 500.0f);
        }
    }
    
    public void CreateCube()
    {
        if (!isRotating && !isShuffling  )
        {
            foreach (GameObject cubelet in Cubelets)
                DestroyImmediate(cubelet);

            Cubelets.Clear();
            movableSlices.Clear();
            planes.Clear();
            movingSlice.Clear();
            int step = 1;
            CCount = 9;
            if (simple)
            {
                CCount = 4;
                step++;
            }
            for (int x = -1; x <= 1; x+= step)
                for (int y = -1; y <= 1; y += step)
                    for (int z = -1; z <= 1; z += step)
                    {
                        GameObject cubelet = Instantiate(CubeletPrefab, transform, false);
                        if (simple)
                        {
                            cubelet.transform.localScale *= 2f;
                        }
                        for (int k = 0, len00 = cubelet.transform.childCount; k < len00; k++)
                        {
                            Transform t = cubelet.transform.GetChild(k);
                            MeshRenderer mr = t.GetComponent<MeshRenderer>();
                            if(mr.sharedMaterials[1].name != "back")
                                planes.Add(t);
                        }
                        cubelet.transform.localPosition = new Vector3(-x, -y, z);
                        cubelet.GetComponent<Cubelet>().SetColor(-x, -y, z);
                        Cubelets.Add(cubelet);
                    }
            isNew = true;
            isExploded = false;
        }
    }

    
    bool isComplete()
    {
        
        return isSideComplete(Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1),Vector3.left) &&
            isSideComplete(Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 1),Vector3.right) &&
            isSideComplete(Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1),Vector3.down) &&
            isSideComplete(Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 1),Vector3.up) &&
            isSideComplete(Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -1),Vector3.back) &&
            isSideComplete(Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1),Vector3.forward);
    }

    bool isSideComplete(List<GameObject> slice,Vector3 dir)
    {
        int count = slice.Count;
        //Debug.Log(dir.ToString());
        Material mat = null;
        for (int i = 0; i < count; i++)
        {
            
            for (int j = 0; j < 6; j++)
            {
                GameObject g = slice[i].GetComponent<Cubelet>().Planes[j];
                var u = g.transform.up;
                float d = Vector3.Dot(dir, u);
                if (d > 0.999f)
                {
                    //Debug.Log(g.transform.parent.name+"."+g.name+" "+d );
                    //Debug.Log(u.ToString());
                    var m0 = g.GetComponent<MeshRenderer>().sharedMaterials[1];
                    
                    if (mat && m0 != mat)
                    {
                        //Debug.Log(m0.name + " "+mat.name );
                        return false;
                    }
                    mat = m0;
                }
            }
            
             
        }
        return true;
    }

    static Dictionary<int, Material> mats = new Dictionary<int, Material>();
    static Material GetMatExplode(Material m)
    {
        Material n=null;
        mats.TryGetValue(m.GetInstanceID(),out n);

        if (null == n)
        {
            n = new Material(Shader.Find("Explode"));
            n.CopyPropertiesFromMaterial(m);
            mats[m.GetInstanceID()] = n;
        }
        return n;
    }
    void Explode(float radius, float power)
    {
        /*
        for (int i = 0, len = planes.Count; i < len; i++)
        {
            Transform t = planes[i];
            MeshRenderer mr = t.GetComponent<MeshRenderer>();
            var mat = mr.sharedMaterials[1];
            var newMat = GetMatExplode(mat);
            mr.sharedMaterials[1] = newMat;

        }
        */
            isExploded = true;
    }

    void FixCube()
    {
        foreach (GameObject cubelet in Cubelets)
        {
            cubelet.transform.localPosition = new Vector3(
                    Mathf.Round(cubelet.transform.localPosition.x),
                    Mathf.Round(cubelet.transform.localPosition.y),
                    Mathf.Round(cubelet.transform.localPosition.z));
            cubelet.transform.localRotation = Quaternion.Euler(
                    Round((int)cubelet.transform.localRotation.eulerAngles.x),
                    Round((int)cubelet.transform.localRotation.eulerAngles.y),
                    Round((int)cubelet.transform.localRotation.eulerAngles.z));
        }
    }

    int Round(int angle)
    {
        if (angle % 90 > 45)
            return angle + (90 - angle % 90);
        else
            return angle - angle % 90;
    }

    void CheckInput()
    {

        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 origin = ray.origin;


            movableSlices.Clear();
            movingSlice.Clear();
            rotation = Vector3.zero;
            position = Vector3.zero;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                operating = true;
                if (hit.transform.name == "Cubelet(Clone)")
                    return;
                Transform selectedSide = hit.transform;
                Transform selectedCubelet = selectedSide.parent;
                position = selectedCubelet.localRotation * selectedSide.localPosition + selectedCubelet.localPosition;
                if (Mathf.Abs(position.x) >= 1.5)
                {
                    movableSlices.Add(GetYSlice(selectedCubelet));
                    movableSlices.Add(GetZSlice(selectedCubelet));
                }
                if (Mathf.Abs(position.y) >= 1.5)
                {
                    if (origin.x > Mathf.Abs(origin.z) || origin.x < -Mathf.Abs(origin.z))
                    {
                        movableSlices.Add(GetXSlice(selectedCubelet));
                        movableSlices.Add(GetZSlice(selectedCubelet));
                    }
                    else if (origin.z > Mathf.Abs(origin.x) || origin.z < -Mathf.Abs(origin.x))
                    {
                        movableSlices.Add(GetZSlice(selectedCubelet));
                        movableSlices.Add(GetXSlice(selectedCubelet));
                    }
                }
                if (Mathf.Abs(position.z) >= 1.5)
                {
                    movableSlices.Add(GetYSlice(selectedCubelet));
                    movableSlices.Add(GetXSlice(selectedCubelet));
                }
            }
        }
        // MouseMoveDirection = new Vector2( Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        else if (Input.GetMouseButtonUp(0))
        {
            operating = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 origin = ray.origin;
            if (movingSlice.Count != 0)
            {
                int angle = 0;
                Vector3 rotationVector = Vector3.zero;

                if ((int)movingSlice[0].transform.localRotation.eulerAngles.x % 90 != 0)
                    angle = (int)movingSlice[0].transform.localRotation.eulerAngles.x % 90;
                else if ((int)movingSlice[0].transform.localRotation.eulerAngles.y % 90 != 0)
                    angle = (int)movingSlice[0].transform.localRotation.eulerAngles.y % 90;
                else if ((int)movingSlice[0].transform.localRotation.eulerAngles.z % 90 != 0)
                    angle = (int)movingSlice[0].transform.localRotation.eulerAngles.z % 90;

                if (rotation.x != 0)
                    rotationVector = new Vector3(angle > 45 ? 1 : -1, 0, 0);
                else if (rotation.y != 0)
                    rotationVector = new Vector3(0, angle > 45 ? 1 : -1, 0);
                else if (rotation.z != 0)
                    rotationVector = new Vector3(0, 0, angle > 45 ? 1 : -1);
                StartCoroutine(CompleteRotation(movingSlice, rotationVector, speed));
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 MouseMoveDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))*10;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 origin = ray.origin;
            if (movableSlices.Count != 0)
            {
                Vector3 rotationVector = Vector3.zero;
                Vector3 initialVector = rotationVector;
                if (Mathf.Abs(MouseMoveDirection.x) > Mathf.Abs(MouseMoveDirection.y) && MouseMoveDirection.x != 0)
                {
                    if (movingSlice.Count == 0)
                        movingSlice = movableSlices[0];
                    if (movingSlice.All(movableSlices[0].Contains))
                    {
                        if (Mathf.Abs(position.x) >= 1.5)
                            rotationVector = new Vector3(0, -MouseMoveDirection.x * speed, 0);
                        if (position.y >= 1.5)
                        {
                            if (origin.z > Mathf.Abs(origin.x))
                                rotationVector = new Vector3(0, 0, MouseMoveDirection.x * speed);
                            else if (origin.z < -Mathf.Abs(origin.x))
                                rotationVector = new Vector3(0, 0, -MouseMoveDirection.x * speed);
                            else if (origin.x > Mathf.Abs(origin.z))
                                rotationVector = new Vector3(MouseMoveDirection.x * speed, 0, 0);
                            else if (origin.x < -Mathf.Abs(origin.z))
                                rotationVector = new Vector3(-MouseMoveDirection.x * speed, 0, 0);
                        }
                        else if (position.y <= -1.5)
                        {
                            if (origin.z > Mathf.Abs(origin.x))
                                rotationVector = new Vector3(0, 0, -MouseMoveDirection.x * speed);
                            else if (origin.z < -Mathf.Abs(origin.x))
                                rotationVector = new Vector3(0, 0, MouseMoveDirection.x * speed);
                            else if (origin.x > Mathf.Abs(origin.z))
                                rotationVector = new Vector3(-MouseMoveDirection.x * speed, 0, 0);
                            else if (origin.x < -Mathf.Abs(origin.z))
                                rotationVector = new Vector3(MouseMoveDirection.x * speed, 0, 0);
                        }
                        if (Mathf.Abs(position.z) >= 1.5)
                            rotationVector = new Vector3(0, -MouseMoveDirection.x * speed, 0);
                    }
                }
                else if (Mathf.Abs(MouseMoveDirection.x) < Mathf.Abs(MouseMoveDirection.y) && MouseMoveDirection.y != 0)
                {
                    if (movingSlice.Count == 0)
                        movingSlice = movableSlices[1];
                    if (movingSlice.All(movableSlices[1].Contains))
                    {
                        if (position.x >= 1.5)
                            rotationVector = new Vector3(0, 0, MouseMoveDirection.y * speed);
                        else if (position.x <= 1.5)
                            rotationVector = new Vector3(0, 0, -MouseMoveDirection.y * speed);
                        if (Mathf.Abs(position.y) >= 1.5)
                        {
                            if (origin.z > Mathf.Abs(origin.x))
                                rotationVector = new Vector3(-MouseMoveDirection.y * speed, 0, 0);
                            else if (origin.z < -Mathf.Abs(origin.x))
                                rotationVector = new Vector3(MouseMoveDirection.y * speed, 0, 0);
                            else if (origin.x > Mathf.Abs(origin.z))
                                rotationVector = new Vector3(0, 0, MouseMoveDirection.y * speed);
                            else if (origin.x < -Mathf.Abs(origin.z))
                                rotationVector = new Vector3(0, 0, -MouseMoveDirection.y * speed);
                        }
                        if (position.z >= 1.5)
                            rotationVector = new Vector3(-MouseMoveDirection.y * speed, 0, 0);
                        else if (position.z <= -1.5)
                            rotationVector = new Vector3(MouseMoveDirection.y * speed, 0, 0);
                    }
                }
                if (!rotationVector.Equals(initialVector))
                    rotation = rotationVector;
                foreach (GameObject cubelet in movingSlice)
                    cubelet.transform.RotateAround(Vector3.zero, rotationVector, speed);
            }
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            Vector3 origin = ray.origin;
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    {
                        movableSlices.Clear();
                        movingSlice.Clear();
                        rotation = Vector3.zero;
                        position = Vector3.zero;
                        RaycastHit hit;
                        if(Physics.Raycast(ray, out hit))
                        {
                            operating = true; 
                            if (hit.transform.name == "Cubelet(Clone)")
                                return;
                            Transform selectedSide = hit.transform;
                            Transform selectedCubelet = selectedSide.parent;
                            position = selectedCubelet.localRotation * selectedSide.localPosition + selectedCubelet.localPosition;
                            if (Mathf.Abs(position.x) >= 1.5)
                            {
                                movableSlices.Add(GetYSlice(selectedCubelet));
                                movableSlices.Add(GetZSlice(selectedCubelet));
                            }
                            if (Mathf.Abs(position.y) >= 1.5)
                            {
                                if (origin.x > Mathf.Abs(origin.z) || origin.x < -Mathf.Abs(origin.z))
                                {
                                    movableSlices.Add(GetXSlice(selectedCubelet));
                                    movableSlices.Add(GetZSlice(selectedCubelet));
                                }
                                else if (origin.z > Mathf.Abs(origin.x) || origin.z < -Mathf.Abs(origin.x))
                                {
                                    movableSlices.Add(GetZSlice(selectedCubelet));
                                    movableSlices.Add(GetXSlice(selectedCubelet));
                                }
                            }
                            if (Mathf.Abs(position.z) >= 1.5)
                            {
                                movableSlices.Add(GetYSlice(selectedCubelet));
                                movableSlices.Add(GetXSlice(selectedCubelet));
                            }
                        }
                        break;
                    }
                case TouchPhase.Moved:
                    {
                        if (movableSlices.Count != 0)
                        {
                            Vector3 rotationVector = Vector3.zero;
                            Vector3 initialVector = rotationVector;
                            if (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y) && touch.deltaPosition.x != 0)
                            {
                                if (movingSlice.Count == 0)
                                    movingSlice = movableSlices[0];
                                if (movingSlice.All(movableSlices[0].Contains))
                                {
                                    if (Mathf.Abs(position.x) >= 1.5)
                                        rotationVector = new Vector3(0, -touch.deltaPosition.x * speed, 0);
                                    if (position.y >= 1.5)
                                    {
                                        if (origin.z > Mathf.Abs(origin.x))
                                            rotationVector = new Vector3(0, 0, touch.deltaPosition.x * speed);
                                        else if (origin.z < -Mathf.Abs(origin.x))
                                            rotationVector = new Vector3(0, 0, -touch.deltaPosition.x * speed);
                                        else if (origin.x > Mathf.Abs(origin.z))
                                            rotationVector = new Vector3(touch.deltaPosition.x * speed, 0, 0);
                                        else if (origin.x < -Mathf.Abs(origin.z))
                                            rotationVector = new Vector3(-touch.deltaPosition.x * speed, 0, 0);
                                    }
                                    else if (position.y <= -1.5)
                                    {
                                        if (origin.z > Mathf.Abs(origin.x))
                                            rotationVector = new Vector3(0, 0, -touch.deltaPosition.x * speed);
                                        else if (origin.z < -Mathf.Abs(origin.x))
                                            rotationVector = new Vector3(0, 0, touch.deltaPosition.x * speed);
                                        else if (origin.x > Mathf.Abs(origin.z))
                                            rotationVector = new Vector3(-touch.deltaPosition.x * speed, 0, 0);
                                        else if (origin.x < -Mathf.Abs(origin.z))
                                            rotationVector = new Vector3(touch.deltaPosition.x * speed, 0, 0);
                                    }
                                    if (Mathf.Abs(position.z) >= 1.5)
                                        rotationVector = new Vector3(0, -touch.deltaPosition.x * speed, 0);
                                }
                            }
                            else if (Mathf.Abs(touch.deltaPosition.x) < Mathf.Abs(touch.deltaPosition.y) && touch.deltaPosition.y != 0)
                            {
                                if (movingSlice.Count == 0)
                                    movingSlice = movableSlices[1];
                                if (movingSlice.All(movableSlices[1].Contains))
                                {
                                    if (position.x >= 1.5)
                                        rotationVector = new Vector3(0, 0, touch.deltaPosition.y * speed);
                                    else if (position.x <= 1.5)
                                        rotationVector = new Vector3(0, 0, -touch.deltaPosition.y * speed);
                                    if (Mathf.Abs(position.y) >= 1.5)
                                    {
                                        if (origin.z > Mathf.Abs(origin.x))
                                            rotationVector = new Vector3(-touch.deltaPosition.y * speed, 0, 0);
                                        else if (origin.z < -Mathf.Abs(origin.x))
                                            rotationVector = new Vector3(touch.deltaPosition.y * speed, 0, 0);
                                        else if (origin.x > Mathf.Abs(origin.z))
                                            rotationVector = new Vector3(0, 0, touch.deltaPosition.y * speed);
                                        else if (origin.x < -Mathf.Abs(origin.z))
                                            rotationVector = new Vector3(0, 0, -touch.deltaPosition.y * speed);
                                    }
                                    if (position.z >= 1.5)
                                        rotationVector = new Vector3(-touch.deltaPosition.y * speed, 0, 0);
                                    else if (position.z <= -1.5)
                                        rotationVector = new Vector3(touch.deltaPosition.y * speed, 0, 0);
                                }
                            }
                            if (!rotationVector.Equals(initialVector))
                                rotation = rotationVector;
                            foreach (GameObject cubelet in movingSlice)
                                cubelet.transform.RotateAround(Vector3.zero, rotationVector, speed);
                        }
                        break;
                    }
                case TouchPhase.Ended:
                    {
                        if (movingSlice.Count != 0)
                        {
                            operating = false;
                            int angle = 0;
                            Vector3 rotationVector = Vector3.zero;

                            if ((int)movingSlice[0].transform.localRotation.eulerAngles.x % 90 != 0)
                                angle = (int)movingSlice[0].transform.localRotation.eulerAngles.x % 90;
                            else if ((int)movingSlice[0].transform.localRotation.eulerAngles.y % 90 != 0)
                                angle = (int)movingSlice[0].transform.localRotation.eulerAngles.y % 90;
                            else if ((int)movingSlice[0].transform.localRotation.eulerAngles.z % 90 != 0)
                                angle = (int)movingSlice[0].transform.localRotation.eulerAngles.z % 90;

                            if (rotation.x != 0)
                                rotationVector = new Vector3(angle > 45 ? 1 : -1, 0, 0);
                            else if (rotation.y != 0)
                                rotationVector = new Vector3(0, angle > 45 ? 1 : -1, 0);
                            else if (rotation.z != 0)
                                rotationVector = new Vector3(0, 0, angle > 45 ? 1 : -1);
                            StartCoroutine(CompleteRotation(movingSlice, rotationVector, speed));
                        }
                        break;
                    }
                default: break;
            }
        }
    }

    List<GameObject> GetXSlice(Transform transform)
    {
        return Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.x) == transform.localPosition.x);
    }

    List<GameObject> GetYSlice(Transform transform)
    {
        return Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.y) == transform.localPosition.y);
    }

    List<GameObject> GetZSlice(Transform transform)
    {
        return Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.z) == transform.localPosition.z);
    }

    IEnumerator Rotate(List<GameObject> cubelets, Vector3 rotationVector, int angle, int speed)
    {
        isRotating = true;
        while (angle > 0)
        {
            foreach (GameObject cubelet in cubelets)
                cubelet.transform.RotateAround(Vector3.zero, rotationVector, (angle % speed == 0) ? speed : 1);
            angle -= (angle % speed == 0) ? speed : 1;
            yield return null;
        }
        FixCube();
        isRotating = false;
    }

    IEnumerator CompleteRotation(List<GameObject> cubelets, Vector3 rotationVector, int speed)
    {
        isRotating = true;
        if ((int)cubelets[0].transform.localRotation.eulerAngles.x % 90 != 0)
        {
            while ((int)cubelets[0].transform.localRotation.eulerAngles.x % 90 != 0)
            {
                foreach (GameObject cubelet in cubelets)
                    cubelet.transform.RotateAround(Vector3.zero, rotationVector, ((int)cubelets[0].transform.localRotation.eulerAngles.x % speed == 0) ? speed : 1);
                yield return null;
            }
        }
        else if ((int)cubelets[0].transform.localRotation.eulerAngles.y % 90 != 0)
        {
            while ((int)cubelets[0].transform.localRotation.eulerAngles.y % 90 != 0)
            {
                foreach (GameObject cubelet in cubelets)
                    cubelet.transform.RotateAround(Vector3.zero, rotationVector, ((int)cubelets[0].transform.localRotation.eulerAngles.y % speed == 0) ? speed : 1);
                yield return null;
            }
        }
        else if ((int)cubelets[0].transform.localRotation.eulerAngles.z % 90 != 0)
        {
            while ((int)cubelets[0].transform.localRotation.eulerAngles.z % 90 != 0)
            {
                foreach (GameObject cubelet in cubelets)
                    cubelet.transform.RotateAround(Vector3.zero, rotationVector, ((int)cubelets[0].transform.localRotation.eulerAngles.z % speed == 0) ? speed : 1);
                yield return null;
            }
        }
        FixCube();
        isRotating = false;
    }

    public void Shuffle()
    {
        if (!isExploded && !isRotating && !isShuffling && movableSlices.Count == 0)
        {
            StartCoroutine(ShuffleRoutine());
            isNew = false;
        }
    }

    IEnumerator ShuffleRoutine()
    {
        isShuffling = true;
        int axis, slice, direction;
        List<GameObject> moveCubelets = new List<GameObject>();
        Vector3 rotationVector;
        for (int moveCount = randomStep; moveCount >= 0; moveCount--)
        {
            axis = Random.Range(0, 3);
            slice = Random.Range(-1, 2);
            if (axis == 0)
                moveCubelets = Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.x) == slice);
            else if (axis == 1)
                moveCubelets = Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.y) == slice);
            else
                moveCubelets = Cubelets.FindAll(x => Mathf.Round(x.transform.localPosition.z) == slice);

            direction = Random.Range(0,2);
            rotationVector = RotationVectors[axis + 3 * direction];
            StartCoroutine(Rotate(moveCubelets, rotationVector, 90, 10));
            yield return new WaitForSeconds(.25f);
        }
        isShuffling = false;
    }
}
