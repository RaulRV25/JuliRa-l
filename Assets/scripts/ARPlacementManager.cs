using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARPlacementManager : MonoBehaviour
{
    [Header("Componentes de AR Foundation")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Prefabs de los Modelos 3D")]
    [SerializeField] private GameObject horizontalModelPrefab; // Objeto para suelo/mesa
    [SerializeField] private GameObject verticalModelPrefab;   // Objeto para pared

    [Header("Interfaz de Usuario (UI)")]
    [SerializeField] private TextMeshProUGUI statusText;

    // 0 = Modelo Horizontal Seleccionado | 1 = Modelo Vertical Seleccionado
    private int selectedModelIndex = 0;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Start()
    {
        SetStatusMessage("Escaneando entorno ...");
    }

    private void Update()
    {
        // 1. Detectar e informar sobre planos en pantalla
        CheckDetectedPlanes();

        // 2. Procesar el toque del usuario
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Evitar instanciar si el usuario tocó un botón de la UI
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                TryPlaceObject(touch.position);
            }
        }
    }

    private void CheckDetectedPlanes()
    {
        if (planeManager.trackables.count > 0)
        {
            foreach (var plane in planeManager.trackables)
            {
                if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
                {
                    SetStatusMessage("Superficie horizontal detectada!");
                    break;
                }
                else if (plane.alignment == PlaneAlignment.Vertical)
                {
                    SetStatusMessage("Superficie vertical detectada!");
                    break;
                }
            }
        }
        else
        {
            SetStatusMessage("Escaneando entorno ...");
        }
    }

    // Método que llamarán los botones de la UI
    public void SelectModel(int index)
    {
        selectedModelIndex = index;
    }

    private void TryPlaceObject(Vector2 touchPosition)
    {
        // Realizar Raycast hacia las superficies detectadas por AR Foundation
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            TrackableId planeId = hits[0].trackableId;
            ARPlane hitPlane = planeManager.GetPlane(planeId);

            if (hitPlane == null) return;

            bool isHorizontalPlane = hitPlane.alignment == PlaneAlignment.HorizontalUp || hitPlane.alignment == PlaneAlignment.HorizontalDown;
            bool isVerticalPlane = hitPlane.alignment == PlaneAlignment.Vertical;

            // Validación estricta: Modelo Horizontal -> Solo en plano horizontal
            if (selectedModelIndex == 0 && isHorizontalPlane)
            {
                Instantiate(horizontalModelPrefab, hitPose.position, hitPose.rotation);
                SetStatusMessage("Objeto colocado en plano horizontal");
            }
            // Validación estricta: Modelo Vertical -> Solo en plano vertical
            else if (selectedModelIndex == 1 && isVerticalPlane)
            {
                Instantiate(verticalModelPrefab, hitPose.position, hitPose.rotation);
                SetStatusMessage("Objeto colocado en plano vertical");
            }
        }
    }

    public void SetStatusMessage(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}