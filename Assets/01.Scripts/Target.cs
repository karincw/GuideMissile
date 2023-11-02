using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : TargetObject
{
    [SerializeField] private float maxHp;
    public float CurrentHp;

    private void Awake()
    {
        CurrentHp = maxHp;
    }

    private void OnEnable()
    {
        transform.position = new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-50f, -10f));
    }

    public override void Hit(float Atk)
    {
        CurrentHp -= Atk;
        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Release();
    }
}
