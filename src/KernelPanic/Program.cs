﻿using System;

namespace KernelPanic
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var statistics = new Statistics())
            using (var game = new Game1(statistics))
                game.Run();
        }
    }
#endif
}
