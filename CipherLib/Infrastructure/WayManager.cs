namespace CipherLib.Infrastructure;

public class WayManager
{
    private Dictionary<List<int>, int> _wayDictionary;

    public WayManager()
    {
        _wayDictionary = new Dictionary<List<int>, int>(new ListEqualityComparer());
        InitializeDefaultWays();
    }

    private void InitializeDefaultWays()
    {
        AddWay(new List<int> { 1, 2, 16 }, 1); // Путь 1
        AddWay(new List<int> { 1, 2, 3, 4, 13, 14, 15, 2, 16 }, 2); // Путь 2
        AddWay(new List<int> { 1, 2, 3, 4, 5, 6,8, 9, 11, 12, 14, 15, 2, 16 }, 3); // Путь 3
        AddWay(new List<int> { 1, 2, 3, 4, 5, 6,8, 9, 10, 12, 14, 15, 2, 16 }, 4); // Путь 4
        AddWay(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 15, 2, 16 }, 5); // Путь 5 (шифрование)
        AddWay(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 14, 15, 2, 16 }, 5); // Путь 5 (дешифрование)
    }

    private void AddWay(List<int> way, int pathNumber)
    {
        if (!_wayDictionary.ContainsKey(way))
        {
            _wayDictionary[way] = pathNumber;
        }
    }

    public int GetWayNumber(List<int> way)
    {
        if (_wayDictionary.TryGetValue(way, out int pathNumber))
        {
            return pathNumber;
        }
        return -1; // Путь не найден
    }

    public void OutputWay(List<int> way)
    {
        int wayNumber = GetWayNumber(way);
        if (wayNumber != -1)
        {
            Console.Write($"Путь {wayNumber} : ");
        }
        else
        {
            Console.WriteLine("Путь не найден");
        }
    }
}