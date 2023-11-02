using Karin.PoolingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetObject : Poolable
{
    public abstract void Hit(float Atk);
}
