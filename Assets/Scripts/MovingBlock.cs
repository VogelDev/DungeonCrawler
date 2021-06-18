using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public enum PushDirections
    {
        Left,
        Right,
        Up,
        Down
    }

    public List<PushDirections> Pushes = new List<PushDirections>();
    public int MaxPushes;
    public Vector3 startMarker;
    public Vector3 endMarker;

    private float journeyLength;
    public float speed = .01f;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        speed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if(startMarker != Vector3.zero && endMarker != Vector3.zero && transform.position != endMarker)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Debug.Log($"distCovered / journeyLength {distCovered / journeyLength}");

            transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.transform.gameObject.name != "Player")
        {
            return;
        }

        //Debug.Log($"Collision position: {collision.transform.position}");
        //Debug.Log($"Collision localScale: {collision.transform.localScale}");

        if (MaxPushes <= 0)
        {
            return;
        }


        if (Pushes.Contains(PushDirections.Left) && collision.transform.position.x + collision.transform.localScale.x - .15 <= transform.position.x)
        {
            //transform.Translate(new Vector3(1, 0, 0));
            startMarker = transform.position;
            endMarker = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            MaxPushes--;
        }


        if(Pushes.Contains(PushDirections.Right) && collision.transform.position.x + .15 >= transform.position.x + transform.localScale.x)
        {
            startMarker = transform.position;
            endMarker = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            //transform.Translate(new Vector3(-1, 0, 0));
            MaxPushes--;

        }

        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker, endMarker);
    }
}
