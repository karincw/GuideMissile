using Karin.PoolingSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GuideMissile : Poolable
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
    [Tooltip("타겟 오브젝트 태그")]
    [SerializeField] string tag;

    [Header("유도 속성값")]
    [Tooltip("각도 오차 1회당 수정할각도")]
    [SerializeField] private float changeValue;
    [Tooltip("찾은 물체의 각도")]
    [SerializeField] private float radius;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        objects.AddRange(GameObject.FindObjectsOfType<Target>().ToList());
        objects.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude);
        Debug.Log("Find End");
    }

    private void OnDisable()
    {
        objects.Clear();
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            Debug.Log("못찾음");
            foreach (var item in objects)
            {
                currentTarget = FindTarget(item.transform, out radius);
            }
        }
        else
        {
            Debug.Log("Find");
            currentTarget = FindTarget(currentTarget, out radius);
            if (currentTarget == null) return;

            Vector3 cross = Vector3.Cross(currentTarget.transform.position, transform.position);

            if (cross.y > 0f) //우측
            {
                Debug.Log("왿쪽 회전");
                transform.Rotate(new Vector3(0f, 0f, -changeValue * radius / 5f));
                moveVector = transform.up;
            }
            else if (cross.y < 0f) //좌측
            {
                Debug.Log("오른쪽 회전");
                transform.Rotate(new Vector3(0f, 0f, +changeValue * radius / 5f));
                moveVector = transform.up;// y값이 좌우 [-] =왼쪽 [+] =오른쪽
            }
            else
            {
                Debug.Log("x방향이 같음");
            }

            cross = Vector3.Cross(currentTarget.transform.position - transform.position, transform.forward);

            //if (cross.y > 0f) //우측
            //{
            //    Debug.Log("위쪽 회전");
            //    transform.Rotate(new Vector3(-changeValue, 0f, 0f));
            //    moveVector = transform.up;

            //}
            //else if (cross.y < 0f) //좌측
            //{
            //    Debug.Log("아래쪽 회전");
            //    transform.Rotate(new Vector3(+changeValue, 0f, 0f));
            //    moveVector = transform.up;

            //}
            //else
            //{
            //    Debug.Log("y방향이 같음");
            //}
        }
    }

    public void SetMoveVector()
    {
        moveVector = transform.up;
    }

    private void FixedUpdate()
    {
        rigid.velocity = moveVector * Time.fixedDeltaTime * moveSpeed;
    }

    private Transform FindTarget(Transform target, out float Radius)
    {
        moveVector = transform.up;
        Vector3 frontVector = transform.up;
        Vector3 targetVector = target.position - transform.position;

        float Dot = Vector3.Dot(frontVector.normalized, targetVector.normalized);
        float CosA = Mathf.Acos(Dot) * Mathf.Rad2Deg;

        Debug.DrawRay(transform.position, new Vector3(sightScaleHalf, 0, 0), Color.black, 5f);
        Debug.DrawRay(transform.position, new Vector3(-sightScaleHalf, 0, 0), Color.black, 5f);

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

}
