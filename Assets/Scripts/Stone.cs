using UnityEngine;

public class Stone : MonoBehaviour
{
    public Rigidbody rb;
    private Vector3 startedVector;
    public bool isMovementEnabled;
    private float Speed = 0.0f;
    private float MaxSpeed = 0.25f;
    private float Acceleration = 0.25f;
    private float Deceleration;
    private bool isMaximumSpeedReached = false;
    private float SideForce;

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "LAUNCHER")
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionX;
            rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            isMovementEnabled = true;
        }
    }

    void Start()
    {
        isMovementEnabled = false;
        startedVector = new Vector3(0, 0.0655f, 1.23f);
        Deceleration = Random.Range(0.033f, 0.06f);
        SideForce = Random.Range(-0.2f, 0.2f);
    }

    void Update()
    {
        Movement();
    }

    public void SetStoneColor(GameBoard.Team team)
    {
        var color = GameBoard.Team.RED == team ? "PlayerOneSkin" : "PlayerTwoSkin";

        Material newMaterial = Resources.Load(color, typeof(Material)) as Material;
        foreach(Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (r.name != "StoneBase") 
            {
                r.material = newMaterial;
            }
        }
    }

    private void Movement()
    {
        if (isMovementEnabled)
        {
            if (!isMaximumSpeedReached && Speed < MaxSpeed) {
                Speed += Acceleration * Time.deltaTime;
            } else if (Speed <= 0.0f) {
                Speed = 0.0f;
            } else {
                isMaximumSpeedReached = true;
                Speed -= Deceleration * Time.deltaTime;
            }
                
            Vector3 p = startedVector;
            p.z += Speed * Time.deltaTime;
            p.x += Speed * Time.deltaTime * SideForce;
            startedVector = p;
            transform.localPosition = startedVector;
        }
    }
}
