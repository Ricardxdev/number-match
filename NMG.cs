namespace NumberMatchGame
{
    using System.ComponentModel.DataAnnotations;
    using Terminal.Gui;
    public class NumberMatchGame
    {
        private Window? GameWindow;
        private FrameView? GameBoard;
        private Label? ScoreLabel;
        private int Score = 0;
        private int BoardSize;
        private NumberButton[,]? NumberButtons;
        private NumberButton? SelectedButton;
        public ColorScheme? DefaultColorScheme;
        public ColorScheme? SelectedColorScheme;

        public void Start()
        {
            Application.Init();

            BoardSize = 12;

            InitializeColorSchemes();

            InitializeComponents();

            InitializeBoard();

            GameWindow.Add(GameBoard, ScoreLabel);
            Application.Top.Add(GameWindow);
            Application.Run();
        }

        private void InitializeColorSchemes()
        {
            DefaultColorScheme = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.White, Color.Gray),
                Disabled = Application.Driver.MakeAttribute(Color.Gray, Color.Black)
            };

            SelectedColorScheme = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Black),
                Focus = Application.Driver.MakeAttribute(Color.White, Color.BrightBlue),
                Disabled = Application.Driver.MakeAttribute(Color.Gray, Color.Black)
            };
        }

        private void InitializeComponents()
        {
            // Crear ventana principal
            GameWindow = new Window("Number Match")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Crear tablero de juego
            GameBoard = new FrameView("Game Board")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill() - 2,
                Height = Dim.Fill() - 3
            };

            // Crear etiqueta de puntaje
            ScoreLabel = new Label($"Score: {Score}")
            {
                X = 1,
                Y = Pos.Bottom(GameBoard)
            };
        }

        private void InitializeBoard()
        {
            NumberButtons = new NumberButton[BoardSize, BoardSize];
            var random = new Random();

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    var number = new Number(i, j, random.Next(1, 10));
                    var btn = new NumberButton(number)
                    {
                        Text = number.Value.ToString(),
                        X = j * 5,
                        Y = i * 2,
                        Width = 4,
                        Height = 1
                    };

                    btn.Clicked += () => HandleButtonClick(btn);
                    NumberButtons[i, j] = btn;
                    GameBoard.Add(btn);
                }
            }
        }

        private void HandleButtonClick(NumberButton clickedButton)
        {
            if (!clickedButton.Number.IsSelected)
            {
                clickedButton.Number.IsSelected = true;
                clickedButton.ColorScheme = SelectedColorScheme;
            }
            else
            {
                clickedButton.Number.IsSelected = false;
                clickedButton.ColorScheme = DefaultColorScheme;
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
                Match(clickedButton);
            }
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
            int y = num_a.Number.X;
            int x = num_a.Number.Y;
            int columnsCount = NumberButtons.GetLength(0);
            int rowsCount = NumberButtons.GetLength(1);

            bool found = false;
            while (!found)
            {
                x += direction[0];
                y += direction[1];

                if (!(x >= 0 && x < columnsCount))
                {
                    if(direction[1] != 0) break;

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

                var act = NumberButtons[y, x];
                if (!act.Visible) continue;

                if ((act.Number.X != num_b.Number.X) || (act.Number.Y != num_b.Number.Y)) break;

                return true;
            }

            return found;
        }

        private void Match(NumberButton clickedButton)
        {
            // Verificar si los números suman 10 o son iguales
            int num1 = SelectedButton.Number.Value;
            int num2 = clickedButton.Number.Value;

            if ((num1 + num2 == 10) || (num1 == num2))
            {
                if (BFS(SelectedButton, clickedButton))
                {
                    // Remover los botones y actualizar el puntaje
                    SelectedButton.Visible = false;
                    clickedButton.Visible = false;
                    Score += 10;
                    ScoreLabel.Text = $"Score: {Score}";
                }
            }

            SelectedButton.ColorScheme = DefaultColorScheme;
            SelectedButton.Number.IsSelected = false;
            clickedButton.ColorScheme = DefaultColorScheme;
            clickedButton.Number.IsSelected = false;
            SelectedButton = null;
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