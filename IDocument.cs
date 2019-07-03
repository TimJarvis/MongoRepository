namespace MongoRepository
{
    public interface IDocument<T>
    {
        T Id { get; set; }
    }
}