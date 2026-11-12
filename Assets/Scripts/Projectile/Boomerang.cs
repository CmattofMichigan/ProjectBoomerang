using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [SerializeField] private float throwSpeed = 20f;
    [SerializeField] private float rotSpeed = 360f;
    [SerializeField] private float returnDelay = 0.5f;
    [SerializeField] private float failsafeTimer = 10f;
    private float startTime;
    private Rigidbody rb;
    public bool finishForward = false;
    [SerializeField] private GameObject Player;
    private Transform PlayerTranform;
    private Transform visual;
    private Vector3 destination;
    private Vector3 initial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
        rb = GetComponent<Rigidbody>();
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerTranform = Player.transform;
        visual = transform.Find("BoomerangVisual");
        initial = rb.position;
        //Debug.Log("start completed");
        //Debug.Log($"Object name: {Player.name}");
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
            destination = PlayerTranform.position;
            destination.y -= 0.5f;
            destination = Vector3.MoveTowards(rb.position, destination, throwSpeed * Time.deltaTime);
        }

        rb.MovePosition(destination);
        visual.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);

        if (Time.time >= startTime + returnDelay)
        {
            finishForward = true;
        }

        if (Time.time >= startTime + failsafeTimer)
        {
            Player.GetComponent<PlayerController>().ReturnBoomerang();
            //Debug.Log("Boomerang failsafe triggered");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        finishForward = true;
    }
}
