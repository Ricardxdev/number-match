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

            GameUI = new GameUI();
            GameController = new GameController(GameUI, 3, 5, 3);

            
            Application.Top.Add(GameUI.GameWindow);
            Application.Run();
            Console.Out.Flush();
        }
    }
}