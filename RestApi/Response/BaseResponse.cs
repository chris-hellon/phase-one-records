using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.Response
{
    public class BaseResponse : IBaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Json { get; set; }
    }
}
