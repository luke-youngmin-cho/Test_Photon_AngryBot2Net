using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _effect;
    [SerializeField] private float _speed = 1000f;
    private void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * _speed);
        Destroy(gameObject, 3.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var contact = collision.GetContact(0);
        var obj = Instantiate(_effect,
                              contact.point,
                              Quaternion.LookRotation(-contact.normal));
        Destroy(obj, 2.0f);
        Destroy(gameObject);
    }
}
