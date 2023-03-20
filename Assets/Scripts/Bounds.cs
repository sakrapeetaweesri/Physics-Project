using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Beyblade b))
        {
            GameManager.Instance.OutOfBound(b);
            b.ForceStop();
        }
    }
}