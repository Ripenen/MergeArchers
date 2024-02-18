public interface IDataProvider<out T>
{
    public T Data { get; }
}
