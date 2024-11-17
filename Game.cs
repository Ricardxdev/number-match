namespace NumberMatchGame
{
    using System.ComponentModel.DataAnnotations;
    using Terminal.Gui;
    public class NumberMatchGame
    {
        private GameUI? GameUI;
        private GameController? GameController;
        public void Start()
        {
            Application.Init();

            this.GameUI = new GameUI();
            this.GameController = new GameController(GameUI, 5);

            
            Application.Top.Add(GameUI.GameWindow);
            Application.Run();
            Console.Out.Flush();
        }

        

        // private void CheckAndReplaceRows()
        // {
        //     for (int i = BoardSize + AddLineCount - 1; i >= 0; i--)
        //     {
        //         bool rowEmpty = true;
        //         for (int j = 0; j < BoardSize; j++)
        //         {
        //             if (NumberButtons[i, j] != null && NumberButtons[i, j].Visible)
        //             {
        //                 rowEmpty = false;
        //                 break;
        //             }
        //         }

        //         if (rowEmpty)
        //         {
        //             // Desplazar filas hacia arriba
        //             for (int k = i; k > 0; k--)
        //             {
        //                 for (int l = 0; l < BoardSize; l++)
        //                 {
        //                     if (k - 1 >= 0 && k < BoardSize + AddLineCount && NumberButtons[k - 1, l] != null)
        //                     {
        //                         NumberButtons[k, l] = NumberButtons[k - 1, l];
        //                         if (NumberButtons[k, l] != null)
        //                         {
        //                             NumberButtons[k, l].Y += 2;
        //                         }
        //                     }
        //                 }
        //             }

        //             // Limpiar la fila superior
        //             for (int l = 0; l < BoardSize; l++)
        //             {
        //                 NumberButtons[0, l] = null;
        //             }

        //             // Actualizar la interfaz de usuario
        //             GameBoard.RemoveAll();
        //             for (int m = 0; m < BoardSize + AddLineCount; m++)
        //             {
        //                 for (int n = 0; n < BoardSize; n++)
        //                 {
        //                     if (NumberButtons[m, n] != null)
        //                     {
        //                         GameBoard.Add(NumberButtons[m, n]);
        //                     }
        //                 }
        //             }

        //             GameBoard.SetNeedsDisplay();
        //             ResetBoardIfEmpty();
        //             CheckEndGame(); // Comprobar si el juego debe terminar
        //         }
        //     }

        //     ResetBoardIfEmpty();
        // }

        
    }
}