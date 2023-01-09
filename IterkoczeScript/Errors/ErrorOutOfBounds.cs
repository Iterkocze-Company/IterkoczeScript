using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IterkoczeScript.Errors {
    internal class ErrorOutOfBounds : IError {
        public string Message => "Index was out of bounds.";
    } 
}
