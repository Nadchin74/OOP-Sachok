public class Figure
{
    private double _area;

    public double Area
    {
        get { return _area; }
        set { _area = value; }
    }

    public Figure(double area)
    {
        _area = area;
        Console.WriteLine("Figure created");
    }

    ~Figure()
    {
        Console.WriteLine("Figure destroyed");
    }

    public void GetFigure()
    {
        Console.WriteLine($"Figure with area: {Area}");
    }
}