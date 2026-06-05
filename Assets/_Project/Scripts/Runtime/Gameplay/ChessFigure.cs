using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;



public class ChessFigure : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    public string figureName;
    public string color;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    // ЛОГІКА КЛІКУ
    public void OnPointerClick(PointerEventData eventData)
    {
        // Якщо ми її зараз драгаємо, клік не зараховуємо
        if (eventData.dragging) return;

        // Запам'ятовуємо, що вибрали саме цю фігуру
        BattleManager.Instance.SelectedFigure = this;

        // TODO: Тут можна додати візуальний ефект виділення (наприклад, підсвітити фігуру через DOTween)
        rectTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.2f);
    }

    // ЛОГІКА ПЕРЕСУВАННЯ ЧЕРЕЗ DOTWEEN
    public void MoveToCellAnimated(Transform newCellParent)
    {
        // Виносимо на root Canvas на час анімації, щоб фігура була поверх усіх клітинок
        transform.SetParent(transform.root);

        // Анімуємо СВІТОВУ позицію до світової позиції нової клітинки
        rectTransform.DOMove(newCellParent.position, 0.3f) // 0.3 секунди на політ
            .SetEase(Ease.OutQuad) // Плавне затухання в кінці
            .OnComplete(() =>
            {
                // Коли прилетіли — фіксуємо фігуру в новій клітинці
                ReturnToCell(newCellParent);
            });
    }

    public void ReturnToCell(Transform cellParent)
    {
        transform.SetParent(cellParent);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

    // Стара логіка драгу (залишається без змін, але адаптована під нову структуру)
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.lossyScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (transform.parent == transform.root)
        {
            ReturnToCell(originalParent);
        }
    }

}