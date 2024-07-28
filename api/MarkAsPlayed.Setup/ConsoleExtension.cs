namespace MarkAsPlayed.Setup;

internal class ConsoleExtension
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
        Console.ForegroundColor = color;
        if (newLine)
            Console.WriteLine($"-----\n{message}\n-----");
        else
            Console.WriteLine(message);
        Console.ForegroundColor = oldColor;
    }
}