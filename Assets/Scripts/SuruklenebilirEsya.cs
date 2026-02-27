using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SuruklenebilirEsya : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform asilYuva;
    private Image resim;

    void Start()
    {
        resim = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        asilYuva = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        resim.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(asilYuva);

        // Eþyanýn yuvanýn tam merkezine mýknatýs gibi yapýþmasýný saðlayan kod:
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        resim.raycastTarget = true;
    }
}