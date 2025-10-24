using UnityEngine;

public class DiceSlotsPanel : MonoBehaviour
{
    [Header("References")]
    public GameObject diceFaceUIPrefab;     // Prefab del DiceFaceUI (la cara del dado con info)
    public RectTransform[] slotTransforms;  // Referencias a los 3 objetos "DiceSlot1/2/3"

    private GameObject[] instantiatedFaces;

    private void Awake()
    {
        instantiatedFaces = new GameObject[slotTransforms.Length];
    }

    /// <summary>
    /// Coloca un DiceFace en un slot (instancia o reemplaza el contenido anterior).
    /// </summary>
    public void SetSlot(int index, DiceFace face)
    {
        if (index < 0 || index >= slotTransforms.Length || diceFaceUIPrefab == null) return;

        // Eliminar el prefab anterior si ya había uno en ese slot
        if (instantiatedFaces[index] != null)
        {
            Destroy(instantiatedFaces[index]);
            instantiatedFaces[index] = null;
        }

        // Instanciar un nuevo DiceFaceUI en el slot correspondiente
        GameObject go = Instantiate(diceFaceUIPrefab, slotTransforms[index]);
        instantiatedFaces[index] = go;

        // Configurar la cara del dado
        var dfui = go.GetComponent<DiceFaceUI>();
        if (dfui != null)
            dfui.SetFace(face);
    }

    /// <summary>
    /// Limpia todos los slots (por ejemplo, al iniciar un nuevo turno).
    /// </summary>
    public void ClearAll()
    {
        for (int i = 0; i < instantiatedFaces.Length; i++)
        {
            if (instantiatedFaces[i] != null)
            {
                Destroy(instantiatedFaces[i]);
                instantiatedFaces[i] = null;
            }
        }
    }
}
