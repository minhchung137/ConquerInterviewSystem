using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.Commons
{
    public static class ErrorMessages

    {
        private static readonly Dictionary<AppErrorCode, string> _messages = new()
        {
            { AppErrorCode.UserAlreadyExists, "User already exists with same email or username" },
            { AppErrorCode.InvalidCredentials, "Invalid credentials, please check email/username and password or account has been disabled" },
            { AppErrorCode.UnauthorizedAccess, "You are not authorized to perform this action" },
            { AppErrorCode.InvalidInput, "The provided input data is invalid" },
            { AppErrorCode.MissingRequiredField, "Some required fields are missing" },
            { AppErrorCode.SessionNotFound, "Interview session not found" },
            { AppErrorCode.QuestionNotFound, "Interview question not found" },
            { AppErrorCode.InternalError, "An unexpected error occurred" },
            { AppErrorCode.UserNotFound, "User not found"   },
            { AppErrorCode.ListIsEmpty, "The list is empty" },
            { AppErrorCode.UserAlreadyDeleted, "User is already deleted" }, 
            { AppErrorCode.UserUpdateFailed, "Failed to update user" }
        };

        public static string GetMessage(AppErrorCode code) =>
            _messages.TryGetValue(code, out var message) ? message : "Unknown error";
    }
}
