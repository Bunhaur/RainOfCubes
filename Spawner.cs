using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Cube _cube;
    [SerializeField] private float _positionX;
    [SerializeField] private float _positionZ;
    [SerializeField] private float _repeatRate = 1f;
    [SerializeField] private int _maxSize;
    [SerializeField] private int _poolCapacity;

    private ObjectPool<Cube> _pool;
    private float _positionY = 15;
    private float _offset;
    private byte _halfValue = 2;
    private Quaternion _quaternion;


    private void Awake()
    {
        _quaternion = Quaternion.identity;
        _offset = _cube.transform.localScale.x / _halfValue;
        StartPool();
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), 0f, _repeatRate);
    }

    private void GetCube()
    {
        _pool.Get();
    }

    private void StartPool()
    {
        _pool = new ObjectPool<Cube>(
            createFunc: () => CreateCube(),
            actionOnGet: (cube) => ActionOnGet(cube),
            actionOnRelease: (cube) => cube.gameObject.SetActive(false),
            actionOnDestroy: (cube) => Destroy(cube),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _maxSize);
    }

    private float GetRandomValue(float value)
    {
        value = Random.Range(-value + _offset, value - _offset);
        return value;
    }

    private Cube CreateCube()
    {
        Cube cube = Instantiate(_cube, GetRandomPosition(), _quaternion);
        cube.Dead += ReturnCubeInPool;

        return cube;
    }

    private void ReturnCubeInPool(Cube cube)
    {
        _pool.Release(cube);
    }

    private void ActionOnGet(Cube cube)
    {
        cube.transform.position = GetRandomPosition();
        cube.gameObject.SetActive(true);
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(GetRandomValue(_positionX), _positionY, GetRandomValue(_positionZ));
    }
}