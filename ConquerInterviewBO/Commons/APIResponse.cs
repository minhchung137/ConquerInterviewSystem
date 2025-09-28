using ConquerInterviewBO.Commons;

namespace ConquerInterviewBO.Common
{
    public class APIResponse<T>
    {
        public int StatusCode { get; set; }
        public AppErrorCode? ErrorCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static APIResponse<T> Success(T? data, string message = "Success", ResponseStatus status = ResponseStatus.Success)
        {
            return new APIResponse<T>
            {
                StatusCode = (int)status,
                Message = message,
                Data = data
            };
        }

        public static APIResponse<T> Fail(AppErrorCode errorCode, ResponseStatus status = ResponseStatus.BadRequest)
        {
            return new APIResponse<T>
            {
                StatusCode = (int)status,
                ErrorCode = errorCode,
                Message = ErrorMessages.GetMessage(errorCode),
                Data = default
            };
        }
    }
}
