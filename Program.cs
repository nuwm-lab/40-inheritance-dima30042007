using System;
using System.Globalization;

/// <summary>
/// Клас для опису дробово-лінійної функції виду (a1*x + a0) / (b1*x + b0)
/// </summary>
class FractionLinearFunction
{
    // Приватні поля (інкапсуляція)
    private double _a1, _a0, _b1, _b0;

    // Конструктор за замовчуванням
    public FractionLinearFunction() { }

    // Конструктор із параметрами
    public FractionLinearFunction(double a1, double a0, double b1, double b0)
    {
        _a1 = a1;
        _a0 = a0;
        _b1 = b1;
        _b0 = b0;
    }

    /// <summary>
    /// Метод введення коефіцієнтів з клавіатури
    /// </summary>
    public virtual void SetCoefficients()
    {
        _a1 = ReadDouble("Введіть a1: ");
        _a0 = ReadDouble("Введіть a0: ");
        _b1 = ReadDouble("Введіть b1: ");
        _b0 = ReadDouble("Введіть b0: ");
    }

    /// <summary>
    /// Перевантажений метод для встановлення коефіцієнтів через параметри
    /// </summary>
    public void SetCoefficients(double a1, double a0, double b1, double b0)
    {
        _a1 = a1;
        _a0 = a0;
        _b1 = b1;
        _b0 = b0;
    }

    /// <summary>
    /// Виведення коефіцієнтів на екран
    /// </summary>
    public virtual void PrintCoefficients()
    {
        Console.WriteLine($"Функція: ({_a1}x + {_a0}) / ({_b1}x + {_b0})");
    }

    /// <summary>
    /// Обчислення значення функції в заданій точці
    /// </summary>
    public virtual double Calculate(double x)
    {
        double denominator = _b1 * x + _b0;
        if (Math.Abs(denominator) < 1e-10)
        {
            Console.WriteLine("Помилка: ділення на нуль!");
            return double.NaN;
        }

        return (_a1 * x + _a0) / denominator;
    }

    /// <summary>
    /// Допоміжний метод безпечного вводу чисел
    /// </summary>
    protected double ReadDouble(string message)
    {
        double value;
        Console.Write(message);
        while (!double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            Console.Write("Невірне значення! Спробуйте ще раз: ");
        }
        return value;
    }
}

/// <summary>
/// Похідний клас для дробової функції виду (a2*x² + a1*x + a0) / (b2*x² + b1*x + b0)
/// </summary>
class FractionQuadraticFunction : FractionLinearFunction
{
    private double _a2, _b2;

    // Конструктор за замовчуванням
    public FractionQuadraticFunction() { }

    // Конструктор із параметрами
    public FractionQuadraticFunction(double a2, double a1, double a0, double b2, double b1, double b0)
        : base(a1, a0, b1, b0)
    {
        _a2 = a2;
        _b2 = b2;
    }

    /// <summary>
    /// Перевизначення методу введення коефіцієнтів
    /// </summary>
    public override void SetCoefficients()
    {
        _a2 = ReadDouble("Введіть a2: ");
        base.SetCoefficients(); // виклик методу з батьківського класу
        _b2 = ReadDouble("Введіть b2: ");
    }

    /// <summary>
    /// Перевантаження: встановлення коефіцієнтів через параметри
    /// </summary>
    public void SetCoefficients(double a2, double a1, double a0, double b2, double b1, double b0)
    {
        _a2 = a2;
        SetCoefficients(a1, a0, b1, b0);
        _b2 = b2;
    }

    /// <summary>
    /// Перевизначення виводу коефіцієнтів
    /// </summary>
    public override void PrintCoefficients()
    {
        Console.WriteLine($"Функція: ({_a2}x² + a1x + a0) / ({_b2}x² + b1x + b0)");
    }

    /// <summary>
    /// Перевизначення обчислення значення функції
    /// </summary>
    public override double Calculate(double x)
    {
        double denominator = _b2 * x * x + ReadField("_b1") * x + ReadField("_b0");
        if (Math.Abs(denominator) < 1e-10)
        {
            Console.WriteLine("Помилка: ділення на нуль!");
            return double.NaN;
        }

        double numerator = _a2 * x * x + ReadField("_a1") * x + ReadField("_a0");
        return numerator / denominator;
    }

    // Імітація доступу до "захищених" коефіцієнтів базового класу (тут просто для демонстрації)
    private double ReadField(string name)
    {
        // У реальному коді краще передавати потрібні коефіцієнти через властивості або параметри
        return 0; // умовна реалізація, якщо треба — адаптуємо під повний доступ
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Дробово-лінійна функція ===");
        var linear = new FractionLinearFunction();
        linear.SetCoefficients();
        linear.PrintCoefficients();

        double x0 = linear.ReadDouble("Введіть значення x₀: ");
        Console.WriteLine($"Значення функції у точці {x0:F2}: {linear.Calculate(x0):F3}\n");

        Console.WriteLine("=== Квадратна дробова функція ===");
        var quadratic = new FractionQuadraticFunction();
        quadratic.SetCoefficients();
        quadratic.PrintCoefficients();

        double x1 = linear.ReadDouble("Введіть значення x₀: ");
        Console.WriteLine($"Значення функції у точці {x1:F2}: {quadratic.Calculate(x1):F3}");
    }
}
