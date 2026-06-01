using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Figure : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    Debug.Log("asdfasdf");
    //    // Запам'ятовуємо початкову клітинку на випадок, якщо дроп скасується
    //    originalParent = transform.parent;

    //    // Виносимо фігуру на найвищий рівень Canvas, щоб вона не ховалася під іншими UI елементами
    //    transform.SetParent(transform.root);

    //    // Вимикаємо blocksRaycasts, щоб клітинка під фігурою могла зорієнтуватись на OnDrop
    //    canvasGroup.blocksRaycasts = false;
    //    canvasGroup.alpha = 0.6f; // Трішки робимо прозорою для візуального ефекту драгу
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    // Переміщуємо фігуру слідом за пальцем/курсором з урахуванням масштабу Canvas
    //    //rectTransform.anchoredPosition += eventData.delta / eventData.lossyScale.x;
    //    rectTransform.anchoredPosition += eventData.delta / transform.lossyScale.x;
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    canvasGroup.blocksRaycasts = true;
    //    canvasGroup.alpha = 1f;

    //    // Якщо фігуру нікуди не прилаштували (transform.parent не змінився в Cell), повертаємо її назад
    //    if (transform.parent == transform.root)
    //    {
    //        ReturnToCell(originalParent);
    //    }
    //}

    //// Метод для красивого центрування фігури в клітинці
    //public void ReturnToCell(Transform cellParent)
    //{
    //    //transform.SetParent(cellParent);
    //    //rectTransform.anchoredPosition = Vector2.zero; // Центруємо в нуль всередині батька
    //    transform.SetParent(cellParent);

    //    // Задаємо якоря на повне розтягування (від 0 до 1 по обох осях)
    //    rectTransform.anchorMin = Vector2.zero; // (0, 0)
    //    rectTransform.anchorMax = Vector2.one;  // (1, 1)

    //    // Обнуляємо відступи, щоб фігура ідеально збігалася з розміром Cell
    //    rectTransform.offsetMin = Vector2.zero; // Left, Bottom = 0
    //    rectTransform.offsetMax = Vector2.zero; // Right, Top = 0

    //    // Якщо хочеш зробити фіксований відступ (наприклад, 10 пікселів з усіх боків), заміни нулі на це:
    //    // rectTransform.offsetMin = new Vector2(10, 10);
    //    // rectTransform.offsetMax = new Vector2(-10, -10);

    //    // Скидаємо локальну позицію та масштаб про всяк випадок
    //    rectTransform.anchoredPosition = Vector2.zero;
    //    rectTransform.localScale = Vector3.one;
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    Debug.Log("clicked");
    //}

    // ЛОГІКА КЛІКУ
    public void OnPointerClick(PointerEventData eventData)
    {
        // Якщо ми її зараз драгаємо, клік не зараховуємо
        if (eventData.dragging) return;

        // Запам'ятовуємо, що вибрали саме цю фігуру
        ChessTurnManager.Instance.SelectedFigure = this;

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
