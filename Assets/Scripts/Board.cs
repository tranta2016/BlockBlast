using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int Size = 8;

    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Transform cellsTransform;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesTransform;

    private readonly Cell[,] cells = new Cell[Size, Size];
    private readonly int[,] data = new int[Size, Size]; // 0 = empty, 1 = hover, 2 = filled

    public int LastPlacedCells { get; private set; }
    public int LastClearedLines { get; private set; }
    private readonly List<Vector2Int> hoverPoints = new();
    private readonly List<int> highlightpolyominoCols = new();
    private readonly List<int> highlightpolyominoRows = new();
    private readonly List<int> fullCols = new();
    private readonly List<int> fullRows = new();

    public List<int> HighlightpolyominoCols => highlightpolyominoCols;
    public List<int> HighlightpolyominoRows => highlightpolyominoRows;

    private void Start()
    {
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                var position = new Vector3(c + 0.5f, r + 0.5f, 0.0f);

                if (tilePrefab != null)
                {
                    var tile = Instantiate(tilePrefab, tilesTransform);
                    tile.transform.position = position;
                }

                cells[r, c] = Instantiate(cellPrefab, cellsTransform);
                cells[r, c].transform.position = position;
                cells[r, c].Hide();
            }
        }
    }

    public void Hover(Vector2Int point, int polyominoIndex)
    {
        var polyomino = Polyominos.Get(polyominoIndex);
        var polyominoRows = polyomino.GetLength(0);
        var polyominoCols = polyomino.GetLength(1);

        Unhover();
        Unhighlight();

        highlightpolyominoCols.Clear();
        highlightpolyominoRows.Clear();

        CollectHoverPoints(point, polyominoRows, polyominoCols, polyomino);

        if (hoverPoints.Count > 0)
        {
            Hover();
            Highlight(point, polyominoCols, polyominoRows);

            foreach (var fullCol in fullCols)
            {
                highlightpolyominoCols.Add(fullCol - point.x);
            }

            foreach (var fullRow in fullRows)
            {
                highlightpolyominoRows.Add(fullRow - point.y);
            }
        }
    }

    private void Highlight(Vector2Int point, int polyominoCols, int polyominoRows)
    {
        PredictFullLineCols(point.x, point.x + polyominoCols);
        PredictFullLineRows(point.y, point.y + polyominoRows);

        HighlightFullCols();
        HighlightFullRows();
    }

    private void Unhighlight()
    {
        UnHighlightFullCols();
        UnHighlightFullRows();
        fullCols.Clear();
        fullRows.Clear();
    }

    private void CollectHoverPoints(Vector2Int point, int polyominoRows, int polyominoCols, int[,] polyomino)
    {
        for (var r = 0; r < polyominoRows; r++)
        {
            for (var c = 0; c < polyominoCols; c++)
            {
                if (polyomino[r, c] == 0) continue;

                var hoverPoint = point + new Vector2Int(c, r);
                if (!IsValidPoint(hoverPoint))
                {
                    hoverPoints.Clear();
                    return;
                }
                hoverPoints.Add(hoverPoint);
            }
        }
    }

    private bool IsValidPoint(Vector2Int point)
    {
        if (point.x < 0 || Size <= point.x) return false;
        if (point.y < 0 || Size <= point.y) return false;
        if (data[point.y, point.x] > 0) return false;
        return true;
    }

    private void Hover()
    {
        foreach (var hoverPoint in hoverPoints)
        {
            data[hoverPoint.y, hoverPoint.x] = 1;
            cells[hoverPoint.y, hoverPoint.x].Hover();
        }
    }

    private void Unhover()
    {
        foreach (var hoverPoint in hoverPoints)
        {
            data[hoverPoint.y, hoverPoint.x] = 0;
            cells[hoverPoint.y, hoverPoint.x].Hide();
        }
        hoverPoints.Clear();
    }

    public bool Place(Vector2Int point, int polyominoIndex)
    {
        var polyomino = Polyominos.Get(polyominoIndex);
        var polyominoRows = polyomino.GetLength(0);
        var polyominoCols = polyomino.GetLength(1);

        Unhover();
        Unhighlight();

        CollectHoverPoints(point, polyominoRows, polyominoCols, polyomino);
        if (hoverPoints.Count > 0)
        {
            Place(point, polyominoCols, polyominoRows);
            return true;
        }
        return false;
    }

    private void Place(Vector2Int point, int polyominoCols, int polyominoRows)
    {
        LastPlacedCells = hoverPoints.Count;

        foreach (var hoverPoint in hoverPoints)
        {
            data[hoverPoint.y, hoverPoint.x] = 2;
            cells[hoverPoint.y, hoverPoint.x].Normal();
        }

        hoverPoints.Clear();

        ClearFullLines(point, polyominoCols, polyominoRows);
    }

    private void ClearFullLines(Vector2Int point, int polyominoCols, int polyominoRows)
    {
        FullLineCols(point.x, point.x + polyominoCols);
        FullLineRows(point.y, point.y + polyominoRows);

        LastClearedLines = fullCols.Count + fullRows.Count;

        ClearFullCols();
        ClearFullRows();

        fullCols.Clear();
        fullRows.Clear();
    }

    private void FullLineCols(int startCol, int endCol)
    {
        fullCols.Clear();
        for (var c = startCol; c < endCol; c++)
        {
            var isFull = true;
            for (var r = 0; r < Size; r++)
            {
                if (data[r, c] != 2)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull) fullCols.Add(c);
        }
    }

    private void FullLineRows(int startRow, int endRow)
    {
        fullRows.Clear();
        for (var r = startRow; r < endRow; r++)
        {
            var isFull = true;
            for (var c = 0; c < Size; c++)
            {
                if (data[r, c] != 2)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull) fullRows.Add(r);
        }
    }

    private void ClearFullCols()
    {
        foreach (var c in fullCols)
        {
            for (var r = 0; r < Size; r++)
            {
                data[r, c] = 0;
                cells[r, c].Hide();
            }
        }
    }

    private void ClearFullRows()
    {
        foreach (var r in fullRows)
        {
            for (var c = 0; c < Size; c++)
            {
                data[r, c] = 0;
                cells[r, c].Hide();
            }
        }
    }

    private void PredictFullLineCols(int startCol, int endCol)
    {
        fullCols.Clear();
        for (var c = startCol; c < endCol; c++)
        {
            var isFull = true;
            for (var r = 0; r < Size; r++)
            {
                if (data[r, c] != 1 && data[r, c] != 2)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull) fullCols.Add(c);
        }
    }

    private void PredictFullLineRows(int startRow, int endRow)
    {
        fullRows.Clear();
        for (var r = startRow; r < endRow; r++)
        {
            var isFull = true;
            for (var c = 0; c < Size; c++)
            {
                if (data[r, c] != 1 && data[r, c] != 2)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull) fullRows.Add(r);
        }
    }

    private void HighlightFullCols()
    {
        foreach (var c in fullCols)
            for (var r = 0; r < Size; r++)
                if (data[r, c] == 2)
                    cells[r, c].Highlight();
    }

    private void HighlightFullRows()
    {
        foreach (var r in fullRows)
            for (var c = 0; c < Size; c++)
                if (data[r, c] == 2)
                    cells[r, c].Highlight();
    }

    private void UnHighlightFullCols()
    {
        foreach (var c in fullCols)
            for (var r = 0; r < Size; r++)
                if (data[r, c] == 2)
                    cells[r, c].Normal();
    }

    private void UnHighlightFullRows()
    {
        foreach (var r in fullRows)
            for (var c = 0; c < Size; c++)
                if (data[r, c] == 2)
                    cells[r, c].Normal();
    }

    public bool CheckPlace(int polyominoIndex)
    {
        var polyomino = Polyominos.Get(polyominoIndex);
        var polyominoRows = polyomino.GetLength(0);
        var polyominoCols = polyomino.GetLength(1);

        for (var r = 0; r <= Size - polyominoRows; r++)
        {
            for (var c = 0; c <= Size - polyominoCols; c++)
            {
                if (CheckPlace(c, r, polyominoCols, polyominoRows, polyomino))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckPlace(int col, int row, int polyominoCols, int polyominoRows, int[,] polyomino)
    {
        for (var r = 0; r < polyominoRows; r++)
        {
            for (var c = 0; c < polyominoCols; c++)
            {
                if (polyomino[r, c] > 0 && data[row + r, col + c] > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
}