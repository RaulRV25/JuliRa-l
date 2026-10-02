using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

[RequireComponent(typeof(ARPlane))]
public class ARPlaneVisualizerCustom : MonoBehaviour
{
    private ARPlane arPlane;

    [Header("Componentes Visuales")]
    [SerializeField] private MeshRenderer planeMeshRenderer;
    [SerializeField] private TextMeshPro textDimensions;

    [Header("Materiales por Tipo de Plano")]
    [SerializeField] private Material horizontalMaterial; 
    [SerializeField] private Material verticalMaterial;   

    private void Awake()
    {
        arPlane = GetComponent<ARPlane>();
    }

    private void OnEnable()
    {
        arPlane.boundaryChanged += OnBoundaryChanged;
        UpdatePlaneAppearance();
    }

    private void OnDisable()
    {
        arPlane.boundaryChanged -= OnBoundaryChanged;
    }

    private void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs args)
    {
        UpdatePlaneAppearance();
    }

    private void UpdatePlaneAppearance()
    {
        // 1. Asignar Material/Color según alineación
        if (arPlane.alignment == PlaneAlignment.HorizontalUp || arPlane.alignment == PlaneAlignment.HorizontalDown)
        {
            if (planeMeshRenderer != null && horizontalMaterial != null)
                planeMeshRenderer.material = horizontalMaterial;
        }
        else if (arPlane.alignment == PlaneAlignment.Vertical)
        {
            if (planeMeshRenderer != null && verticalMaterial != null)
                planeMeshRenderer.material = verticalMaterial;
        }

        // 2. Calcular dimensiones en metros (Ancho x Alto) y actualizar la etiqueta
        Vector2 size = arPlane.size;
        if (textDimensions != null)
        {
            textDimensions.text = $"{size.x:F2}m x {size.y:F2}m";

            // Posicionar la etiqueta en el centro del plano
            textDimensions.transform.position = arPlane.center;

            // Orientar el texto mirando hacia la dirección normal del plano
            textDimensions.transform.rotation = Quaternion.LookRotation(-arPlane.normal, Vector3.up);
        }
    }
}