using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.Response
{
    public interface IBaseResponse
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}
