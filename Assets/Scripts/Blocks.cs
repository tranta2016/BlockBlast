using UnityEngine;

public class Blocks : MonoBehaviour
{
    [SerializeField] private Block[] blocks;
    [SerializeField] private Board board;
    [SerializeField] private GameManager gameManager;

    private int[] polyominosIndexes;
    private int blockCount = 0;

    private void Start()
    {
        var blockWidth = (float)Board.Size / blocks.Length;
        var cellSize = (float)Board.Size / (Block.Size * blocks.Length + blocks.Length + 1);

        for (var i = 0; i < blocks.Length; i++)
        {
            blocks[i].transform.localPosition = new Vector3(blockWidth * (i + 0.5f), -0.25f - cellSize * 4.0f, 0.0f);
            blocks[i].transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            blocks[i].Initialize();
        }

        polyominosIndexes = new int[blocks.Length];

        Generate();
    }

    private void Generate()
    {
        for (var i = 0; i < blocks.Length; i++)
        {
            polyominosIndexes[i] = Random.Range(0, Polyominos.Length);
            blocks[i].gameObject.SetActive(true);
            blocks[i].Show(polyominosIndexes[i]);
        }

        blockCount = blocks.Length;
    }

    public void Remove()
    {
        --blockCount;
        if (blockCount <= 0)
        {
            blockCount = 0;
            Generate();
        }

        var lose = true;
        for (var i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].gameObject.activeSelf && board.CheckPlace(polyominosIndexes[i]))
            {
                lose = false;
                break;
            }
        }

            if (lose)
        {
            gameManager.GameOver();
        }
    }

    public void ResetBlocksSortingOrders()
    {
        for (var i = 0; i < blocks.Length; i++)
        {
            blocks[i].SetSortingOrder(Block.RestSortingOrder);
        }
    }
}