using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class FrameSnapshot
{
    public Vector3 acceleration = new Vector3();
    public Vector3 velocity = new Vector3();
    public Vector3 position = new Vector3();
    public float time = 0;

    public FrameSnapshot(Vector3 acc, Vector3 vel, Vector3 pos, float t)
    {
        acceleration = acc;
        velocity = vel;
        position = pos;
        time = t;
    }

    public string String()
    {
        string result = "Acceleration : " + acceleration.ToString() + "\n" +
        "Velocity : " + acceleration.ToString() + "\n" +
        "Position : " + acceleration.ToString() + "\n" +
        "Time : " + time.ToString();
        return result;
    }


}


public class PhysicsBall : MonoBehaviour
{
    [SerializeField] float mass;
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 acceleration;

    [SerializeField] List<FrameSnapshot> frames = new List<FrameSnapshot>();

    float bounciness = 0.9f;
    float radius = 0.1f;
    float time = 0;


    [SerializeField] bool colliding = false;
    int colliding_with;

    private void Awake()
    {
        mass = 10;
        position = new Vector3();
        velocity = new Vector3();
        acceleration = new Vector3();
        colliding_with = 0;
    }

    private void Start()
    {
        position = transform.position;
        time = 0;
    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        // Add Forces
        Vector3 all_forces = new Vector3();
        Vector3 gravity_force = new Vector3(0, -9.81f * mass,0);
        Vector3 normal_force = new Vector3();


        CheckCollission();

        bool on_triangle_1 = OnTriangleOne();
        //Normal Force need expanding
        if (colliding)
        {
            Vector3 normal_unit_vector = new Vector3();
            Vector3 PQ = new Vector3();
            Vector3 PR = new Vector3();
            List<Vector3[]> list = Controller.instance.Vertices();
            switch (colliding_with)
            {
                case 0:
                    PQ = list[0][1] - list[0][2];
                    PR = list[0][0] - list[0][2];                   
                    normal_unit_vector = Vector3.Cross(PQ, PR);
                    normal_unit_vector = normal_unit_vector.normalized;
                    break;
                case 1:
                    PQ = list[1][1] - list[1][2];
                    PR = list[1][0] - list[1][2];

                    normal_unit_vector = Vector3.Cross(PQ, PR);
                    normal_unit_vector = normal_unit_vector.normalized;
                    break;
                case 2:
                    PQ = list[2][2] - list[2][0];
                    PR = list[2][1] - list[2][0];

                    normal_unit_vector = Vector3.Cross(PQ, PR);
                    normal_unit_vector = normal_unit_vector.normalized;
                    break;
                case 3:
                    PQ = list[3][2] - list[3][1];
                    PR = list[3][0] - list[3][1];

                    normal_unit_vector = Vector3.Cross(PQ, PR);
                    normal_unit_vector = normal_unit_vector.normalized;
                    break;
                default:
                    break;
            }
            
            normal_force = -Vector3.Dot(normal_unit_vector, gravity_force) * normal_unit_vector;
        }


        all_forces = gravity_force + normal_force;
        AddForce(all_forces);

        velocity += acceleration * Time.fixedDeltaTime;
        position += velocity * Time.fixedDeltaTime;

        transform.position = position;

        if (on_triangle_1)
        {
            frames.Add(new FrameSnapshot(acceleration, velocity, position, time));
            Debug.Log("Added Frame");

        }
        


        acceleration = Vector3.zero;
    }

    void AddForce(Vector3 force)
    {
        acceleration = force / mass;
        
    }


    bool OnTriangleOne()
    {
        List<Vector3[]> list = Controller.instance.Vertices();
        Vector2 X = new Vector2(position.x, position.z);
        Vector2 P = new Vector2(list[0][0].x, list[0][0].z);
        Vector2 Q = new Vector2(list[0][1].x, list[0][1].z);
        Vector2 R = new Vector2(list[0][2].x, list[0][2].z);


        Vector3 bary1 = Bary(X, Q, R, P);
        if (bary1.x >= 0f && bary1.x <= 1f && bary1.y >= 0f && bary1.y <= 1f && bary1.z >= 0f && bary1.z <= 1f)
        {
            return true;
        }
        return false;
    }

    void CheckCollission()
    {
        List<Vector3[]> list = Controller.instance.Vertices();
        Vector2 X = new Vector2(position.x, position.z);
        Vector2 P = new Vector2(list[0][0].x, list[0][0].z);
        Vector2 Q = new Vector2(list[0][1].x, list[0][1].z);
        Vector2 R = new Vector2(list[0][2].x, list[0][2].z);


        Vector3 bary1 = Bary(X, Q, R, P);


        P = new Vector2(list[1][0].x, list[1][0].z);
        Q = new Vector2(list[1][1].x, list[1][1].z);
        R = new Vector2(list[1][2].x, list[1][2].z);

        Vector3 bary2 = Bary(X, Q, R, P);


        P = new Vector2(list[2][0].x, list[2][0].z);
        Q = new Vector2(list[2][1].x, list[2][1].z);
        R = new Vector2(list[2][2].x, list[2][2].z);

        Vector3 bary3 = Bary(X, Q, R, P);


        P = new Vector2(list[3][0].x, list[3][0].z);
        Q = new Vector2(list[3][1].x, list[3][1].z);
        R = new Vector2(list[3][2].x, list[3][2].z);

        Vector3 bary4 = Bary(X, Q, R, P);


        if (bary1.x >= 0f && bary1.x <= 1f && bary1.y >= 0f && bary1.y <= 1f && bary1.z >= 0f && bary1.z <= 1f)
        {
            float y = bary1.x * list[0][0].y + bary1.y * list[0][1].y + bary1.z * list[0][2].y;
            Vector3 position_on_plane = new Vector3(position.x, y, position.z);
            if (position.y - radius <= position_on_plane.y)
            {
                Vector3 PQ = list[0][1] - list[0][2];
                Vector3 PR = list[0][0] - list[0][2];

                Vector3 normal_unit_vector = Vector3.Cross(PQ, PR);
                normal_unit_vector = normal_unit_vector.normalized;

                velocity -= 2 * bounciness * Vector3.Dot(velocity, normal_unit_vector) * normal_unit_vector;
                colliding = true;
                colliding_with = 0;
            }
            else
            {
                colliding = false;
            }
        }
        else if (bary2.x >= 0f && bary2.x <= 1f && bary2.y >= 0f && bary2.y <= 1f && bary2.z >= 0f && bary2.z <= 1f)
        {
            float y = (bary2.x * list[1][0].y) + (bary2.y * list[1][1].y) + (bary2.z * list[1][2].y);
            Vector3 position_on_plane = new Vector3(position.x, y, position.z);
            if (position.y - radius <= position_on_plane.y)
            {
                Vector3 PQ = list[1][1] - list[1][2];
                Vector3 PR = list[1][0] - list[1][2];

                Vector3 normal_unit_vector = Vector3.Cross(PQ, PR);
                normal_unit_vector = normal_unit_vector.normalized;

                velocity -= 2 * bounciness * Vector3.Dot(velocity, normal_unit_vector) * normal_unit_vector;
                colliding = true;
                colliding_with = 1;
            }
            else
            {
                colliding = false;
            }
        }
        else if (bary3.x >= 0f && bary3.x <= 1f && bary3.y >= 0f && bary3.y <= 1f && bary3.z >= 0f && bary3.z <= 1f)
        {
            float y = (bary3.x * list[2][0].y) + (bary3.y * list[2][1].y) + (bary3.z * list[2][2].y);
            Vector3 position_on_plane = new Vector3(position.x, y, position.z);
            if (position.y - radius <= position_on_plane.y)
            {
                Vector3 PQ = list[2][2] - list[2][0];
                Vector3 PR = list[2][1] - list[2][0];

                Vector3 normal_unit_vector = Vector3.Cross(PQ, PR);
                normal_unit_vector = normal_unit_vector.normalized;

                velocity -= 2 * bounciness * Vector3.Dot(velocity, normal_unit_vector) * normal_unit_vector;
                colliding = true;
                colliding_with = 2;
            }
            else
            {
                colliding = false;
            }
        }
        else if (bary4.x >= 0f && bary4.x <= 1f && bary4.y >= 0f && bary4.y <= 1f && bary4.z >= 0f && bary4.z <= 1f)
        {
            float y = (bary4.x * list[3][0].y) + (bary4.y * list[3][1].y) + (bary4.z * list[3][2].y);
            Vector3 position_on_plane = new Vector3(position.x, y, position.z);
            if (position.y - radius <= position_on_plane.y)
            {
                Vector3 PQ = list[3][2] - list[3][1];
                Vector3 PR = list[3][0] - list[3][1];

                Vector3 normal_unit_vector = Vector3.Cross(PQ, PR);
                normal_unit_vector = normal_unit_vector.normalized;

                velocity -= 2 * bounciness * Vector3.Dot(velocity, normal_unit_vector) * normal_unit_vector;
                colliding = true;
                colliding_with = 3;
            }
            else
            {
                colliding = false;
            }
        }
        else
        {
            colliding = false;
        }



    }



    Vector3 Bary(Vector2 playerxz, Vector2 P, Vector2 Q, Vector2 R)
    {
        Vector2 PQ = Q - P;
        Vector2 PR = R - P;

        Vector3 PQR = Cross2D(PQ, PR);
        float normal = PQR.magnitude;


        Vector2 XP = P - playerxz;
        Vector2 XQ = Q - playerxz;
        Vector2 XR = R - playerxz;

        float x = Cross2D(XP, XQ).z / normal;
        float y = Cross2D(XQ, XR).z / normal;
        float z = Cross2D(XR, XP).z / normal;

        x *= -1f;

        y *= -1f;

        z *= -1f;

        return new Vector3(x,y,z);
    }
    Vector3 Cross2D(Vector2 A, Vector2 B)
    {
        Vector3 cross = new Vector3();

        cross.z = (A.x * B.y) - (A.y * B.x);

        return cross;
    }

}
