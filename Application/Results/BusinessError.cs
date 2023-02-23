namespace Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public record BusinessError(string Code, string Message, BusinessError.Type ErrorType = BusinessError.Type.BadRequest) : Error(Code, Message)
{
    public static BusinessError BadRequest(string code, string message) => new BusinessError(code, message, BusinessError.Type.BadRequest);
    public static BusinessError Conflict(string code, string message) => new BusinessError(code, message, BusinessError.Type.Conflict);
    public static BusinessError Unexpected(string code, string message) => new BusinessError(code, message, BusinessError.Type.Unexpected);

    public enum Type
    {
        BadRequest = 1,
        Conflict = 2,
        Unexpected = 3
    }
}
