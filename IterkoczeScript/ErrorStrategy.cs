using Antlr4.Runtime.Sharpen;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace IterkoczeScript
{
    class ErrorStrategy : IAntlrErrorStrategy
    {
        public void ReportError(Parser recognizer, RecognitionException e)
        {
            // Custom error handling logic goes here
        }

        public void Reset(Parser recognizer)
        {
            // Reset error state
        }

        public void Recover(Parser recognizer, RecognitionException e)
        {
            // Attempt to recover from an error by consuming extra tokens
        }

        public IToken RecoverFromMismatchedToken(Parser recognizer, IToken t, int ttype, BitSet follow)
        {
            // Attempt to recover from a mismatched token error
            return null;
        }

        IToken IAntlrErrorStrategy.RecoverInline(Parser recognizer)
        {
            throw new NotImplementedException();
        }

        public void Sync([NotNull] Parser recognizer)
        {
            //Console.WriteLine(recognizer.Context.start.Text);
            //Environment.Exit(-1);
            //throw new NotImplementedException();
        }

        public bool InErrorRecoveryMode([NotNull] Parser recognizer)
        {
            throw new NotImplementedException();
        }

        public void ReportMatch([NotNull] Parser recognizer)
        {
            //Console.WriteLine(recognizer.Context.start.Text);
            throw new NotImplementedException();
        }
    }
}
