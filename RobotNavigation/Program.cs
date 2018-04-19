using System;

namespace RobotNavigation
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var game = new Game1();

            if (args.Length == 2)
                game.Setup(args);

            game.Run();
            game.Dispose();
        }
    }
#endif
}
