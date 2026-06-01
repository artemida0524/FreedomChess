using UnityEngine;

public class ChessTurnManager : MonoBehaviour
{
    public static ChessTurnManager Instance { get; private set; }

    public Figure SelectedFigure { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}