using System.Collections.Generic;
using UnityEngine;

public class UniversalGravitational : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;
    private const float GRAVITATIONAL_CONSTANT = 6.671f;
    protected bool preventAttraction = false;

    protected Transform _transform;

    public static List<UniversalGravitational> gravitatingObjects;
    protected virtual void Awake()
    {
        _transform = transform;

        if (gravitatingObjects == null) gravitatingObjects = new List<UniversalGravitational>();
        gravitatingObjects.Add(this);
    }
    protected virtual void OnDestroy()
    {
        gravitatingObjects.Remove(this);
    }

    protected virtual void FixedUpdate()
    {
        foreach (var o in gravitatingObjects)
        {
            if (o == this) continue;

            Attract(o);
        }
    }

    private void Attract(UniversalGravitational o)
    {
        if (o.preventAttraction) return;

        var direction = rb.position - o.rb.position;
        var gForceMagnitude = GRAVITATIONAL_CONSTANT * rb.mass * o.rb.mass / direction.sqrMagnitude;

        o.rb.AddForce(direction.normalized * gForceMagnitude);
    }
}