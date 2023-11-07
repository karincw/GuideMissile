using Karin.PoolingSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class SimpleGuideMissile : Poolable
{
    Rigidbody rigid;

    Transform currentTarget;
    [Header("기본 속성값")]
    [Tooltip("시야각의 절반")]
    [SerializeField] private float sightScaleHalf = 35f;
    [Tooltip("움직이는 방향")]
    [SerializeField] private Vector3 moveVector;
    [Tooltip("움직이는 속도")]
    [SerializeField] private float moveSpeed = 200f;

    [Header("타겟")]
    [Tooltip("타겟 오브젝트들")]
    [SerializeField] private List<Target> objects = new List<Target>();
    private List<Target> Objects
    {
        get
        {
            objects.Clear();
            objects.AddRange(GameObject.FindObjectsOfType<Target>());
            objects = objects.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).ToList();
            Debug.Log("Find End");
            return objects;
        }
        set
        {

        }
    }
    [Tooltip("타겟 오브젝트 태그")]
    [SerializeField] string tag;

    [Header("유도 속성값")]
    [Tooltip("각도 오차 1회당 수정할각도")]
    [SerializeField] private float changeValue;
    [Tooltip("회전속도")]
    [SerializeField] private float RotationSpeed = 2.0f;
    [Tooltip("찾은 물체의 각도")]
    [SerializeField] private float radius;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {

    }

    private void Update()
    {
        if (currentTarget == null)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                currentTarget = FindTarget(objects[i].transform, out radius);

            }

            Debug.Log("찾는중");
        }
        else
        {

            currentTarget = FindTarget(currentTarget, out radius);
            if (currentTarget == null || currentTarget.gameObject.activeInHierarchy == false) return;

            Vector3 dir = currentTarget.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        }

        transform.Translate(moveVector * moveSpeed * Time.deltaTime);
    }

    public void SetMoveVector()
    {
        moveVector = transform.forward;
    }

    private Transform FindTarget(Transform target, out float Radius)
    {
        Radius = 0f;
        moveVector = transform.up;
        Vector3 frontVector = transform.up;
        Vector3 targetVector = target.position - transform.position;

        float Dot = Vector3.Dot(frontVector.normalized, targetVector.normalized);
        float CosA = Mathf.Acos(Dot) * Mathf.Rad2Deg;


        if (0 <= CosA && CosA < sightScaleHalf) // 안쪽에 있음
                                                // Cos 이여서 클수록 작음 즉 반대라고 생각하면 됨
        {
            Radius = CosA;
            return target;
        }
        else
        {
            Radius = CosA;
            return null;
        }
    }

    public override void Release()
    {
        rigid.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rigid.angularVelocity = Vector3.zero;
        base.Release();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tag))
        {
            collision.gameObject.GetComponent<Target>().Hit(20);
        }

        Release();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(3, 3, 3));
    }
}
