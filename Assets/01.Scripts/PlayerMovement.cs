using Karin.PoolingSystem;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject guideBullet;
    [SerializeField] private Transform summonTransform;
    [SerializeField] List<GameObject> bullets = new List<GameObject>();

    private float movementSpeed = 100f;
    private int deleteIndex = 0;

    private Vector3 moveVector;
    private Rigidbody rigid;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bullets.Clear();
    }

    private void Update()
    {
        moveVector = new Vector3(-Input.GetAxisRaw("Horizontal"), 0, -Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject bullet = GameManager.instance.poolManager.Spawn("GuideMissile");

            bullet.transform.position = summonTransform.position;
            bullet.transform.rotation = summonTransform.localRotation;
            bullet.GetComponent<GuideMissile>().SetMoveVector();

            bullets.Add(bullet);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.poolManager.Spawn("Target");
        }

            if (Input.GetKeyDown(KeyCode.E))
        {
            bullets[deleteIndex++].GetComponent<Poolable>().Release();
        }

    }

    private void FixedUpdate()
    {
        rigid.velocity = moveVector * Time.fixedDeltaTime * movementSpeed;
    }
}
