using Terminal.Gui;

namespace NumberMatchGame {
    public class GameUI
    {
        public Window? GameWindow;
        public FrameView? GameBoard;
        public Label? ScoreLabel;
        public Label? PointsGained;
        public Label? AddLineLabel;
        public Label? HintLabel;
        public Button? AddLineButton { get; private set; }
        public Button? HintButton { get; private set; }
        public ColorScheme? DefaultColorScheme;
        public ColorScheme? SelectedColorScheme;
        public ColorScheme? SpecialColorScheme;
        Label? PhaseLabel;

        public GameUI(int Score = 0, int Points = 0, int CurrentPhase = 1, int CurrentLines = 5, int CurrentHints = 3)
        {
            InitializeColorSchemes();
            InitializeComponents(Score, Points, CurrentPhase, CurrentLines, CurrentHints);
        }

        public void Refresh() {
            GameBoard.SetNeedsDisplay();
        }
        private void InitializeColorSchemes()
        {
            DefaultColorScheme = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.White, Color.Gray),
                Disabled = Application.Driver.MakeAttribute(Color.Gray, Color.Black)
            };

            SpecialColorScheme = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.Blue, Color.Blue),
            };

            SelectedColorScheme = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Black),
                Focus = Application.Driver.MakeAttribute(Color.White, Color.BrightBlue),
                Disabled = Application.Driver.MakeAttribute(Color.Gray, Color.Black)
            };
        }

        private void InitializeComponents(int Score, int Points, int CurrentPhase, int CurrentLines, int CurrentHints)
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

            PointsGained = new Label($"+{Points}")
            {
                X = Pos.Right(ScoreLabel) + 2,
                Y = Pos.Bottom(GameBoard),
                ColorScheme = SpecialColorScheme
            };

            // Crear etiqueta de fase
            PhaseLabel = new Label($"Phase: {CurrentPhase}")
            {
                X = Pos.Center(),
                Y = 0

            };

            // Crear botón para añadir línea
            AddLineButton = new Button("Add Line")
            {
                X = Pos.Right(PointsGained) + 2,
                Y = Pos.Bottom(GameBoard)
            };

            // Crear botón para pedir pista
            HintButton = new Button("Hint")
            {
                X = Pos.Right(AddLineButton) + 2,
                Y = Pos.Bottom(GameBoard)
            };

            AddLineLabel = new Label($"Add Lines: {CurrentLines}")
            {
                X = Pos.Right(HintButton) + 2,
                Y = Pos.Bottom(GameBoard)
            };

            HintLabel = new Label($"Hints: {CurrentHints}"){
                X = Pos.Right(AddLineLabel) + 2,
                Y = Pos.Bottom(GameBoard)
            };

            GameWindow.Add(GameBoard, ScoreLabel, PointsGained, PhaseLabel, AddLineButton, HintButton, AddLineLabel, HintLabel);
        }

        public void SetColorSelected(params NumberButton[] buttons)
        {
            foreach (var btn in buttons)
            {
                btn.ColorScheme = SelectedColorScheme;
            }
        }

        public void ResetColor(params NumberButton[] buttons)
        {
            foreach (var btn in buttons)
            {
                btn.ColorScheme = DefaultColorScheme;
            }
        }

        public async void FadeInOut(Label label)
        {
            int interval1 = 200, interval2 = 500;
            label.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.Blue, Color.White);
            Application.Refresh();
            await Task.Delay(interval1);
            label.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue);
            Application.Refresh();
            await Task.Delay(interval1);
            label.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.Blue, Color.White);
            Application.Refresh();
            await Task.Delay(interval1);
            label.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue);
            Application.Refresh();
            await Task.Delay(interval2);
            label.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.Blue, Color.Blue);
            Application.Refresh();
        }

        public void SetPhaseLabel(int CurrentPhase) {
            PhaseLabel.Text = $"Phase: {CurrentPhase}";
            //FadeInOut(PhaseLabel);
        }

        public void SetPointsGained(int Points) {
            PointsGained.Text = $"+ {Points}";
            FadeInOut(PointsGained);
        }

        public void SetScore(int Score) {
            ScoreLabel.Text = $"Score: {Score}";
        }

        public void SetLines(int Lines) {
            AddLineLabel.Text = $"Add Lines: {Lines}";
            //FadeInOut(AddLineLabel);
        }

        public void SetHints(int Hints) {
            HintLabel.Text = $"Add Hints: {Hints}";
            //FadeInOut(HintLabel);
        }
        public void Add(View view) {
            GameBoard.Add(view);
        }
    }
}