using System.Drawing;

namespace MarkAsPlayed.Setup;

internal static class ConsoleExtension
{
    public static void WriteLine(String message)
    {
        WriteLine(message, ConsoleColor.White, false);
    }

    public static void WriteLine(String message, ConsoleColor color)
    {
        WriteLine(message, color, false);
    }

    public static void WriteLine(String message, ConsoleColor color, bool newLine = false)
    {
        var oldColor = Console.ForegroundColor;

        if (newLine)
        {
            Console.WriteLine("--------");
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
            Console.WriteLine("--------");
        }
        else
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }
    }
}