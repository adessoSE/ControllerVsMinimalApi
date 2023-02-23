namespace Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed record UnexpectedError(Exception Exception) 
    : Error("unexpected", "An unexpected error occurred");
