using System;
using System.Globalization;
using System.Text;

#region Helpers
/// <summary>
/// Допоміжні методи для вводу з консолі.
/// </summary>
static class InputHelper
{
    /// <summary>
    /// Читає double з консолі (повторює запит до коректного вводу).
    /// За замовчуванням використовує CultureInfo.CurrentCulture.
    /// </summary>
    public static double ReadDouble(string prompt, CultureInfo culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        double value;
        Console.Write(prompt);
        string s = Console.ReadLine();
        while (!double.TryParse(s, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, culture, out value))
        {
            Console.Write("Невірне число. Спробуйте ще раз: ");
            s = Console.ReadLine();
        }
        return value;
    }

    /// <summary>
    /// Читає значення x0 (повторює, поки знаменник не стане ненульовим або користувач не хоче вийти).
    /// Повертає nullable double — null якщо користувач відмовився продовжувати.
    /// </summary>
    public static double? ReadX0ForDenominator(Func<double, bool> denominatorIsZero, string prompt, CultureInfo culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        while (true)
        {
            Console.Write(prompt);
            string s = Console.ReadLine();
            if (!double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture, out double x0))
            {
                Console.WriteLine("Невірне значення. Спробуйте ще раз.");
                continue;
            }

            if (denominatorIsZero(x0))
            {
                Console.WriteLine("Знаменник дорівнює нулю для цього x. Введіть інше x або напишіть 'c' щоб скасувати.");
                string choice = Console.ReadLine();
                if (choice != null && choice.Trim().ToLower() == "c")
                    return null;
                // else повторюємо цикл
                continue;
            }

            return x0;
        }
    }
}
#endregion

#region Base class
/// <summary>
/// Дробово-лінійна функція: (A1*x + A0) / (B1*x + B0)
/// </summary>
class FractionLinearFunction
{
    // Властивості з public get та protected set — дочірні класи можуть змінювати.
    public double A1 { get; protected set; }
    public double A0 { get; protected set; }
    public double B1 { get; protected set; }
    public double B0 { get; protected set; }

    public FractionLinearFunction() { }

    public FractionLinearFunction(double a1, double a0, double b1, double b0)
    {
        SetCoefficients(a1, a0, b1, b0);
    }

    /// <summary>
    /// Задати коефіцієнти через параметри (перевантаження).
    /// </summary>
    public void SetCoefficients(double a1, double a0, double b1, double b0)
    {
        A1 = a1;
        A0 = a0;
        B1 = b1;
        B0 = b0;
    }

    /// <summary>
    /// Задати коефіцієнти через ввід користувача.
    /// </summary>
    public virtual void SetCoefficientsFromInput(CultureInfo culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        Console.WriteLine("Введіть коефіцієнти дробово-лінійної функції (A1, A0, B1, B0):");
        A1 = InputHelper.ReadDouble("A1 = ", culture);
        A0 = InputHelper.ReadDouble("A0 = ", culture);
        B1 = InputHelper.ReadDouble("B1 = ", culture);
        B0 = InputHelper.ReadDouble("B0 = ", culture);
    }

    /// <summary>
    /// Вивести коефіцієнти у вигляді формули.
    /// </summary>
    public virtual void PrintCoefficients(CultureInfo culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        string num = FormatLinearPolynomial(A1, A0, culture);
        string den = FormatLinearPolynomial(B1, B0, culture);
        Console.WriteLine($"f(x) = ({num}) / ({den})");
    }

    /// <summary>
    /// Обчислити значення функції. Якщо знаменник = 0, повертає null.
    /// </summary>
    public virtual double? Calculate(double x)
    {
        double denom = B1 * x + B0;
        if (Math.Abs(denom) < 1e-12) return null;
        double num = A1 * x + A0;
        return num / denom;
    }

    #region Formatting helpers
    protected static string FormatLinearPolynomial(double c1, double c0, CultureInfo culture)
    {
        // Повертає рядок виду "2x + 3" або "-x - 5"
        var sb = new StringBuilder();
        AppendTerm(sb, c1, "x", culture);
        AppendTerm(sb, c0, "", culture);
        if (sb.Length == 0) return "0";
        return sb.ToString();
    }

    protected static void AppendTerm(StringBuilder sb, double coef, string variable, CultureInfo culture)
    {
        if (Math.Abs(coef) < 1e-12) return;

        string coefStr;
        double abs = Math.Abs(coef);

        // Формат коефіцієнта (без зайвих нулів)
        if (variable != "" && Math.Abs(abs - 1.0) < 1e-12)
            coefStr = ""; // "1x" -> "x"
        else
            coefStr = abs.ToString("G", culture);

        if (sb.Length == 0)
        {
            // перший термін
            sb.Append(coef < 0 ? "-" : "");
            sb.Append(coefStr);
            sb.Append(variable);
        }
        else
        {
            sb.Append(coef < 0 ? " - " : " + ");
            sb.Append(coefStr);
            sb.Append(variable);
        }
    }
    #endregion
}
#endregion

#region Quadratic class
/// <summary>
/// Дробова функція з квадратичним чисельником і знаменником:
/// (A2*x^2 + A1*x + A0) / (B2*x^2 + B1*x + B0)
/// </summary>
class FractionQuadraticFunction : FractionLinearFunction
{
    public double A2 { get; protected set; }
    public double B2 { get; protected set; }

    public FractionQuadraticFunction() : base() { }

    public FractionQuadraticFunction(double a2, double a1, double a0, double b2, double b1, double b0)
        : base(a1, a0, b1, b0)
    {
        A2 = a2;
        B2 = b2;
    }

    /// <summary>
    /// Перевантажений метод для задання 6 коефіцієнтів.
    /// </summary>
    public void SetCoefficients(double a2, double a1, double a0, double b2, double b1, double b0)
    {
        A2 = a2;
        SetCoefficients(a1, a0, b1, b0); // метод батька
        B2 = b2;
    }

    /// <summary>
    /// Зчитування коефіцієнтів з клавіатури (перевизначення).
    /// Читає порядок: A2, A1, A0, B2, B1, B0
    /// </summary>
    public override void SetCoefficientsFromInput(CultureInfo culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        Console.WriteLine("Введіть коефіцієнти квадратичної дробової функції (A2, A1, A0, B2, B1, B0):");
        A2 = InputHelper.ReadDouble("A2 = ", culture);
        // Викликаємо встановлення лінійних коефіцієнтів (A1,A0,B1,B0)
        double a1 = InputHelper.ReadDouble("A1 = ", culture);
        double a0 = InputHelper.ReadDouble("A0 = ", culture);
        double b2 = InputHelper.ReadDouble("B2 = ", culture);
        double b1 = InputHelper.ReadDouble("B1 = ", culture);
        double b0 = InputHelper.ReadDouble("B0 = ", culture);

        // Записуємо
        SetCoefficients(A2, a1, a0, b2, b1, b0);
    }

    public override void PrintCoefficients(CultureInfo culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        string num = FormatQuadraticPolynomial(A2, A1, A0, culture);
        string den = FormatQuadraticPolynomial(B2, B1, B0, culture);
        Console.WriteLine($"f(x) = ({num}) / ({den})");
    }

    public override double? Calculate(double x)
    {
        double denom = B2 * x * x + B1 * x + B0;
        if (Math.Abs(denom) < 1e-12) return null;
        double num = A2 * x * x + A1 * x + A0;
        return num / denom;
    }

    #region Helpers for formatting quadratic
    protected static string FormatQuadraticPolynomial(double c2, double c1, double c0, CultureInfo culture)
    {
        var sb = new StringBuilder();
        AppendTerm(sb, c2, "x^2", culture);
        AppendTerm(sb, c1, "x", culture);
        AppendTerm(sb, c0, "", culture);
        if (sb.Length == 0) return "0";
        return sb.ToString();
    }
    #endregion
}
#endregion

#region Scaled derived class (extra task)
/// <summary>
/// Приклад додаткової практичної задачі: масштабований квадратичний дробовий клас.
/// Повертає Scale * (базове значення).
/// </summary>
class ScaledFractionQuadraticFunction : FractionQuadraticFunction
{
    public double Scale { get; private set; } = 1.0;

    public ScaledFractionQuadraticFunction() : base() { }

    public ScaledFractionQuadraticFunction(double scale, double a2, double a1, double a0, double b2, double b1, double b0)
        : base(a2, a1, a0, b2, b1, b0)
    {
        Scale = scale;
    }

    public void SetScale(double scale) => Scale = scale;

    public override void PrintCoefficients(CultureInfo culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        base.PrintCoefficients(culture);
        Console.WriteLine($"Scale = {Scale.ToString("G", culture)}");
    }

    public override double? Calculate(double x)
    {
        var baseVal = base.Calculate(x);
        if (baseVal == null) return null;
        return baseVal * Scale;
    }
}
#endregion

#region Program (Main)
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var culture = CultureInfo.CurrentCulture;

        Console.WriteLine("=== Дробово-лінійна функція ===");
        var linear = new FractionLinearFunction();
        linear.SetCoefficientsFromInput(culture);
        linear.PrintCoefficients(culture);

        // Читаємо x0, при некоректності знаменника пропонуємо повторити ввід x0
        double? x0 = InputHelper.ReadX0ForDenominator(
            denomX => Math.Abs(linear.B1 * denomX + linear.B0) < 1e-12,
            "Введіть x₀ для обчислення: ",
            culture);

        if (x0 != null)
        {
            var res = linear.Calculate(x0.Value);
            Console.WriteLine($"f({x0.Value.ToString("G", culture)}) = {res.Value.ToString("F6", culture)}");
        }
        else
        {
            Console.WriteLine("Обчислення для дробово-лінійної функції скасовано користувачем.");
        }

        Console.WriteLine("\n=== Квадратична дробова функція ===");
        var quad = new FractionQuadraticFunction();
        quad.SetCoefficientsFromInput(culture);
        quad.PrintCoefficients(culture);

        double? x1 = InputHelper.ReadX0ForDenominator(
            denomX => Math.Abs(quad.B2 * denomX * denomX + quad.B1 * denomX + quad.B0) < 1e-12,
            "Введіть x₀ для обчислення: ",
            culture);

        if (x1 != null)
        {
            var res = quad.Calculate(x1.Value);
            Console.WriteLine($"f({x1.Value.ToString("G", culture)}) = {res.Value.ToString("F6", culture)}");
        }
        else
        {
            Console.WriteLine("Обчислення для квадратичної дробової функції скасовано користувачем.");
        }

        // Демонстрація додаткового класу ScaledFractionQuadraticFunction
        Console.WriteLine("\nБажаєте перевірити масштабований приклад (ScaledFractionQuadraticFunction)? (y/n)");
        string ans = Console.ReadLine()?.Trim().ToLower();
        if (ans == "y" || ans == "так")
        {
            double scale = InputHelper.ReadDouble("Scale = ", culture);
            var scaled = new ScaledFractionQuadraticFunction();
            scaled.SetCoefficientsFromInput(culture);
            scaled.SetScale(scale);
            scaled.PrintCoefficients(culture);

            double? x2 = InputHelper.ReadX0ForDenominator(
                denomX => Math.Abs(scaled.B2 * denomX * denomX + scaled.B1 * denomX + scaled.B0) < 1e-12,
                "Введіть x₀ для обчислення: ",
                culture);

            if (x2 != null)
            {
                var r = scaled.Calculate(x2.Value);
                Console.WriteLine($"scaled f({x2.Value.ToString("G", culture)}) = {r.Value.ToString("F6", culture)}");
            }
        }

        Console.WriteLine("\nКінець програми. Натисніть будь-яку клавішу...");
        Console.ReadKey();
    }
}
#endregion


