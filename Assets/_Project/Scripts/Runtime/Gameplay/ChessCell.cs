using Game.Core.Sounds;
using Game.Runtime.Gameplay.Board;
using System.Reflection;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChessCell : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public string position;

    [SerializeField] private AdditionalCells whiteCells;
    [SerializeField] private AdditionalCells blackCells;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChessFigure selected = BattleManager.Instance.SelectedFigure;
        string sound = "move";
        if (selected != null)
        {
            // Перевіряємо, чи ми клікнули не на ту саму клітинку, де фігура вже стоїть
            if (selected.transform.parent != transform)
            {
                // TODO: Тут перевірка правил шахів. Якщо хід дозволено:
                if (transform.childCount > 0)
                {
                    sound = "take";
                    if (selected.color == "white")
                    {

                        ChessFigure inFigure = transform.GetChild(0).GetComponent<ChessFigure>();
                        ChessCell freeCell = whiteCells.GetFree();
                        BattleManager.Instance.MeMove(inFigure, freeCell, sound);
                        inFigure.MoveToCellAnimated(freeCell.transform);
                    }
                    else if (selected.color == "black")
                    {
                        ChessFigure inFigure = transform.GetChild(0).GetComponent<ChessFigure>();
                        ChessCell freeCell = blackCells.GetFree();
                        BattleManager.Instance.MeMove(inFigure, freeCell, sound);
                        inFigure.MoveToCellAnimated(freeCell.transform);
                    }
                }

                selected.MoveToCellAnimated(transform);
                BattleManager.Instance.MeMove(selected, this, sound);
                SoundManager.Instance.Move();

                // Скидаємо вибір після ходу
                BattleManager.Instance.SelectedFigure = null;
            }
        }
    }

    // 2. Спрацьовує при ДРАГ-ЕНД-ДРОПІ (стара логіка, працює паралельно)
    public void OnDrop(PointerEventData eventData)
    {
        string sound = "move";
        if (eventData.pointerDrag != null)
        {
            ChessFigure draggedFigure = eventData.pointerDrag.GetComponent<ChessFigure>();

            if (draggedFigure != null)
            {
                bool sounded = false;
                if (transform.childCount > 0)
                {
                    sound = "take";
                    ChessFigure inFigure = transform.GetChild(0).GetComponent<ChessFigure>();

                    ChessCell freeCell = null;

                    if (inFigure.color == "white")
                    {
                        freeCell = whiteCells.GetFree();
                    }
                    else if (inFigure.color == "black")
                    {
                        freeCell = blackCells.GetFree();
                    }

                    BattleManager.Instance.MeMove(inFigure, freeCell, string.Empty);
                    inFigure.MoveToCellAnimated(freeCell.transform);
                    SoundManager.Instance.Take();
                    sounded = true;
                }

                draggedFigure.ReturnToCell(transform);
                if (!sounded)
                {
                    SoundManager.Instance.Move();
                }
                BattleManager.Instance.SelectedFigure = null; // Про всяк випадок скидаємо
                BattleManager.Instance.MeMove(draggedFigure, this, sound);
                // якшо тут була фігура то вернути в additionalCell
            }
        }
    }

}
