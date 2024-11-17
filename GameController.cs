using Terminal.Gui;

namespace NumberMatchGame
{
    public class GameController
    {
        private int Score = 0;
        public bool Distant = false;
        private int BoardSize;
        // Create a list with initial capacity of 10
        private List<NumberButton[]>? Numbers;
        //private NumberButton[,]? Numbers;
        private NumberButton? SelectedButton;
        private int AddLineCount = 0;
        private int HintCount = 0;
        int CurrentPhase = 1;

        public GameController(GameUI GameUI, int BoardSize, int AddLineCount = 5, int HintCount = 3)
        {
            this.AddLineCount = AddLineCount;
            this.HintCount = HintCount;
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

            for (int i = 0; i < Numbers.Count; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    var number = new Number(j, i, random.Next(1, 10));
                    var btn = new NumberButton(number);

                    btn.Clicked += () => HandleButtonClick(GameUI, btn);
                    Numbers[i][j] = btn;
                    GameUI.Add(btn);
                }
            }
        }

        private void RefreshBoard(GameUI GameUI) {
            for (int i = 0; i < Numbers.Count; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    NumberButton btn = Numbers[i][j];
                    btn.Refresh(j, i);
                    
                }
            }

            GameUI.Refresh();
        }

        private async void HandleButtonClick(GameUI GameUI, NumberButton clickedButton)
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
                if ((BFS(SelectedButton, clickedButton)))
                {
                    // Remover los botones y actualizar el puntaje
                    Hide(SelectedButton, clickedButton);
                    Scoring(GameUI);
                    ResetBoardIfEmpty(GameUI);
                    CheckEndGame();

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

        void Scoring(GameUI GameUI)
        {
            int Points = 0;
            int RemovedRows = CheckAndReplaceRows(GameUI);

            if (Distant)
            {
                Score += 4 * CurrentPhase;
                Points += 4 * CurrentPhase;
            }
            else
            {
                Score += 1 * CurrentPhase;
                Points += 1 * CurrentPhase;
            }
            Distant = false;

            Score += 10 * RemovedRows * CurrentPhase;
            Points += 10 * RemovedRows * CurrentPhase;
            GameUI.SetPointsGained(Points);
            GameUI.SetScore(Score);
        }

        bool CheckEmptyRow(int Y)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (Numbers[Y][i].Visible) return false;
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

        private void InsertNewLine(int numbersCount, (int, int) lastPosition)
        {
            var (x, y) = lastPosition;
            if (x == -1 && y == -1)
            {
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
                for (int j = 0; j < BoardSize; j++)
                {
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
            if (AddLineCount <= 0)
            {
                MessageBox.ErrorQuery("Lo siento", "Ya no puedes agregar mas lineas", "OK");
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
            InsertNewLine(visibleNumbers.Count, (x, y));
            if (x == -1 && y == -1)
            {
                x = 0;
                y = sizePreNewLines;
            }

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

            AddLineCount--;
            GameUI.SetLines(AddLineCount);
            GameUI.Refresh(); // Asegurarse de que el tablero se redibuje
            CheckEndGame(); // Comprobar si el juego debe terminar
        }

        private (int, int) GetLastNumberPosition()
        {
            for (int i = Numbers.Count - 1; i >= 0; i--)
            {
                for (int j = BoardSize - 1; j >= 0; j--)
                {
                    if (Numbers[i][j] != null && Numbers[i][j].Visible)
                    {
                        return (j, i);
                    }
                }
            }
            return (0, 0);
        }

        private (int, int) GetLastEmptyPosition()
        {
            var (x, y) = GetLastNumberPosition();

            if (x + 1 < BoardSize)
            {
                if (x == 0 && y == 0)
                {
                    return (0, 0);
                }
                if (x == BoardSize - 1)
                {
                    if (y + 1 < Numbers.Count)
                    {
                        return (0, y + 1);
                    }
                }
                else
                {
                    if (x + 1 < BoardSize) return (x + 1, y);
                }
            }

            return (-1, -1);
        }

        private void ProvideHint(GameUI GameUI)
        {
            if (HintCount <= 0)
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
                        HintCount--;
                        GameUI.SetHints(HintCount);
                        GameUI.Refresh();
                        return;
                    }
                }
            }

            MessageBox.ErrorQuery("Hint", "No quedan posibles combinaciones, añade una nueva línea de números.", "OK");
        }

        private int CheckAndReplaceRows(GameUI GameUI)
        {
            int count = 0;
            bool removed;
            do {
                removed = false;
                for(int i = 0; i < Numbers.Count; i++) {
                    if(CheckEmptyRow(i)) {
                        Numbers.RemoveAt(i);
                        ++count;
                        removed = true;
                        break;
                    }
                }
                RefreshBoard(GameUI);
            } while(removed);
            return count;
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
                AddLineCount = 5;
                GameUI.SetLines(AddLineCount);
                HintCount = 3;
                GameUI.SetHints(HintCount);
                MessageBox.Query("Game Reset", "El tablero ha sido limpiado. Generando nuevo tablero...", "OK");
                GameUI.Refresh();
            }
        }

         private void CheckEndGame()
        {
            if (AddLineCount <= 0 && !CheckPossibleCombinations())
            {
                MessageBox.Query("Game Over", "No more possible combinations. Game Over!", "OK");
                Application.RequestStop(); // Terminar la aplicación

            }
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