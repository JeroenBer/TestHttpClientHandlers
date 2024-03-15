using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageHandlerTests.cs.Exceptions
{
    public class UnexpectedException : Exception
    {
        public UnexpectedException(Exception innerException) : base("Unexpected exception", innerException)
        {
        }
    }
}
