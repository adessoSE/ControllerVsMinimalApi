namespace Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public record Error(string Code, string Message)
{
    public static Error NotFound { get; } = new Error("not_found", "The requested resource was not found");
    public static Error Unauthorized { get; } = new Error("unauthorized", "Unauthorized access");
}
