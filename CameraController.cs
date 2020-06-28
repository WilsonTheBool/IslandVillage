using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    Camera cam;
    public float speed;

    public float leftBorder;

    public float rightBorder;

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;

        if(mouse.x <= cam.pixelWidth * 0.01)
        {
            if(cam.transform.position.x - speed * Time.deltaTime > leftBorder)
            gameObject.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }

        if(mouse.x >= cam.pixelWidth * 0.99)
        {
            if (cam.transform.position.x + speed * Time.deltaTime < rightBorder)
                gameObject.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
            
    }
}
