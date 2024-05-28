public class BusinessExceptiona : Exception
{
    public string Type { get; }

    public BusinessExceptiona(string type, string message) : base(message)
    {
        Type = type;
    }
}