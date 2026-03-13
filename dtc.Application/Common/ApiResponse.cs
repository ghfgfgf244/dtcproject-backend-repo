namespace dtc.Application.Common
{
    /// <summary>
    /// Unified API Response wrapper for all endpoints.
    /// Frontend should always check 'success' first.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? Message { get; private set; }
        public IEnumerable<string> Errors { get; private set; } = [];

        private ApiResponse() { }

        public static ApiResponse<T> Ok(T data, string? message = null) => new()
        {
            Success = true,
            Data = data,
            Message = message
        };

        public static ApiResponse<T> Created(T data, string? message = "Created successfully.") => new()
        {
            Success = true,
            Data = data,
            Message = message
        };

        public static ApiResponse<T> Fail(string error) => new()
        {
            Success = false,
            Errors = [error]
        };

        public static ApiResponse<T> Fail(IEnumerable<string> errors) => new()
        {
            Success = false,
            Errors = errors
        };

        public static ApiResponse<T> NotFound(string resource = "Resource") => new()
        {
            Success = false,
            Errors = [$"{resource} was not found."]
        };

        public static ApiResponse<T> NoContent(string? message = null) => new()
        {
            Success = true,
            Message = message
        };
    }
}
