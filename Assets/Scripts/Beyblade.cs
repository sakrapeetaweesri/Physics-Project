using System;
using System.Collections.Generic;
using UnityEngine;

public class Beyblade : UniversalGravitational
{
    [SerializeField] private bool isPlayer;

    [SerializeField] private float maxAngualarVelocity;
    [SerializeField] private float AngualarVelocityDecayRate;
    [SerializeField] private float currentAngularVelocity;
    [SerializeField] private float bendToOthersStrength;
    [SerializeField] private float steeringStrength;
    [SerializeField] private float magnusStrength;

    public static List<Beyblade> beyblades;

    private bool isLaunched = false;
    private bool isStoppedSpinning = false;
    private float determinedDir;

    public Action<Beyblade> OnSpinningStopped;

    protected override void Awake()
    {
        base.Awake();

        if (beyblades == null) beyblades = new List<Beyblade>();
        beyblades.Add(this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        beyblades.Remove(this);
    }

    public void ReleaseBeyblade(Vector3 releaseForce, float initialAngularVelocity)
    {
        // Unfreezes the body.
        rb.constraints = RigidbodyConstraints.None;

        // Launches the beyblade.
        rb.AddForce(releaseForce, ForceMode.Impulse);

        // Starts spinning.
        rb.maxAngularVelocity = maxAngualarVelocity;

        if (isPlayer && (determinedDir < 0f || determinedDir > 0f))
        {
            rb.angularVelocity = new Vector3(0, Mathf.Sign(determinedDir) * initialAngularVelocity, 0);
        }
        else
        {
            magnusStrength = 0f;
            rb.angularVelocity = new Vector3(0, initialAngularVelocity, 0);
        }

        isLaunched = true;
    }

    public void Update()
    {
        if (!isLaunched)
        {
            determinedDir = Input.GetAxisRaw("Horizontal");
        }

        currentAngularVelocity = rb.angularVelocity.y;

        if (isLaunched && !isStoppedSpinning && Mathf.Abs(currentAngularVelocity) <= 0.1f)
        {
            preventAttraction = true;
            isStoppedSpinning = true;
            OnSpinningStopped?.Invoke(this);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        rb.angularDrag = AngualarVelocityDecayRate;

        // Stops bending, applying magnus, and steering when low on spinning speed.
        if (currentAngularVelocity / maxAngualarVelocity <= 0.2f) return;

        float hor_input = Input.GetAxisRaw("Horizontal");
        float vert_input = Input.GetAxisRaw("Vertical");
        if (isPlayer)
        {
            BendMovement(hor_input, vert_input);
        }

        foreach (var b in beyblades)
        {
            BendToOtherBeyblades(b);
        }

        ApplyMagnusEffect();
    }

    private void BendToOtherBeyblades(Beyblade otherBeyblade)
    {
        if (otherBeyblade == this) return;

        var distance = (otherBeyblade._transform.position - _transform.position).normalized;
        rb.AddForce(distance * bendToOthersStrength, ForceMode.Force);
    }

    private void BendMovement(float hor_input, float vert_input)
    {
        var moveDir = new Vector3(hor_input, 0f, vert_input).normalized;
        rb.AddForce(steeringStrength * moveDir, ForceMode.Force);
    }
    private void ApplyMagnusEffect()
    {
        var magnusDir = Vector3.Cross(rb.angularVelocity, rb.velocity);
        rb.AddForce(magnusDir.normalized * magnusStrength, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Removes magnus effect when hit the ground
        magnusStrength = 0f;
    }

    public void ForceStop()
    {
        rb.angularVelocity = Vector3.zero;
    }

    public float GetAngularVelocityRatio()
    {
        if (!GameManager.GameStarted) return 1f;

        return Mathf.Abs(rb.angularVelocity.y) / maxAngualarVelocity;
    }
}