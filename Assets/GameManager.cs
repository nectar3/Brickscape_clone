using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform cubeSpace;

    private Camera _cam;

    public bool isBlockDragging = false;

    public float rotationSpeed = 3f;

    private static GameManager instance = null;
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
            Destroy(this.gameObject);
    }
    public static GameManager I
    {
        get
        {
            if (null == instance)
                return null;
            return instance;
        }
    }

    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if(isBlockDragging == false)
        {
            if (Input.GetMouseButton(0))
            {
                cubeSpace.Rotate(0f, -Input.GetAxis("Mouse X") * rotationSpeed, 0f, Space.World);
                cubeSpace.Rotate(-Input.GetAxis("Mouse Y") * rotationSpeed, 0f, 0f);
            }
        }

    }

}
