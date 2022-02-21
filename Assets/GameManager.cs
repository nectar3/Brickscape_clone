using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform cubeSpace;

    [HideInInspector]
    public float spaceSize;

    private Camera _cam;

    private static GameManager instance = null;
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            spaceSize = cubeSpace.localScale.x;

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
        Debug.Log("gm start");
        _cam = Camera.main;
    }



}
