using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour
{
    public static Controller instance;

    public static float h1 = 4.5f;
    public static float h2 = 3.4f;
    public static float h3 = 3.9f;

    public string vertex_path = "Assets/Resources/vertex.txt";
    public string index_path = "Assets/Resources/index.txt";
    public string uv_path = "Assets/Resources/uvs.txt";

    [SerializeField] GameObject mesh_objects;

    List<Vector3[]> vertices = new List<Vector3[]>();    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            
        }

    }

    private void Start()
    {
        List<Mesh> meshes = new List<Mesh>();

        vertices = ReadVertexData(vertex_path);
        List<Vector2[]> uvs = ReadUVData(uv_path);
        List<int[]> triangles = ReadIndexData(index_path);


        for (int i = 0; i < 4; i++)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices[i];
            mesh.uv = uvs[i];
            mesh.triangles = triangles[i];
            mesh.colors = new Color[] { Color.white, Color.white, Color.white };
            mesh.colors32 = new Color32[] { new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255) };
            mesh.RecalculateNormals();
            mesh_objects.transform.Find("Mesh" + (i + 1).ToString()).GetComponent<MeshFilter>().mesh = mesh;
        }


    }


    public float H1()
    {
        return h1;
    }
    public float H2()
    {
        return h2;
    }
    public float H3()
    {
        return h3;
    }


    public List<Vector3[]> Vertices()
    {
        return vertices;
    }


    List<Vector3[]> ReadVertexData(string path)
    {
        StreamReader reader = new StreamReader(path);
        List<Vector3[]> result = new List<Vector3[]>();


        for (int trekant = 0; trekant < 4; trekant++)
        {
            Vector3[] vector = new Vector3[3];
            for (int i = 0; i < 3; i++)
            {              
               vector[i] = StringToVector3(reader.ReadLine());        
            }
            result.Add(vector);

        }
        return result;
    }
    public static Vector3 StringToVector3(string vector_string)
    {

        if (vector_string.StartsWith("(") && vector_string.EndsWith(")"))
        {
            vector_string = vector_string.Substring(1, vector_string.Length - 2);
        }
        string[] sArray = vector_string.Split(',');

        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    List<int[]> ReadIndexData(string path)
    {
        StreamReader reader = new StreamReader(path);
        List<int[]> result = new List<int[]>();

        for (int trekant = 0; trekant < 4; trekant++)
        {
            int[] vector = StringToInt(reader.ReadLine());
            result.Add(vector);
        }
        return result;
    }
    List<Vector2[]> ReadUVData(string path)
    {
        StreamReader reader = new StreamReader(path);
        
        List<Vector2[]> result = new List<Vector2[]>();

        for (int trekant = 0; trekant < 4; trekant++)
        {
            Vector2[] vector = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                vector[i] = StringToVector2(reader.ReadLine());
            }
            result.Add(vector);

        }
        return result;
    }




    public static Vector3 StringToVector2(string vector_string)
    {

        if (vector_string.StartsWith("(") && vector_string.EndsWith(")"))
        {
            vector_string = vector_string.Substring(1, vector_string.Length - 2);
        }


        string[] sArray = vector_string.Split(',');


        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));

        return result;
    }

    public static int[] StringToInt(string int_string)
    {

        if (int_string.StartsWith("(") && int_string.EndsWith(")"))
        {
            int_string = int_string.Substring(1, int_string.Length - 2);
        }


        string[] sArray = int_string.Split(',');


        int[] result = new int[3] {
            int.Parse(sArray[0]),
            int.Parse(sArray[1]),
            int.Parse(sArray[2])};

        return result;
    }
}
