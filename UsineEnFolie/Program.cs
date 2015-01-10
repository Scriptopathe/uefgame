using System;

namespace UeFGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (UeFClass game = new UeFClass())
            {
                game.Run();
            }
        }
    }
#endif
}

