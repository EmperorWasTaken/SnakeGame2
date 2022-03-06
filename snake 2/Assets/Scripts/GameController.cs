using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject bean;

    private float minimum_x = -8.5f, maximum_x = 8.5f, minimum_y = -8.5f, maximum_y = 8.5f;
    private float z_position = -0.36f;

    private void Awake()
    {
        MakeInstance();
    }

    private void Start()
    {
        throw new NotImplementedException();
    }

    void MakeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Spawn()
    {
        //StartCoroutine(SpawnBean());
        Instantiate(bean,
            new Vector3(Random.Range(minimum_x, maximum_x), Random.Range(minimum_y, maximum_y), z_position),
            Quaternion.identity);
        
    }

    IEnumerator SpawnBean()
    {
        yield return new WaitForSeconds(Random.Range(1f, 1.5f));
        Instantiate(bean,
            new Vector3(Random.Range(minimum_x, maximum_x), Random.Range(minimum_y, maximum_y), z_position),
            Quaternion.identity);
        Invoke("Spawn", 0f);
    }
}
