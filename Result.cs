using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeliosBot
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Payload { get; set; }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
