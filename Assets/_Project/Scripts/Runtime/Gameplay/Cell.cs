using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ////Figure selected = BattleManager.Instance.SelectedFigure;

        //if (selected != null)
        //{
        //    // Перевіряємо, чи ми клікнули не на ту саму клітинку, де фігура вже стоїть
        //    if (selected.transform.parent != transform)
        //    {
        //        // TODO: Тут перевірка правил шахів. Якщо хід дозволено:

        //        selected.MoveToCellAnimated(transform);

        //        // Скидаємо вибір після ходу
        //        BattleManager.Instance.SelectedFigure = null;
        //    }
        //}
    }

    // 2. Спрацьовує при ДРАГ-ЕНД-ДРОПІ (стара логіка, працює паралельно)
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Figure draggedFigure = eventData.pointerDrag.GetComponent<Figure>();

            if (draggedFigure != null)
            {
                draggedFigure.ReturnToCell(transform);
                BattleManager.Instance.SelectedFigure = null; // Про всяк випадок скидаємо
            }
        }
    }
}
