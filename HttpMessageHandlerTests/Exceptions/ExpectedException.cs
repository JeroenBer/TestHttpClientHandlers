using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageHandlerTests.cs.Exceptions
{
    public class ExpectedException : Exception
    {
        public ExpectedException() : base("Expected exception but did not happen")
        {
        }
    }
}
