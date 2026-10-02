using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;
using TMPro;

public class RaycastController : MonoBehaviour
{
    [Header("Componentes AR")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Prefabs de los Modelos")]
    [SerializeField] private GameObject horizontalModelPrefab; // Modelo para piso/mesa
    [SerializeField] private GameObject verticalModelPrefab;   // Modelo para pared

    [Header("UI")]
    [SerializeField] private TMP_Text debugText;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // 0 = Modelo Horizontal, 1 = Modelo Vertical
    private int selectedModelIndex = 0;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable(); // Ayuda a probar toques táctiles
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        TouchSimulation.Disable();
    }

    private void Update()
    {
        var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

        if (activeTouches.Count == 0)
        {
            return;
        }

        var touch = activeTouches[0];

        // Procesar solo en el instante en que inicia el toque
        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
            return;

        // Hacer Raycast contra planos detectados
        if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            TrackableId planeId = hits[0].trackableId;
            ARPlane hitPlane = planeManager.GetPlane(planeId);

            if (hitPlane == null) return;

            // Verificar si el plano impactado es horizontal o vertical
            bool isHorizontal = (hitPlane.alignment == PlaneAlignment.HorizontalUp ||
                                 hitPlane.alignment == PlaneAlignment.HorizontalDown);

            bool isVertical = (hitPlane.alignment == PlaneAlignment.Vertical);

            // Intentar instanciar según la regla del proyecto
            if (selectedModelIndex == 0 && isHorizontal)
            {
                Instantiate(horizontalModelPrefab, hitPose.position, hitPose.rotation);
                if (debugText != null) debugText.text = "Objeto colocado en plano horizontal";
            }
            else if (selectedModelIndex == 1 && isVertical)
            {
                Instantiate(verticalModelPrefab, hitPose.position, hitPose.rotation);
                if (debugText != null) debugText.text = "Objeto colocado en plano vertical";
            }
            else
            {
                if (debugText != null)
                    debugText.text = selectedModelIndex == 0 ?
                    "¡Este modelo solo va en planos HORIZONTALES!" :
                    "¡Este modelo solo va en planos VERTICALES!";
            }
        }
        else
        {
            if (debugText != null) debugText.text = "Sin superficie en esa zona";
        }
    }

    // Llama a esta función desde los botones de la UI
    public void SelectModel(int index)
    {
        selectedModelIndex = index;
    }
}