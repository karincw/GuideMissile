using Karin.PoolingSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance { get => instance; }

    public PoolManager poolManager;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        poolManager = GetComponent<PoolManager>();
    }

}
