using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask;

    [Space]
    [Header("Constraints")]
    [SerializeField] private Vector3 leftLimit;
    [SerializeField] private Vector3 rightLimit;
    
    public Vector3 AimingPosition { get; private set; }

    private void Start()
    {
        lineRenderer.positionCount = 2;

        GameManager.OnGameStarted += GameStarted;
    }
    private void OnDestroy()
    {
        GameManager.OnGameStarted -= GameStarted;
    }

    private void GameStarted()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (GameManager.GameStarted) return;

        var camRay = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(camRay, out RaycastHit hit, float.MaxValue, mask))
        {
            var destination = new Vector3(hit.point.x, player.position.y, hit.point.z);

            lineRenderer.SetPosition(0, player.position);
            lineRenderer.SetPosition(1, destination);

            AimingPosition = (destination - player.position).normalized;
        }
    }
}