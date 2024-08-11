using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]

public class Cube : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Coroutine _countDownStart;
    private Color _defaulColor;
    private bool _isCollision;
    private float _minLifeTime = 2f;
    private float _maxLifeTime = 5f;
    private Quaternion _defaultRotation;
    private Rigidbody _rigidBody;

    public event Action<Cube> Dead;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _defaultRotation = transform.rotation;
        _meshRenderer = GetComponent<MeshRenderer>();
        _defaulColor = _meshRenderer.material.color;
    }

    private void OnDisable()
    {
        ResetCube();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isCollision == false && collision.collider.TryGetComponent(out Plane plane))
        {
            _isCollision = true;
            ChangeColor();
            _countDownStart = StartCoroutine(CountDown());
        }
    }

    private void ResetCube()
    {
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
        transform.rotation = _defaultRotation;
        _meshRenderer.material.color = _defaulColor;       
        _isCollision = false;
    }

    private void ChangeColor()
    {
        _meshRenderer.material.color = UnityEngine.Random.ColorHSV();
    }

    private IEnumerator CountDown()
    {
        var wait = new WaitForSeconds(GetRandomLifeTime());

        yield return wait;

        Dead?.Invoke(this);
    }

    private float GetRandomLifeTime()
    {
        return UnityEngine.Random.Range(_minLifeTime, _maxLifeTime);
    }
}