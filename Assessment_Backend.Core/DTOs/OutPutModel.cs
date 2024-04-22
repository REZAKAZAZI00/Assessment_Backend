namespace Assessment_Backend.Core.DTOs
{
    public class OutPutModel<T>
    {
        public T? Result { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }
}
