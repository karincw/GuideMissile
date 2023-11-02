using Karin.PoolingSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GuideMissile : Poolable
{
    Rigidbody rigid;

    Transform currentTarget;
    [Header("�⺻ �Ӽ���")]
    [Tooltip("�þ߰��� ����")]
    [SerializeField] private float sightScaleHalf = 35f;
    [Tooltip("�����̴� ����")]
    [SerializeField] private Vector3 moveVector;
    [Tooltip("�����̴� �ӵ�")]
    [SerializeField] private float moveSpeed = 200f;

    [Header("Ÿ��")]
    [Tooltip("Ÿ�� ������Ʈ��")]
    [SerializeField] private List<Target> objects = new List<Target>();
    [Tooltip("Ÿ�� ������Ʈ �±�")]
    [SerializeField] string tag;

    [Header("���� �Ӽ���")]
    [Tooltip("���� ���� 1ȸ�� �����Ұ���")]
    [SerializeField] private float changeValue;
    [Tooltip("ã�� ��ü�� ����")]
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
            Debug.Log("��ã��");
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

            if (cross.y > 0f) //����
            {
                Debug.Log("���� ȸ��");
                transform.Rotate(new Vector3(0f, 0f, -changeValue * radius / 5f));
                moveVector = transform.up;
            }
            else if (cross.y < 0f) //����
            {
                Debug.Log("������ ȸ��");
                transform.Rotate(new Vector3(0f, 0f, +changeValue * radius / 5f));
                moveVector = transform.up;// y���� �¿� [-] =���� [+] =������
            }
            else
            {
                Debug.Log("x������ ����");
            }

            cross = Vector3.Cross(currentTarget.transform.position - transform.position, transform.forward);

            //if (cross.y > 0f) //����
            //{
            //    Debug.Log("���� ȸ��");
            //    transform.Rotate(new Vector3(-changeValue, 0f, 0f));
            //    moveVector = transform.up;

            //}
            //else if (cross.y < 0f) //����
            //{
            //    Debug.Log("�Ʒ��� ȸ��");
            //    transform.Rotate(new Vector3(+changeValue, 0f, 0f));
            //    moveVector = transform.up;

            //}
            //else
            //{
            //    Debug.Log("y������ ����");
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

        if (0 <= CosA && CosA < sightScaleHalf) // ���ʿ� ����
                                                // Cos �̿��� Ŭ���� ���� �� �ݴ��� �����ϸ� ��
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
