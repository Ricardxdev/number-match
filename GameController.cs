using Terminal.Gui;

namespace NumberMatchGame
{
    public class GameController
    {
        private int Score = 0, Level = 1;
        public bool Distant = false;
        private int BoardSize;
        // Create a list with initial capacity of 10
        private List<NumberButton[]>? Numbers;
        //private NumberButton[,]? Numbers;
        private NumberButton? SelectedButton;
        private int AddLineCount = 0;
        private const int MaxAddLines = 1000000;
        private int HintCount = 0;
        private const int MaxHints = 10000000;
        int CurrentPhase = 1;

        public GameController(GameUI GameUI, int BoardSize)
        {
            this.BoardSize = BoardSize;
            GameUI.AddLineButton.Clicked += () => AddNewLine(GameUI);
            GameUI.HintButton.Clicked += () => ProvideHint(GameUI);

            InitializeBoard(GameUI);
        }
        private void InitializeBoard(GameUI GameUI)
        {
            // Initialize List of arrays
            Numbers = new List<NumberButton[]>(BoardSize);

            // Pre-initialize arrays for each row
            for (int i = 0; i < BoardSize; i++)
            {
                Numbers.Add(new NumberButton[BoardSize]);
            }

            var random = new Random();

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < Numbers.Count; j++)
                {
                    var number = new Number(j, i, random.Next(1, 10));
                    var btn = new NumberButton(number);

                    btn.Clicked += () => HandleButtonClick(GameUI, btn);
                    Numbers[i][j] = btn;
                    GameUI.Add(btn);
                }
            }
        }

        private void HandleButtonClick(GameUI GameUI, NumberButton clickedButton)
        {
            if (clickedButton == null) return;
            if (!clickedButton.Number.IsSelected)
            {
                clickedButton.Number.IsSelected = true;
                GameUI.SetColorSelected(clickedButton);
            }
            else
            {
                clickedButton.Number.IsSelected = false;
                GameUI.ResetColor(clickedButton);
                SelectedButton = null;
            }

            if (SelectedButton == null)
            {
                if (clickedButton.Number.IsSelected)
                {
                    SelectedButton = clickedButton;
                }
            }
            else
            {
                Match(GameUI, clickedButton);
            }
        }

        private void Match(GameUI GameUI, NumberButton clickedButton)
        {
            // Verificar si los números suman 10 o son iguales
            int num1 = SelectedButton.Number.Value;
            int num2 = clickedButton.Number.Value;

            if ((num1 + num2 == 10) || (num1 == num2))
            {
                if (BFS(SelectedButton, clickedButton))
                {
                    // Remover los botones y actualizar el puntaje
                    Hide(SelectedButton, clickedButton);
                    Scoring(GameUI, clickedButton);

                    // Verificar y reemplazar filas si es necesario
                    ///////////////////////////////////////////////////////////////////CheckAndReplaceRows();
                }
            }

            GameUI.ResetColor(SelectedButton, clickedButton);
            UnSelect(SelectedButton, clickedButton);
            SelectedButton = null;
        }

        private bool BFS(NumberButton num_a, NumberButton num_b)
        {
            int[,] directions = new int[,] {
                {  0, -1}, // Arriba
                {  1, -1}, // 45°
                {  1,  0}, // Derecha
                {  1,  1}, // 135°
                {  0,  1}, // Abajo
                { -1,  1}, // 225°
                { -1,  0}, // Izquierda
                { -1, -1}  // 315°
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                if (CheckDirection(num_a, num_b, GetRow(directions, i)))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckDirection(NumberButton num_a, NumberButton num_b, int[] direction)
        {
            int x = num_a.Number.X;
            int y = num_a.Number.Y;
            int columnsCount = BoardSize;
            int rowsCount = Numbers.Count;

            while (true)
            {
                x += direction[0];
                y += direction[1];

                if (!(x >= 0 && x < columnsCount))
                {
                    if (direction[1] != 0) break;

                    if (x < 0)
                    {
                        x = columnsCount - 1;
                        y -= 1;
                    }

                    if (x >= columnsCount)
                    {
                        x = 0;
                        y += 1;
                    }
                }

                if (!(y >= 0 && y < rowsCount)) break;

                var act = Numbers[y][x];
                if (!act.Visible)
                {
                    Distant = true;
                    continue;
                };

                if ((act.Number.X != num_b.Number.X) || (act.Number.Y != num_b.Number.Y))
                {
                    Distant = false;
                    break;
                };

                return true;
            }

            return false;
        }

        private NumberButton? RetrieveCombination(NumberButton num)
        {
            int[,] directions = new int[,] {
                {  0, -1}, // Arriba
                {  1, -1}, // 45°
                {  1,  0}, // Derecha
                {  1,  1}, // 135°
                {  0,  1}, // Abajo
                { -1,  1}, // 225°
                { -1,  0}, // Izquierda
                { -1, -1}  // 315°
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                var pair = CheckDirectionForCombination(num, GetRow(directions, i));
                if (pair != null)
                {
                    return pair;
                }
            }

            return null;
        }

        private NumberButton? CheckDirectionForCombination(NumberButton num_a, int[] direction)
        {
            int x = num_a.Number.X;
            int y = num_a.Number.Y;
            int columnsCount = BoardSize;
            int rowsCount = Numbers.Count;

            while (true)
            {
                x += direction[0];
                y += direction[1];

                if (!(x >= 0 && x < columnsCount))
                {
                    if (direction[1] != 0) break;

                    if (x < 0)
                    {
                        x = columnsCount - 1;
                        y -= 1;
                    }

                    if (x >= columnsCount)
                    {
                        x = 0;
                        y += 1;
                    }
                }

                if (!(y >= 0 && y < rowsCount)) break;

                var act = Numbers[y][x];
                if (act == null || !act.Visible) continue;

                if ((num_a.Number.Value == act.Number.Value) || ((num_a.Number.Value + act.Number.Value) == 10)) return act;
                else break;
            }

            return null;
        }

        void Scoring(GameUI GameUI, NumberButton clickedbutton)
        {
            int Points = 0;
            if (CheckEmptyRow(SelectedButton) && CheckEmptyRow(clickedbutton) && SelectedButton.Number.Y != clickedbutton.Number.Y)
            {
                Score += 20 * Level;
                Points += 20 * Level;
            }
            else if (CheckEmptyRow(SelectedButton) || CheckEmptyRow(clickedbutton))
            {
                Score += 10 * Level;
                Points += 10 * Level;
            }
            if (Distant)
            {
                Score += 4 * Level;
                Points += 4 * Level;
            }
            else
            {
                Score += 1 * Level;
                Points += 1 * Level;
            }
            Distant = false;
            GameUI.SetPointsGained(Points);
            GameUI.AnimatePointsGained();
            GameUI.SetScore(Score);
        }

        bool CheckEmptyRow(NumberButton clickedbutton)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (Numbers[clickedbutton.Number.Y][i].Visible) return false;
            }
            return true;
        }

        private bool CheckPossibleCombinations()
        {
            for (int i = 0; i < BoardSize + AddLineCount; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    var act = Numbers[i][j];
                    if (act == null || !act.Visible) continue;
                    if (RetrieveCombination(act) != null) return true;
                }
            }
            return false;
        }

        private void CheckEndGame()
        {
            if (AddLineCount >= MaxAddLines && !CheckPossibleCombinations())
            {
                MessageBox.Query("Game Over", "No more possible combinations. Game Over!", "OK");
                Application.RequestStop(); // Terminar la aplicación

            }
        }

        private void ResetBoardIfEmpty(GameUI GameUI)
        {
            bool boardEmpty = true;
            for (int i = 0; i < Numbers.Count; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (Numbers[i][j] != null && Numbers[i][j].Visible)
                    {
                        boardEmpty = false;
                        break;
                    }
                }
                if (!boardEmpty) break;
            }

            if (boardEmpty)
            {
                // Incrementar la fase
                CurrentPhase++;
                GameUI.SetPhaseLabel(CurrentPhase);

                // Restablecer el tablero
                InitializeBoard(GameUI);
                AddLineCount = 0;
                HintCount = 0;
                MessageBox.Query("Game Reset", "El tablero ha sido limpiado. Generando nuevo tablero...", "OK");
                GameUI.Refresh();
            }
        }

        private void InsertNewLine(int numbersCount, (int, int) lastPosition)
        {
            var (x, y) = lastPosition;
            if (x == -1 && y == -1) {
                x = BoardSize - 1;
                y = Numbers.Count;
            }

            int lastLineFreeSpace = BoardSize - x - 1;
            int exceds = (numbersCount - lastLineFreeSpace) % BoardSize;
            int newLinesCount = exceds > 0 ? (numbersCount - lastLineFreeSpace) / BoardSize + 1 : (numbersCount - lastLineFreeSpace) / BoardSize;
            newLinesCount -= Numbers.Count - y;
            for (int i = 0; i < newLinesCount; i++)
            {
                var newLine = new NumberButton[BoardSize];
                for(int j = 0; j < BoardSize; j++) {
                    var number = new Number(0, 0, -1);
                    var btn = new NumberButton(number);
                    btn.Visible = false;
                    newLine[j] = btn;
                }
                Numbers.Add(newLine);
            }
        }
        private void AddNewLine(GameUI GameUI)
        {
            if (AddLineCount >= MaxAddLines)
            {
                MessageBox.ErrorQuery("Lo siento", "Ya no puedes agregar mas lineas");
                return;
            }

            // Recoger números visibles del tablero
            List<int> visibleNumbers = new List<int>();
            int sizePreNewLines = Numbers.Count;
            for (int i = 0; i < sizePreNewLines; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (Numbers[i][j] != null && Numbers[i][j].Visible)
                    {
                        visibleNumbers.Add(Numbers[i][j].Number.Value);
                    }
                }
            }

            var (x, y) = GetLastEmptyPosition();
            MessageBox.Query("Test", $"GetLastNumberPosition Throw ({x}, {y})", "OK");
            InsertNewLine(visibleNumbers.Count, (x, y));
            if (x == -1 && y == -1) {
                x = 0;
                y = sizePreNewLines;
            }
            MessageBox.Query("Test", $"AddNewLine Throw ({x}, {y})", "OK");

            int visibleNumbersCursor = 0;
            // Añadir una nueva fila con los números visibles
            for (int i = y; i < Numbers.Count; i++)
            {
                int j = 0;
                if (i == y) j = x;
                for (; j < BoardSize; j++)
                {
                    if (visibleNumbersCursor >= visibleNumbers.Count) break;
                    var number = new Number(j, i, visibleNumbers[visibleNumbersCursor++]);
                    var btn = new NumberButton(number);

                    Numbers[i][j] = btn;
                    btn.Clicked += () => HandleButtonClick(GameUI, btn);
                    GameUI.Add(btn);
                }
            }

            AddLineCount++;
            GameUI.Refresh(); // Asegurarse de que el tablero se redibuje
            CheckEndGame(); // Comprobar si el juego debe terminar
        }

        private (int, int) GetLastNumberPosition()
        {
            for (int i = Numbers.Count - 1; i >= 0; i--)
            {
                for (int j = BoardSize - 1; j >= 0; j--)
                {
                    if (Numbers[i][j] != null && Numbers[i][j].Visible) {
                        return (j, i);
                    }
                }
            }
            return (-1, -1);
        }

        private (int, int) GetLastEmptyPosition()
        {
            var (x, y) = GetLastNumberPosition();
            MessageBox.Query("Test", $"GetLastNumberPosition Throw ({x}, {y})", "OK");
            if (x == -1 && y == -1) return (0, 0);

            if (x + 1 < BoardSize)
            {
                NumberButton? act = null;
                if (x == BoardSize - 1)
                {
                    if (y + 1 < Numbers.Count)
                    {
                        act = Numbers[y + 1][0];
                    }
                }
                else
                {
                    act = Numbers[y][x + 1];
                }

                if (act != null && !act.Visible)
                {
                    return (act.Number.X, act.Number.Y);
                }
            }

            MessageBox.Query("Test", $"GetLastNumberPositionAA Throw ({x}, {y})", "OK");
            return (-1, -1);
        }

        private void ProvideHint(GameUI GameUI)
        {
            if (HintCount >= MaxHints)
            {
                MessageBox.ErrorQuery("Hint", "Si quieres otra pista tendrás que pasar por una microtransacción.", "OK");
                return;
            }

            for (int i = 0; i < Numbers.Count; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    var act = Numbers[i][j];
                    if (act == null || !act.Visible) continue;
                    var pair = RetrieveCombination(act);
                    if (pair != null)
                    {
                        // Resaltamos la pista
                        GameUI.SetColorSelected(act, pair);
                        HintCount++;
                        GameUI.Refresh();
                        return;
                    }
                }
            }

            MessageBox.ErrorQuery("Hint", "No quedan posibles combinaciones, añade una nueva línea de números.", "OK");
        }


        private void UnSelect(params NumberButton[] buttons)
        {
            foreach (var btn in buttons)
            {
                btn.Number.IsMatched = false;
            }
        }

        private void Hide(params NumberButton[] buttons)
        {
            foreach (var btn in buttons)
            {
                btn.Visible = false;
            }
        }

        public int[] GetRow(int[,] matrix, int rowNumber)
        {
            int columns = matrix.GetLength(1);
            int[] row = new int[columns];

            for (int i = 0; i < columns; i++)
            {
                row[i] = matrix[rowNumber, i];
            }

            return row;
        }
    }
}