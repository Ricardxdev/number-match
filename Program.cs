namespace myproj
{
    using System;
    using Terminal.Gui;

    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();
            var top = Application.Top;

            var win = new Window("Number Match Game")
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Crear una grilla de números
            int size = 5;
            int[,] grid = CreateGrid(size);

            // Crear un contenedor para la grilla
            var gridView = new FrameView("Grid")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            bool OneSelected = false;
            var LastSelected = new Number(0, 0 , 0);
            // Llenar el contenedor con botones
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var number = new Number(i, j, grid[i, j]);
                    var button = new NumberButton(number){
                        X = j * 4, // Espaciado
                        Y = i
                    };

                    // Evento al hacer clic en el botón
                    button.Clicked += () => {
                        // Cambiar el color del botón
                        button.ColorScheme = new ColorScheme
                        {
                            Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                            Focus = Application.Driver.MakeAttribute(Color.White, Color.BrightGreen),
                            HotNormal = Application.Driver.MakeAttribute(Color.Black, Color.BrightYellow),
                            HotFocus = Application.Driver.MakeAttribute(Color.Red, Color.BrightRed),
                            Disabled = Application.Driver.MakeAttribute(Color.Gray, Color.Black)
                        };

                        button.Number.IsSelected = true;
                        if (!OneSelected) {
                            OneSelected = true;
                        } else {
                            // implementar lógica de matcheo
                        }
                    };

                    gridView.Add(button);
                }
            }

            win.Add(gridView);

            // Botón para salir
            var quit = new Button("Quit") { X = 0, Y = size + 1 };
            quit.Clicked += () => Application.RequestStop();
            win.Add(quit);

            top.Add(win);
            Application.Run();
        }

        static int[,] CreateGrid(int size)
        {
            var random = new Random();
            var grid = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    grid[i, j] = random.Next(0, 10); // Números del 1 al 9
                }
            }
            return grid;
        }
    }
}
