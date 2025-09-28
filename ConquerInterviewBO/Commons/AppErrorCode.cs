using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.Commons
{
    public enum AppErrorCode
    {
        // Auth
        UserAlreadyExists = 1001,
        InvalidCredentials = 1002,
        UnauthorizedAccess = 1003,
        UserNotFound = 1004,
        ListIsEmpty = 1005,
        UserAlreadyDeleted = 1006,
        UserUpdateFailed = 1007,

        // Validation
        InvalidInput = 2001,
        MissingRequiredField = 2002,

        // Interview
        SessionNotFound = 3001,
        QuestionNotFound = 3002,

        // General
        InternalError = 9000
    }
}
