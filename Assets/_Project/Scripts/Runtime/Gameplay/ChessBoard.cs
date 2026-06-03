using System;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [SerializeField] private ChessCell[] cells;

    [SerializeField] private ChessCell[] myCells;
    [SerializeField] private ChessCell[] enemyCells;

    [SerializeField] private ChessFigure whitePawn;
    [SerializeField] private ChessFigure whiteRook;
    [SerializeField] private ChessFigure whiteKnight;
    [SerializeField] private ChessFigure whiteBishop;
    [SerializeField] private ChessFigure whiteQueen;
    [SerializeField] private ChessFigure whiteKing;

    [SerializeField] private ChessFigure blackPawn;
    [SerializeField] private ChessFigure blackRook;
    [SerializeField] private ChessFigure blackKnight;
    [SerializeField] private ChessFigure blackBishop;
    [SerializeField] private ChessFigure blackQueen;
    [SerializeField] private ChessFigure blackKing;

    [SerializeField] private Transform boardRoot;

    



    public static ChessBoard Instance { get; private set; }

    private Dictionary<string, ChessCell> cellDictionary;
    private Dictionary<string, ChessFigure> figureDictionary;

    private bool isWhiteBottom;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Init()
    {
        cellDictionary = new Dictionary<string, ChessCell>();
        figureDictionary = new Dictionary<string, ChessFigure>();

        foreach (var cell in cells)
        {
            cellDictionary[cell.position] = cell;
        }
    }

    // 🔥 переворот координати дошки
    private string FlipPosition(string pos)
    {
        char file = pos[0]; // a-h
        char rank = pos[1]; // 1-8

        int fileIndex = file - 'a';
        int rankIndex = rank - '1';

        int flippedFile = 7 - fileIndex;
        int flippedRank = 7 - rankIndex;

        char newFile = (char)('a' + flippedFile);
        char newRank = (char)('1' + flippedRank);

        return $"{newFile}{newRank}";
    }

    // 🔥 отримання клітинки з урахуванням сторони
    private ChessCell GetCell(string file, int rank)
    {
        string pos = file + rank;

        if (!isWhiteBottom)
            pos = FlipPosition(pos);

        return cellDictionary[pos];
    }

    public void ResetBoard(int myTeam)
    {
        isWhiteBottom = myTeam == 0;
        figureDictionary.Clear();

        // 🔥 переворот дошки
        if (isWhiteBottom)
            boardRoot.localRotation = Quaternion.Euler(0, 0, 0);
        else
            boardRoot.localRotation = Quaternion.Euler(0, 0, 180);

        // очистка
        foreach (var cell in cells)
        {
            for (int i = cell.transform.childCount - 1; i >= 0; i--)
                Destroy(cell.transform.GetChild(i).gameObject);
        }

        string[] files = { "a", "b", "c", "d", "e", "f", "g", "h" };

        int whiteBackRank = 1;
        int whitePawnRank = 2;

        int blackBackRank = 8;
        int blackPawnRank = 7;

        for (int i = 0; i < 8; i++)
        {
            ChessFigure figure = Spawn(whitePawn, cellDictionary[files[i] + whitePawnRank].transform);
            figure.figureName = $"{figure.name}{i}" ;
            figureDictionary[figure.figureName] = figure;
            figure.color = "white";

            figure = Spawn(blackPawn, cellDictionary[files[i] + blackPawnRank].transform);
            figure.figureName = $"{figure.name}{i}" ;
            figureDictionary[figure.figureName] = figure;
            figure.color = "black";
        }

        ChessFigure[] whiteBack =
        {
        whiteRook, whiteKnight, whiteBishop, whiteQueen,
        whiteKing,
        whiteBishop, whiteKnight, whiteRook
    };

        ChessFigure[] blackBack =
        {
        blackRook, blackKnight, blackBishop, blackQueen,
        blackKing,
        blackBishop, blackKnight, blackRook
    };

        for (int i = 0; i < 8; i++)
        {
            ChessFigure figure = Spawn(whiteBack[i], cellDictionary[files[i] + whiteBackRank].transform);
            figure.figureName = $"{figure.name}{i}";
            figureDictionary[figure.figureName] = figure;
            figure.color = "white";

            figure = Spawn(blackBack[i], cellDictionary[files[i] + blackBackRank].transform);
            figure.figureName = $"{figure.name}{i}";
            figureDictionary[figure.figureName] = figure;
            figure.color = "black";

        }

        for (int i = 2; i < 10; i++)
        {
            ChessFigure figure = Spawn(blackQueen, myCells[i].transform);
            figure.figureName = $"{figure.name}{i}{i}";
            figureDictionary[figure.figureName] = figure;
            figure.color = "black";

            //figure.transform.localPosition = Vector3.zero;
            //figure.transform.localScale = Vector3.one;
            //figure.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            //figure.GetComponent<RectTransform>().anchorMax = Vector2.one;
            //figure.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            //figure.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            //figure.GetComponent<RectTransform>().localScale = Vector3.one;

            figure = Spawn(whiteQueen, enemyCells[i].transform);
            figure.figureName = $"{figure.name}{i}{i}";
            figureDictionary[figure.figureName] = figure;
            figure.color = "white";

            //figure.transform.localPosition = Vector3.zero;
            //figure.transform.localScale = Vector3.one;

            //figure.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            //figure.GetComponent<RectTransform>().anchorMax = Vector2.one;
            //figure.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            //figure.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            //figure.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    private ChessFigure Spawn(ChessFigure prefab, Transform cell)
    {
        ChessFigure figure = Instantiate(prefab, cell);
        figure.transform.localPosition = Vector3.zero;
        figure.transform.localScale = Vector3.one;
        // 🔥 якщо чорні - перевертаємо фігуру
        if (!isWhiteBottom)
        {
            figure.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
        figure.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        figure.GetComponent<RectTransform>().anchorMax = Vector2.one;
        figure.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        figure.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        figure.GetComponent<RectTransform>().localScale = Vector3.one;
        return figure;
        
    }

    public void MoveEnemyFigure(string figureId, string cellId)
    {
        if (!figureDictionary.TryGetValue(figureId, out ChessFigure figure))
        {
            //Debug.Log()
            return;
        }

        if (!cellDictionary.TryGetValue(cellId, out ChessCell cell))
        {
            return;
        }

        figure.MoveToCellAnimated(cell.transform);

    }
}