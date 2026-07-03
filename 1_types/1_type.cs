using System;
using System.Text;

public class CompoundInterestCalculator
{
    public static string CalculateCompoundInterest
    (double initialDeposit, int years, double interestRate)
    {
        StringBuilder resultBuilder = new StringBuilder();
        double currentAmount = initialDeposit;
        
        for (int year = 1; year <= years; year++)
        {
            currentAmount *= (1 + interestRate / 100);
            resultBuilder.AppendLine($@"Год {year}: 
            {currentAmount:F2} руб.");
        }
        
        return resultBuilder.ToString();
    }

    public static void Main(string[] args)
    {
        // Пример использования:
        Console.WriteLine(CalculateCompoundInterest(1000, 3, 10));

        Console.WriteLine("\n" + CalculateCompoundInterest(5000, 5, 7.5));
    }
}