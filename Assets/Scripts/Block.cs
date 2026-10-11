using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class Block : MonoBehaviour
{
    public const int Size = 5;
    public const int RestSortingOrder = 10;
    public const int DragSortingOrder = 11;

    [SerializeField] private Board board;
    [SerializeField] private Blocks blocks;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Cell cellPrefab;

    private SortingGroup sortingGroup;
    private int polyominoIndex;

    private readonly Vector3 inputOffset = new(0.0f, 2.0f, 0.0f);
    private readonly Cell[,] cells = new Cell[Size, Size];

    private Vector3 position;
    private Vector3 scale;
    private Vector2 inputPoint;
    private Vector2 previousMousePosition = Vector2.positiveInfinity;

    private Vector2Int currentDragPoint;
    private Vector2Int previousDragPoint;

    private Camera mainCamera;
    private Vector2 center;
    private bool isDragging;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
        mainCamera = Camera.main;
    }

    public void Initialize()
    {
        for (var r = 0; r < Size; r++)
            for (var c = 0; c < Size; c++)
                cells[r, c] = Instantiate(cellPrefab, transform);

        position = transform.localPosition;
        scale = transform.localScale;

        SetSortingOrder(RestSortingOrder);
    }

    public void Show(int polyominoIndex)
    {
        Hide();

        transform.localPosition = position;
        transform.localScale = scale;
        SetSortingOrder(RestSortingOrder);

        this.polyominoIndex = polyominoIndex;

        var polyomino = Polyominos.Get(polyominoIndex);
        var polyominoRows = polyomino.GetLength(0);
        var polyominoCols = polyomino.GetLength(1);
        center = new Vector2(polyominoCols / 2.0f, polyominoRows / 2.0f);

        for (var r = 0; r < polyominoRows; r++)
        {
            for (var c = 0; c < polyominoCols; c++)
            {
                if (polyomino[r, c] == 0) continue;

                cells[r, c].transform.localPosition = new Vector3(
                    c - center.x + 0.5f,
                    r - center.y + 0.5f,
                    0.0f);
                cells[r, c].Normal();
            }
        }
    }

    private void Hide()
    {
        for (var r = 0; r < Size; r++)
            for (var c = 0; c < Size; c++)
                cells[r, c].Hide();
    }

    private Vector2 GetWorldMouse()
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        return mainCamera.ScreenToWorldPoint(screen);
    }

    private Vector2Int GetDragPoint()
    {
        return Vector2Int.RoundToInt((Vector2)transform.position - center);
    }

    private void OnMouseDown()
    {
        if (Mouse.current == null) return;
        if (gameManager != null && gameManager.IsGameOver) return;

        isDragging = true;

        inputPoint = GetWorldMouse();
        transform.localPosition = position + inputOffset;
        transform.localScale = Vector3.one;

        blocks.ResetBlocksSortingOrders();
        SetSortingOrder(DragSortingOrder);

        currentDragPoint = GetDragPoint();
        previousDragPoint = currentDragPoint;
        board.Hover(currentDragPoint, polyominoIndex);
        Highlight(board.HighlightpolyominoCols, board.HighlightpolyominoRows);

        previousMousePosition = Mouse.current.position.ReadValue();
    }

    private void OnMouseDrag()
    {
        if (!isDragging || Mouse.current == null) return;

        Vector2 currentMousePosition = Mouse.current.position.ReadValue();

        if (currentMousePosition != previousMousePosition)
        {
            previousMousePosition = currentMousePosition;

            var inputDelta = GetWorldMouse() - inputPoint;
            transform.localPosition = position + inputOffset + (Vector3)inputDelta * 1.4f;

            currentDragPoint = GetDragPoint();
            if (currentDragPoint != previousDragPoint)
            {
                previousDragPoint = currentDragPoint;
                board.Hover(currentDragPoint, polyominoIndex);
                Highlight(board.HighlightpolyominoCols, board.HighlightpolyominoRows);
            }
        }
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        previousMousePosition = Vector2.positiveInfinity;

        currentDragPoint = GetDragPoint();
        var placed = board.Place(currentDragPoint, polyominoIndex);

        transform.localPosition = position;
        transform.localScale = scale;
        SetSortingOrder(RestSortingOrder);

        if (placed)
        {
            // Tính điểm trước Remove(), vì Remove() có thể gọi GameOver
            if (gameManager != null)
            {
                gameManager.AddScore(board.LastPlacedCells, board.LastClearedLines);
            }

            gameObject.SetActive(false);
            blocks.Remove();
        }
        else
        {
            // Đặt thất bại: bỏ highlight trên khối ở khay
            var polyomino = Polyominos.Get(polyominoIndex);
            Unhighlight(polyomino.GetLength(0), polyomino.GetLength(1), polyomino);
        }
    }

    private void Highlight(List<int> cols, List<int> rows)
    {
        var polyomino = Polyominos.Get(polyominoIndex);
        var polyominoRows = polyomino.GetLength(0);
        var polyominoCols = polyomino.GetLength(1);

        Unhighlight(polyominoRows, polyominoCols, polyomino);

        HighlightFullCols(polyominoRows, polyomino, cols);
        HighlightFullRows(polyominoCols, polyomino, rows);
    }

    private void Unhighlight(int polyominoRows, int polyominoCols, int[,] polyomino)
    {
        for (var r = 0; r < polyominoRows; r++)
        {
            for (var c = 0; c < polyominoCols; c++)
            {
                if (polyomino[r, c] > 0)
                {
                    cells[r, c].Normal();
                }
            }
        }
    }

    private void HighlightFullRows(int polyominoCols, int[,] polyomino, List<int> rows)
    {
        foreach (var r in rows)
        {
            for (var c = 0; c < polyominoCols; c++)
            {
                if (polyomino[r, c] > 0)
                {
                    cells[r, c].Highlight();
                }
            }
        }
    }

    private void HighlightFullCols(int polyominoRows, int[,] polyomino, List<int> cols)
    {
        foreach (var c in cols)
        {
            for (var r = 0; r < polyominoRows; r++)
            {
                if (polyomino[r, c] > 0)
                {
                    cells[r, c].Highlight();
                }
            }
        }
    }

    public void SetSortingOrder(int sortingOrder)
    {
        sortingGroup.sortingOrder = sortingOrder;
    }
}