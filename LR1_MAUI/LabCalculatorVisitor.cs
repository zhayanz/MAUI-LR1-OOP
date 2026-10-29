using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_MAUI
{
    class LabCalculatorVisitor : LabCalculatorBaseVisitor<double>
    {
        private readonly Dictionary<string, Cell> _cells;
        private readonly HashSet<string> _visited;

        public LabCalculatorVisitor(Dictionary<string, Cell> cells, HashSet<string> visited)
        {
            _cells = cells;
            _visited = visited;
        }

        public override double VisitCompileUnit(LabCalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(LabCalculatorParser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText(), CultureInfo.InvariantCulture);
            Debug.WriteLine(result);
            return result;
        }

        //IdentifierExpr
        public override double VisitIdentifierExpr(LabCalculatorParser.IdentifierExprContext context)
        {
            var result = context.GetText().ToUpper();
            //видобути значення змінної з таблиці
            if (_cells.ContainsKey(result))
            {
                if (_visited.Contains(result))
                    throw new InvalidOperationException("Циклічне посилання");

                _visited.Add(result);
                try
                {
                    var expression = _cells[result].Expression;
                    if (string.IsNullOrWhiteSpace(expression))
                        return 0.0;

                    return Calculator.Evaluate(expression, _cells, _visited);
                }
                finally
                {
                    _visited.Remove(result);
                }
            }
            else
            {
                return 0.0;
            }
        }

        public override double VisitParenthesizedExpr(LabCalculatorParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitExponentialExpr(LabCalculatorParser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} ^ {1}", left, right);
            return System.Math.Pow(left, right);
        }

        public override double VisitAdditiveExpr(LabCalculatorParser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == LabCalculatorLexer.ADD)
            {
                Debug.WriteLine("{0} + {1}", left, right);
                return left + right;
            }
            else //LabCalculatorLexer.SUBTRACT
            {
                Debug.WriteLine("{0} - {1}", left, right);
                return left - right;
            }
        }

        public override double VisitMultiplicativeExpr(LabCalculatorParser.MultiplicativeExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == LabCalculatorLexer.MULTIPLY)
            {
                Debug.WriteLine("{0} * {1}", left, right);
                return left * right;
            }
            else //LabCalculatorLexer.DIVIDE
            {
                Debug.WriteLine("{0} / {1}", left, right);
                return left / right;
            }
        }

        public override double VisitMaxExpr(LabCalculatorParser.MaxExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (left > right)
            {
                return left;
            }
            return right;
        }

        public override double VisitMinExpr(LabCalculatorParser.MinExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (left < right)
            {
                return left;
            }
            return right;
        }

        public override double VisitIncExpr(LabCalculatorParser.IncExprContext context)
        {
            var value = Visit(context.expression());
            return value + 1;
        }

        public override double VisitDecExpr(LabCalculatorParser.DecExprContext context)
        {
            var value = Visit(context.expression());
            return value - 1;
        }

        private double WalkLeft(LabCalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<LabCalculatorParser.ExpressionContext>(0));
        }

        private double WalkRight(LabCalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<LabCalculatorParser.ExpressionContext>(1));
        }
    }
}
