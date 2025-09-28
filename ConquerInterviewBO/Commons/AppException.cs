using System;
using ConquerInterviewBO.Commons;

namespace ConquerInterviewBO.Common
{
    public class AppException : Exception
    {
        public AppErrorCode ErrorCode { get; }

        public AppException(AppErrorCode errorCode)
            : base(ErrorMessages.GetMessage(errorCode))
        {
            ErrorCode = errorCode;
        }
    }
}
