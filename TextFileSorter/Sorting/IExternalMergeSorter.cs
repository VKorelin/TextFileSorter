namespace TextFileSorter.Sorting
{
    public interface IExternalMergeSorter
    {
        bool Sort(string fileName);
    }
}