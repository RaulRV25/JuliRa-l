using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class ARPlacementController : MonoBehaviour
{
    [Header("Modelos Personalizados (No Primitivas)")]
    [SerializeField] private GameObject horizontalPrefab;
    [SerializeField] private GameObject verticalPrefab;

    private ARRaycastManager _raycastManager;
    private ARPlaneManager _planeManager;
    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
    }

    private void Update()
    {
        // Verificar entrada táctil en pantalla
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        // Ejecutar Raycast contra planos detectados (Horizontales y Verticales)
        if (_raycastManager.Raycast(touch.position, _hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _hits[0].pose;
            TrackableId planeId = _hits[0].trackableId;
            ARPlane hitPlane = _planeManager.GetPlane(planeId);

            if (hitPlane != null)
            {
                SpawnModel(hitPlane.alignment, hitPose);
            }
        }
    }

    private void SpawnModel(PlaneAlignment alignment, Pose pose)
    {
        // Evaluar la alineación del plano detectado
        if (alignment == PlaneAlignment.HorizontalUp || alignment == PlaneAlignment.HorizontalDown)
        {
            if (horizontalPrefab != null)
            {
                Instantiate(horizontalPrefab, pose.position, pose.rotation);
            }
        }
        else if (alignment == PlaneAlignment.Vertical)
        {
            if (verticalPrefab != null)
            {
                Instantiate(verticalPrefab, pose.position, pose.rotation);
            }
        }
    }
}
