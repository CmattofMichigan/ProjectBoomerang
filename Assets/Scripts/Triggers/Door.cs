using UnityEngine;

public class Door : ActivationTarget
{
    [SerializeField] private float openHeight = 1;
    [SerializeField] private float openSpeed = 1;
    private bool isOpen;
    private Vector3 start;
    private Vector3 end;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start = transform.position;
        end = start;
        end.y += openHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen && transform.position != end)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, openSpeed * Time.deltaTime);
        }
        else if (!isOpen && transform.position != start)
        {
            transform.position = Vector3.MoveTowards(transform.position, start, openSpeed * Time.deltaTime);
        }
    }

    public override void activate()
    {
        isOpen = !isOpen;
    }
}
