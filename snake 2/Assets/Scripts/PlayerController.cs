using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Tags
{
    public static string WALL = "Wall";
    public static string BEAN = "Bean";
    public static string TAIL = "Tail";
}

public class Metrics
{
    public static float NODE = 0.5f;
}

public enum PlayerDirection
{
    LEFT = 0,
    UP = 1,
    RIGHT = 2,
    DOWN = 3,
    COUNT = 4,
}
public class PlayerController : MonoBehaviour
{
    public TMP_Text pointsText;
    private int pointsCount = 0;

    public PlayerDirection _playerdirection;
    
    public float step_Length = 0.2f;

    public float movement_freq = 0.1f;

    [SerializeField] private GameObject tailPrefab;
    
    private List<Vector3> deltaPosition;

    private List<Rigidbody> nodes;

    private Rigidbody mainBody;
    private Rigidbody headBody;
    private Transform tr;

    private float speed;
    private float counter;
    private bool move;

    private bool createTail;
    
    void Awake()
    {
        tr = transform;
        mainBody = GetComponent<Rigidbody>();

        InitSnakeNodes();
        InitPlayer();

        deltaPosition = new List<Vector3>()
        {
            new Vector3(-step_Length, 0f),
            new Vector3(0f, step_Length),
            new Vector3(step_Length, 0f),
            new Vector3(0f, -step_Length)
        };
    }
    void Update()
    {
        CheckMovementFrequency();
        showPoints();
    }

    private void FixedUpdate()
    {
        if (move)
        {
            move = false;
            
            Move();
        }
        
    }
    private void InitPlayer()
    {
        SetDirectionRandom();
        
        switch (_playerdirection)
        {
            case PlayerDirection.RIGHT:
                nodes[1].position = nodes[0].position - new Vector3(Metrics.NODE, 0f, 0f);
                nodes[2].position = nodes[0].position - new Vector3(Metrics.NODE * 2f, 0f, 0f);
                break;
            case PlayerDirection.LEFT:
                nodes[1].position = nodes[0].position + new Vector3(Metrics.NODE, 0f, 0f);
                nodes[2].position = nodes[0].position + new Vector3(Metrics.NODE * 2f, 0f, 0f);
                break;
            case PlayerDirection.UP:
                nodes[1].position = nodes[0].position - new Vector3(0f, Metrics.NODE, 0f);
                nodes[2].position = nodes[0].position - new Vector3(0f, Metrics.NODE * 2f, 0f);
                break;
            case PlayerDirection.DOWN:
                nodes[1].position = nodes[0].position + new Vector3(0f, Metrics.NODE, 0f);
                nodes[2].position = nodes[0].position + new Vector3(0f, Metrics.NODE * 2f, 0f);
                break;
        }
    }
    private void InitSnakeNodes()
    {
        nodes = new List<Rigidbody>();
        nodes.Add(tr.GetChild(0).GetComponent<Rigidbody>());
        nodes.Add(tr.GetChild(1).GetComponent<Rigidbody>());
        nodes.Add(tr.GetChild(2).GetComponent<Rigidbody>());

        headBody = nodes[0];
    }
    public void SetInputDirection(PlayerDirection dir)
    {
        if (dir == PlayerDirection.UP && _playerdirection == PlayerDirection.DOWN ||
            dir == PlayerDirection.DOWN && _playerdirection == PlayerDirection.UP ||
            dir == PlayerDirection.RIGHT && _playerdirection == PlayerDirection.LEFT ||
            dir == PlayerDirection.LEFT && _playerdirection == PlayerDirection.RIGHT)
        {
            return;
        }

        _playerdirection = dir;

        ForceMove();
    }
    void CheckMovementFrequency()
    {
        counter += Time.deltaTime;

        if (counter >= movement_freq)
        {
            counter = 0;
            move = true;
        }
    }
    void SetDirectionRandom()
    {
        int dirRandom = Random.Range(0, (int)PlayerDirection.COUNT);
        _playerdirection = (PlayerDirection)dirRandom;
    }
    void Move()
    {
        Vector3 dPosition = deltaPosition[(int) _playerdirection];
        Vector3 parentPos = headBody.position;
        Vector3 prevPosition;
        mainBody.position = mainBody.position + dPosition;
        headBody.position = headBody.position + dPosition;

        for (int i = 1; i < nodes.Count; i++)
        {
            prevPosition = nodes[i].position;
            nodes[i].position = parentPos;
            parentPos = prevPosition;
        }

        if (createTail)
        {
            createTail = false;

            GameObject newNode = Instantiate(tailPrefab, nodes[nodes.Count - 1].position, Quaternion.identity);
            
            newNode.transform.SetParent(transform, true);
            nodes.Add(newNode.GetComponent<Rigidbody>());
        }
    }
    void ForceMove()
    {
        counter = 0;
        move = false;
        Move();
    }
    private void showPoints()
    {
        pointsText.text = String.Format("{0}", pointsCount);
    }
    private void OnTriggerEnter(Collider target)
    {
        if (target.tag == Tags.BEAN)
        {
            target.gameObject.SetActive(false);

            createTail = true;
            
            pointsCount += 1;
            
            step_Length += 1f;

            speed += 100f;
            
            Destroy(target.gameObject);

            GameController.instance.StartCoroutine("Spawn");
        }
        
        if (target.tag == Tags.WALL || target.tag == Tags.TAIL)
        {
            SceneManager.LoadScene(0);
        }
    }

}
