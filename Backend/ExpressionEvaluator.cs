using System.Globalization;

namespace Backend;

public static class ExpressionEvaluator
{
    public static double Evalute(string infix) => EvalutePostfix(ToPostfix(infix));

    private static string ToPostfix(string infix)
    {
        var posfix = string.Empty;
        var stack = new Stack<char>();
        foreach (var item in infix)
        {
            if (item == ' ') continue;

            if (IsOperator(item))
            {
                posfix += ' '; // cierra el número que se estaba acumulando

                if (item == ')')
                {
                    var ope = stack.Pop();
                    while (ope != '(')
                    {
                        posfix += ope;
                        posfix += ' ';
                        ope = stack.Pop();
                    }
                }
                else
                {
                    // Desapila mientras la prioridad de la pila sea mayor o igual
                    while (stack.Count > 0 && PriorityInfix(item) <= PriorityStack(stack.Peek()))
                    {
                        posfix += stack.Pop();
                        posfix += ' ';
                    }
                    stack.Push(item);
                }
            }
            else
            {
                // Dígitos y punto decimal se acumulan (la coma se convierte en punto)
                posfix += item == ',' ? '.' : item;
            }
        }

        posfix += ' ';
        while (stack.Count > 0)
        {
            posfix += stack.Pop();
            posfix += ' ';
        }
        return posfix;
    }

    private static int PriorityStack(char op) => op switch
    {
        '^' => 3,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 0,
        _ => throw new Exception("Invalid expression."),
    };

    private static int PriorityInfix(char op) => op switch
    {
        '^' => 4,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 5,
        _ => throw new Exception("Invalid expression."),
    };

    private static bool IsOperator(char item) =>
        item == '^' || item == '*' || item == '/' || item == '+' || item == '-' || item == '(' || item == ')';

    private static double EvalutePostfix(string postfix)
    {
        var stack = new Stack<double>();
        foreach (var token in postfix.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (token.Length == 1 && IsOperator(token[0]))
            {
                var ope2 = stack.Pop();
                var ope1 = stack.Pop();
                stack.Push(Calculate(ope1, ope2, token[0]));
            }
            else
            {
                stack.Push(double.Parse(token, CultureInfo.InvariantCulture));
            }
        }
        return stack.Pop();
    }

    private static double Calculate(double ope1, double ope2, char item) => item switch
    {
        '*' => ope1 * ope2,
        '/' => ope1 / ope2,
        '+' => ope1 + ope2,
        '-' => ope1 - ope2,
        '^' => Math.Pow(ope1, ope2),
        _ => throw new Exception("Invalid expression."),
    };

    public static object Evaluate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        // Use the existing evaluator (Evalute) which returns a double.
        // Normalize whitespace and decimal separator (comma -> dot) to match Evalute parsing.
        var normalized = text.Trim().Replace(',', '.');

        // Let any parsing/runtime exceptions propagate so callers can handle them.
        return Evalute(normalized);
    }
}

