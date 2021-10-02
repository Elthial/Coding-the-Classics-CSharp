using System;

namespace Cavern
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Cavern())
                game.Run();
        }
    }
}
