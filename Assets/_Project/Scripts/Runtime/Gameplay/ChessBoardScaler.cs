using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Game.Runtime.Gameplay.Board
{

    [ExecuteAlways]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class ChessBoardScaler : UIBehaviour
    {
        private GridLayoutGroup grid;
        private RectTransform rectTransform;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCellSize();
        }

        // Цей метод автоматично викликається Unity щоразу, коли змінюється розмір об'єкта
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateCellSize();
        }

        private void UpdateCellSize()
        {
            if (grid == null) grid = GetComponent<GridLayoutGroup>();
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

            if (grid != null && rectTransform != null && rectTransform.rect.width > 0)
            {
                // Ділимо поточну ширину дошки на 8, щоб отримати ідеальний розмір клітинки
                float size = rectTransform.rect.width / 8f;
                grid.cellSize = new Vector2(size, size);
            }
        }
    }
}