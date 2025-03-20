namespace CipherLib.Infrastructure;

public class ListEqualityComparer: IEqualityComparer<List<int>>
{
    public bool Equals(List<int>? x, List<int>? y)
    {
        if (x == null || y == null) return false;
        if (x.Count != y.Count) return false;

        for (int i = 0; i < x.Count; i++)
        {
            if (x[i] != y[i]) return false;
        }
        return true;
    }

    public int GetHashCode(List<int> obj)
    {
        int hash = 17;
        foreach (var item in obj)
        {
            hash = hash * 31 + item.GetHashCode();
        }
        return hash;
    }
}