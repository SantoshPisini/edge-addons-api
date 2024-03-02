namespace SantoshPisini.EdgeAddonsAPI.Exceptions
{
    public class EdgeClientException : Exception
    {
        public EdgeClientException()
        {
        }

        public EdgeClientException(string message) : base(message)
        {
        }

        public EdgeClientException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
