using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_MAUI
{
    class ThrowExceptionErrorListener : BaseErrorListener, IAntlrErrorListener<int>
    {
        //BaseErrorListener implementation
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ArgumentException("Помилка виразу в рядку " + line + ", позиція " + (charPositionInLine + 1), e);
        }

        //IAntlrErrorListener<int> implementation
        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ArgumentException("Помилка виразу в рядку " + line + ", позиція " + (charPositionInLine + 1), e);
        }
    }
}
