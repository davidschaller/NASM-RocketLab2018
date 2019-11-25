using UnityEngine;
using System.Collections;


public class DoorController : MonoBehaviour
{
    public bool slider = false;

    void Awake()
    {
    }

    void Update()
    {
    }

    public void OnTriggerEnter(Collider coll)
    {
		if (coll.tag == "Player")
		{
	        if (!doorIdle || isOpened)
	            return;
	
	        if (slider)
	            StartCoroutine(OpenSlider());
	        else
	            StartCoroutine(Open());
		}
    }

    private IEnumerator Open()
    {
		doorIdle = false;
        float angle = 0;

        while (transform.localRotation.eulerAngles.y < 90)
        {
            transform.Rotate(Vector3.up, angle);

            angle += Time.deltaTime * 10;

            yield return 0;
        }

        doorIdle = true;
        isOpened = true;
    }

    private IEnumerator Close()
    {
        float angle = 0;

        while (transform.localRotation.eulerAngles.y > 0 && transform.localRotation.eulerAngles.y < 300)
        {
            doorIdle = false;
            transform.Rotate(Vector3.up, -angle);

            angle += Time.deltaTime * 10;

            yield return 0;
        }

        transform.localRotation = new Quaternion(0, 0, 0, 0);

        doorIdle = true;
        isOpened = false;
    }

    private bool doorIdle = true;
    private bool isOpened = false;

    private IEnumerator OpenSlider()
    {
        float t = 0;

        Vector3 oldPos = transform.position;
        Vector3 newPos = transform.position + transform.TransformDirection(Vector3.right * 1);

        while (t < 1)
        {
            doorIdle = false;
            transform.position = Vector3.Lerp(oldPos, newPos, t);

            t+= Time.deltaTime * 3;
            yield return 0;
        }

        transform.position = newPos;
        yield return 0;

        doorIdle = true;
        isOpened = true;
    }

    private IEnumerator CloseSlider()
    {
        float t = 0;

        Vector3 oldPos = transform.position;
        Vector3 newPos = transform.position + transform.TransformDirection(Vector3.left * 1);

        while (t < 1)
        {
            doorIdle = false;
            transform.position = Vector3.Lerp(oldPos, newPos, t);

            t += Time.deltaTime * 3;
            yield return 0;
        }

        transform.position = newPos;
        yield return 0;

        doorIdle = true;
        isOpened = false;
    }

    public void OnTriggerExit(Collider coll)
    {
        if (!doorIdle || !isOpened)
            return;

        if (slider)
            StartCoroutine(CloseSlider());
        else
            StartCoroutine(Close());
    }
}