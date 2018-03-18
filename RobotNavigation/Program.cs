﻿using System;

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
            using (var game = new Game1())
            {
                game.LoadGrid(args[0]);
                game.Run();
            }
        }
    }
#endif
}
