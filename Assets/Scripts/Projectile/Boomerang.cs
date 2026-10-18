using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [SerializeField] private float throwSpeed = 15f;
    [SerializeField] private float rotSpeed = 45f;
    private float startTime;
    private Rigidbody rb;
    public bool finishForward = false;
    [SerializeField] private Transform Player;
    private Transform visual;
    private Vector3 destination;
    private Vector3 initial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
        rb = GetComponent<Rigidbody>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        visual = transform.Find("BoomerangVisual");
        initial = rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!finishForward)
        {
            destination = rb.position + transform.forward * throwSpeed * Time.deltaTime;
            destination.y = initial.y;
        }
        else
        {
            destination = Player.transform.position;
            destination.y -= 0.5f;
            destination = Vector3.MoveTowards(rb.position, destination, throwSpeed * Time.deltaTime);
        }

        rb.MovePosition(destination);
        visual.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);

        if (Time.time >= startTime + 1.0f)
        {
            finishForward = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        finishForward = true;
    }
}
