using System;

namespace CodingClassics
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new BoingGame())
                game.Run();
        }
    }
}