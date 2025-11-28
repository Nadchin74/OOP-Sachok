using System;
using System.Collections.Generic;
using System.Linq;

// ==========================================================
// 1. Оголошення власного делегата (вимога 1)
// ==========================================================
/// <summary>
/// Делегат для розрахунку премії на основі зарплати та коефіцієнта.
/// </summary>
public delegate decimal BonusCalculator(decimal salary, double coefficient);

// ==========================================================
// 2. Клас Employee (сутність предметної області)
// ==========================================================
public class Employee
{
    public string Name { get; set; }
    public string Position { get; set; }
    public decimal Salary { get; set; }

    /// <summary>
    /// Конструктор класу Employee.
    /// </summary>
    public Employee(string name, string position, decimal salary)
    {
        Name = name;
        Position = position;
        Salary = salary;
    }

    /// <summary>
    /// Перевизначення ToString для зручного виводу.
    /// </summary>
    public override string ToString()
    {
        return $"{Name} ({Position}): {Salary:C}";
    }
}

// ==========================================================
// 3. Основна програма
// ==========================================================
public class Program
{
    public static void Main(string[] args)
    {
        // 3.1. Ініціалізація колекції
        List<Employee> employees = new List<Employee>
        {
            new Employee("Іван", "Менеджер", 12500.00m),
            new Employee("Олена", "Розробник", 35000.50m),
            new Employee("Петро", "Адміністратор", 9800.00m),
            new Employee("Марія", "Аналітик", 18000.00m),
            new Employee("Сергій", "Розробник", 28000.00m)
        };

        Console.WriteLine("==================================================");
        Console.WriteLine("    ЛАБОРАТОРНА 6: ДЕЛЕГАТИ ТА ЛЯМБДА-ВИРАЗИ");
        Console.WriteLine("==================================================");


        // ----------------------------------------------------------------------
        // 4. Демонстрація Власного Делегата + Лямбда-вираз
        // ----------------------------------------------------------------------
        
        // Коментар: Реалізація власного делегата за допомогою ЛЯМБДА-ВИРАЗУ
        BonusCalculator calculateBonus = (sal, coeff) => sal * (decimal)coeff;
        
        decimal salaryToTest = employees.First(e => e.Name == "Сергій").Salary;
        decimal bonusAmount = calculateBonus(salaryToTest, 0.20); // Премія 20%
        
        Console.WriteLine("\n--- 1. Власний Делегат (BonusCalculator) ---");
        Console.WriteLine($"Зарплата Сергія: {salaryToTest:C}");
        Console.WriteLine($"Премія (20%): {bonusAmount:C}");


        // ----------------------------------------------------------------------
        // 5. Демонстрація Action<T> (вимога варіанту 5)
        // ----------------------------------------------------------------------

        // Коментар: Action<Employee> - делегат, що приймає Employee і НЕ повертає значення.
        // Використовується для виводу інформації в консоль.
        Action<Employee> displayEmployeeInfo = (emp) => 
        {
            Console.WriteLine($" > {emp.Name} | Посада: {emp.Position} | Зарплата: {emp.Salary:C}");
        };
        
        Console.WriteLine("\n--- 2. Action<Employee> (Вивід списку в консоль) ---");
        // Використання Action для виводу всіх елементів
        employees.ForEach(displayEmployeeInfo);

        // ----------------------------------------------------------------------
        // 6. Демонстрація Func<T, TResult> (вимога варіанту 5) та LINQ
        // ----------------------------------------------------------------------
        
        // Коментар: Func<Employee, bool> - делегат (предикат), що приймає Employee і повертає bool.
        // Використовується для фільтрації.
        Func<Employee, bool> highSalaryFilter = (emp) => emp.Salary > 10000.00m;

        // Використання Func<> у LINQ.Where для вибору працівників із зарплатою > 10 000.
        var highEarners = employees.Where(highSalaryFilter); 
        
        Console.WriteLine("\n--- 3. Func<Employee, bool> та LINQ (Зарплата > 10 000) ---");
        foreach (var emp in highEarners)
        {
            // Повторне використання Action для виводу
            displayEmployeeInfo(emp);
        }

        // ----------------------------------------------------------------------
        // 7. Демонстрація Анонімного Методу та Додатковий LINQ
        // ----------------------------------------------------------------------
        
        Console.WriteLine("\n--- 4. LINQ та Анонімний Метод ---");
        
        // Анонімний метод (функціонально ідентичний лямбда-виразу у цьому контексті)
        Predicate<Employee> isManager = delegate(Employee emp)
        {
            // Коментар: Predicate<Employee> - спеціалізований Func<Employee, bool>
            return emp.Position == "Менеджер";
        };

        var managers = employees.FindAll(isManager);
        Console.WriteLine($"Знайдено {managers.Count} менеджер(ів) (використання Анонімного методу).");

        // Демонстрація LINQ Aggregate (Лямбда-вираз для сумування)
        decimal totalPayroll = employees.Sum(e => e.Salary);
        Console.WriteLine($"Загальний фонд заробітної плати (LINQ Aggregate/Sum): {totalPayroll:C}");
        
        Console.WriteLine("==================================================");
    }
}