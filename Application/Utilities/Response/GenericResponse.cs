namespace Application.Utilities.Response
{
    public class GenericResponse<T>
    {
        public GenericResponse()
        {
            ValidationErrors = new List<string>();
        }

        public GenericResponse(bool success) : this()
        {
            IsSuccess = success;
        }

        public GenericResponse(bool success, T? data) : this(success)
        {
            Data = data;
        }

        public GenericResponse(bool success, T? data, string message) : this(success, data)
        {
            Message = message;
        }

        public GenericResponse(bool success, string message) : this(success)
        {
            Message = message;
        }

        public GenericResponse(bool success, string message, int statusCode) : this(success, message)
        {
            StatusCode = statusCode;
        }

        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; } = 200;
        public List<string> ValidationErrors { get; set; }

        // Static factory methods for easier usage
        public static GenericResponse<T> Success(T data, string message = "")
        {
            return new GenericResponse<T>(true, data, message);
        }

        public static GenericResponse<T> Success(string message)
        {
            return new GenericResponse<T>(true, message);
        }

        public static GenericResponse<T> Fail(string message, int statusCode = 400)
        {
            return new GenericResponse<T>(false, message, statusCode);
        }

        public static GenericResponse<T> Fail(T? data, string message, int statusCode = 400)
        {
            return new GenericResponse<T>(false, data, message) { StatusCode = statusCode };
        }

        public static GenericResponse<T> Fail(List<string> validationErrors, string message = "Validation failed", int statusCode = 400)
        {
            return new GenericResponse<T>(false, message, statusCode)
            {
                ValidationErrors = validationErrors
            };
        }
    }
}