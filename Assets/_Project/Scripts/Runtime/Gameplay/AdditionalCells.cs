using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Game.Runtime.Gameplay.Board
{
    [ExecuteAlways]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class AdditionalCells : UIBehaviour
    {
        [SerializeField] private int columns = 8;
        [SerializeField] private int rows = 3; // Додаємо кількість рядків (3 для 24 клітинок)

        [SerializeField] private ChessCell[] cells;
        [SerializeField] private string color;

        private GridLayoutGroup grid;
        private RectTransform rectTransform;

        public ChessCell GetFree()
        {
            foreach (ChessCell cell in cells)
            {
                if(cell.transform.childCount == 0)
                {
                    return cell;
                }
            }
            return null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCellSize();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateCellSize();
        }

        private void UpdateCellSize()
        {
            if (grid == null) grid = GetComponent<GridLayoutGroup>();
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

            if (grid != null && rectTransform != null && rectTransform.rect.width > 0 && rectTransform.rect.height > 0)
            {
                // 1. Рахуємо розмір клітинки, якщо відштовхуватись ТІЛЬКИ від ширини
                float totalSpacingX = grid.spacing.x * (columns - 1);
                float totalPaddingX = grid.padding.left + grid.padding.right;
                float sizeBasedOnWidth = (rectTransform.rect.width - totalSpacingX - totalPaddingX) / columns;

                // 2. Рахуємо розмір клітинки, якщо відштовхуватись ТІЛЬКИ від висоти контейнера
                float totalSpacingY = grid.spacing.y * (rows - 1);
                float totalPaddingY = grid.padding.top + grid.padding.bottom;
                float sizeBasedOnHeight = (rectTransform.rect.height - totalSpacingY - totalPaddingY) / rows;

                // 3. Беремо НАЙМЕНШИЙ з двох розмірів. Це гарантує, що квадрати влізуть і по ширині, і по висоті!
                float finalSize = Mathf.Min(sizeBasedOnWidth, sizeBasedOnHeight);

                grid.cellSize = new Vector2(finalSize, finalSize);
            }
        }


    }
}