using UnityEngine;

public class Polyominos : MonoBehaviour
{
    private static readonly int[][,] polyominos = new int[][,]
    {
        // 0: chữ L lớn 3x3 (hình gốc của bạn)
        new int[,] { {0,0,1}, {0,0,1}, {1,1,1} },

        // 1-4: chữ L lớn 3x3 (các hướng còn lại)
        new int[,] { {1,0,0}, {1,0,0}, {1,1,1} },
        new int[,] { {1,1,1}, {0,0,1}, {0,0,1} },
        new int[,] { {1,1,1}, {1,0,0}, {1,0,0} },

        // 4-7: ô đơn, thanh 2
        new int[,] { {1} },
        new int[,] { {1,1} },
        new int[,] { {1}, {1} },

        // thanh 3
        new int[,] { {1,1,1} },
        new int[,] { {1}, {1}, {1} },

        // thanh 4
        new int[,] { {1,1,1,1} },
        new int[,] { {1}, {1}, {1}, {1} },

        // thanh 5
        new int[,] { {1,1,1,1,1} },
        new int[,] { {1}, {1}, {1}, {1}, {1} },

        // vuông 2x2, 3x3
        new int[,] { {1,1}, {1,1} },
        new int[,] { {1,1,1}, {1,1,1}, {1,1,1} },

        // chữ nhật 2x3 và 3x2
        new int[,] { {1,1,1}, {1,1,1} },
        new int[,] { {1,1}, {1,1}, {1,1} },

        // chữ L nhỏ (3 ô), 4 hướng
        new int[,] { {1,0}, {1,1} },
        new int[,] { {0,1}, {1,1} },
        new int[,] { {1,1}, {1,0} },
        new int[,] { {1,1}, {0,1} },

        // chữ L (4 ô), dọc
        new int[,] { {1,0}, {1,0}, {1,1} },
        new int[,] { {0,1}, {0,1}, {1,1} },
        new int[,] { {1,1}, {1,0}, {1,0} },
        new int[,] { {1,1}, {0,1}, {0,1} },

        // chữ L (4 ô), ngang
        new int[,] { {1,1,1}, {1,0,0} },
        new int[,] { {1,1,1}, {0,0,1} },
        new int[,] { {1,0,0}, {1,1,1} },
        new int[,] { {0,0,1}, {1,1,1} },

        // chữ T, 4 hướng
        new int[,] { {1,1,1}, {0,1,0} },
        new int[,] { {0,1,0}, {1,1,1} },
        new int[,] { {1,0}, {1,1}, {1,0} },
        new int[,] { {0,1}, {1,1}, {0,1} },

        // chữ S và Z
        new int[,] { {0,1,1}, {1,1,0} },
        new int[,] { {1,1,0}, {0,1,1} },
        new int[,] { {1,0}, {1,1}, {0,1} },
        new int[,] { {0,1}, {1,1}, {1,0} },
    };

    static Polyominos()
    {
        foreach (var polyomino in polyominos)
        {
            ReverseRows(polyomino);
        }
    }

    public static int[,] Get(int index) => polyominos[index];
    public static int Length => polyominos.Length;

    private static void ReverseRows(int[,] polyomino)
    {
        var rows = polyomino.GetLength(0);
        var cols = polyomino.GetLength(1);
        for (var r = 0; r < rows / 2; r++)
        {
            var topRow = r;
            var bottomRow = rows - 1 - r;
            for (var c = 0; c < cols; c++)
            {
                (polyomino[bottomRow, c], polyomino[topRow, c]) = (polyomino[topRow, c], polyomino[bottomRow, c]);
            }
        }
    }
}