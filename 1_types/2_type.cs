using System;

public class DiamondPrinter
{
    public static void PrintDiamond(int n)
    {
        if (n % 2 == 0)
        {
            Console.WriteLine("N должно быть нечетным числом.");
        }

        int mid = n / 2;

        // Верхняя половина ромба.
        for (int i = 0; i < n; i++)
        {
            // Определяем количество пробелов перед первым X.
            int leadingSpaces = Math.Abs(mid - i);

            // Выводим ведущие пробелы.
            for (int j = 0; j < leadingSpaces; j++)
            {
                Console.Write(" ");
            }

            Console.Write("X");

            if (i != 0 && i != n - 1) 
            {
                // Количество пробелов между X'ами.
                int innerSpaces = (n - 2 * leadingSpaces) - 2;

                if (i == mid)
                {
                    // Для центральной строки innerSpaces будет
                    //  N-2, но мы должны поставить 
                    // один пробел в середине, и X по краям.
                    for (int j = 0; j < innerSpaces; j++)
                    {
                        Console.Write(" ");
                    }
                }
                else
                {
                    // Для остальных строк с двумя X.
                    for (int j = 0; j < innerSpaces; j++)
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("X"); 
            }
            Console.WriteLine();
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Пример для N=5:");
        PrintDiamond(5);

        Console.WriteLine("\nПример для N=7:");
        PrintDiamond(7);

        Console.WriteLine("\nПример для N=3:");
        PrintDiamond(3);
    }
}