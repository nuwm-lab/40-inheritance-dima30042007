using System;

class FractionalLinearFunction
{
    protected double a1, a0; // коефіцієнти чисельника
    protected double b1, b0; // коефіцієнти знаменника

    // Метод задання коефіцієнтів з клавіатури
    public virtual void SetCoefficients()
    {
        Console.WriteLine("Введіть коефіцієнти для дробово-лінійної функції (a1, a0, b1, b0):");
        Console.Write("a1 = ");
        a1 = double.Parse(Console.ReadLine());
        Console.Write("a0 = ");
        a0 = double.Parse(Console.ReadLine());
        Console.Write("b1 = ");
        b1 = double.Parse(Console.ReadLine());
        Console.Write("b0 = ");
        b0 = double.Parse(Console.ReadLine());
    }

    // Виведення коефіцієнтів
    public virtual void PrintCoefficients()
    {
        Console.WriteLine($"Чисельник: {a1}x + {a0}");
        Console.WriteLine($"Знаменник: {b1}x + {b0}");
    }

    // Обчислення значення функції
    public virtual double Calculate(double x0)
    {
        double denominator = b1 * x0 + b0;
        if (denominator == 0)
        {
            throw new DivideByZeroException("Знаменник дорівнює нулю!");
        }
        return (a1 * x0 + a0) / denominator;
    }
}

// Похідний клас — дробова функція
class FractionalFunction : FractionalLinearFunction
{
    private double a2; // коефіцієнт x^2 чисельника
    private double b2; // коефіцієнт x^2 знаменника

    // Перевантажений метод задання коефіцієнтів
    public override void SetCoefficients()
    {
        Console.WriteLine("Введіть коефіцієнти для дробової функції (a2, a1, a0, b2, b1, b0):");
        Console.Write("a2 = ");
        a2 = double.Parse(Console.ReadLine());
        Console.Write("a1 = ");
        a1 = double.Parse(Console.ReadLine());
        Console.Write("a0 = ");
        a0 = double.Parse(Console.ReadLine());
        Console.Write("b2 = ");
        b2 = double.Parse(Console.ReadLine());
        Console.Write("b1 = ");
        b1 = double.Parse(Console.ReadLine());
        Console.Write("b0 = ");
        b0 = double.Parse(Console.ReadLine());
    }

    // Перевантажений метод виведення коефіцієнтів
    public override void PrintCoefficients()
    {
        Console.WriteLine($"Чисельник: {a2}x^2 + {a1}x + {a0}");
        Console.WriteLine($"Знаменник: {b2}x^2 + {b1}x + {b0}");
    }

    // Перевантажений метод обчислення значення
    public override double Calculate(double x0)
    {
        double denominator = b2 * Math.Pow(x0, 2) + b1 * x0 + b0;
        if (denominator == 0)
        {
            throw new DivideByZeroException("Знаменник дорівнює нулю!");
        }
        double numerator = a2 * Math.Pow(x0, 2) + a1 * x0 + a0;
        return numerator / denominator;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== ДРОБОВО-ЛІНІЙНА ФУНКЦІЯ ===");
        FractionalLinearFunction f1 = new FractionalLinearFunction();
        f1.SetCoefficients();
        f1.PrintCoefficients();
        Console.Write("\nВведіть значення x0: ");
        double x0 = double.Parse(Console.ReadLine());
        Console.WriteLine($"Значення функції у точці x0 = {x0}: {f1.Calculate(x0)}");

        Console.WriteLine("\n=== ДРОБОВА ФУНКЦІЯ ===");
        FractionalFunction f2 = new FractionalFunction();
        f2.SetCoefficients();
        f2.PrintCoefficients();
        Console.Write("\nВведіть значення x0: ");
        x0 = double.Parse(Console.ReadLine());
        Console.WriteLine($"Значення функції у точці x0 = {x0}: {f2.Calculate(x0)}");
    }
}

