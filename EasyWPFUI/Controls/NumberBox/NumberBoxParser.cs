// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EasyWPFUI.Controls.Primitives;

namespace EasyWPFUI.Controls
{
    internal enum MathTokenType
    {
        Numeric,
        Operator,
        Parenthesis,
    }

    internal struct MathToken
    {
        public MathTokenType Type;
        public char Char;
        public double Value;

        public MathToken(MathTokenType t, char c)
        {
            Type = t;
            Char = c;
            Value = double.NaN;
        }

        public MathToken(MathTokenType t, double d)
        {
            Type = t;
            Char = char.MinValue;
            Value = d;
        }
    }

    internal class NumberBoxParser
    {
        private const string c_numberBoxOperators = "+-*/^";

        public static double? Compute(string expr, INumberBoxFormatter numberParser)
        {
            // Tokenize the input string
            List<MathToken> tokens = GetTokens(expr, numberParser);

            if (tokens.Count > 0)
            {
                // Rearrange to postfix notation
                List<MathToken> postfixTokens = ConvertInfixToPostfix(tokens);

                if (postfixTokens.Count > 0)
                {
                    // Compute expression
                    return ComputePostfixExpression(postfixTokens);
                }
            }

            return null;
        }

        private static List<MathToken> GetTokens(string input, INumberBoxFormatter numberParser)
        {
            List<MathToken> tokens = new List<MathToken>();

            bool expectNumber = true;

            while (input.Length > 0)
            {
                // Skip spaces
                char nextChar = input[0];

                if (nextChar != ' ')
                {
                    if (expectNumber)
                    {
                        if (nextChar == '(')
                        {
                            // Open parens are also acceptable, but don't change the next expected token type.
                            tokens.Add(new MathToken(MathTokenType.Parenthesis, nextChar));
                        }
                        else
                        {
                            var (value, charLength) = GetNextNumber(input, numberParser);

                            if (charLength > 0)
                            {
                                tokens.Add(new MathToken(MathTokenType.Numeric, value));
                                input = input.Substring(charLength - 1); // advance the end of the token
                                expectNumber = false; // next token should be an operator
                            }
                            else
                            {
                                // Error case -- next token is not a number
                                return new List<MathToken>();
                            }
                        }
                    }
                    else
                    {
                        if (c_numberBoxOperators.IndexOf(nextChar) >= 0)
                        {
                            tokens.Add(new MathToken(MathTokenType.Operator, nextChar));
                            expectNumber = true; // next token should be a number
                        }
                        else if (nextChar == ')')
                        {
                            // Closed parens are also acceptable, but don't change the next expected token type.
                            tokens.Add(new MathToken(MathTokenType.Parenthesis, nextChar));
                        }
                        else
                        {
                            return new List<MathToken>();
                        }
                    }
                }

                input = input.Substring(1);
            }

            return tokens;
        }

        // Attempts to parse a number from the beginning of the given input string. Returns the character size of the matched string.
        private static Tuple<double, int> GetNextNumber(string input, INumberBoxFormatter numberParser)
        {
            // Attempt to parse anything before an operator or space as a number
            Regex regex = new Regex("^-?([^-+/*\\(\\)\\^\\s]+)");
            Match match = regex.Match(input);

            if (match.Success)
            {
                // Might be a number
                int matchLength = match.Value.Length;
                double? parsedNum = numberParser.Parse(input.Substring(0, matchLength));

                if (parsedNum != null)
                {
                    // Parsing was successful
                    return new Tuple<double, int>(parsedNum.Value, matchLength);
                }
            }

            return new Tuple<double, int>(double.NaN, 0);
        }

        private static int GetPrecedenceValue(char c)
        {
            int opPrecedence = 0;
            if (c == '*' || c == '/')
            {
                opPrecedence = 1;
            }
            else if (c == '^')
            {
                opPrecedence = 2;
            }

            return opPrecedence;
        }

        // Converts a list of tokens from infix format (e.g. "3 + 5") to postfix (e.g. "3 5 +")
        private static List<MathToken> ConvertInfixToPostfix(List<MathToken> infixTokens)
        {
            List<MathToken> postfixTokens = new List<MathToken>();
            Stack<MathToken> operatorStack = new Stack<MathToken>();

            foreach (MathToken token in infixTokens)
            {
                if (token.Type == MathTokenType.Numeric)
                {
                    postfixTokens.Add(token);
                }
                else if (token.Type == MathTokenType.Operator)
                {
                    while (operatorStack.Count > 0)
                    {
                        MathToken top = operatorStack.Peek();
                        if (top.Type != MathTokenType.Parenthesis && (GetPrecedenceValue(top.Char) >= GetPrecedenceValue(token.Char)))
                        {
                            postfixTokens.Add(operatorStack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }

                    operatorStack.Push(token);
                }
                else if (token.Type == MathTokenType.Parenthesis)
                {
                    if (token.Char == '(')
                    {
                        operatorStack.Push(token);
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek().Char != '(')
                        {
                            // Pop operators onto output until we reach a left paren
                            postfixTokens.Add(operatorStack.Pop());
                        }

                        if (operatorStack.Count > 0)
                        {
                            // Broken parenthesis
                            return new List<MathToken>();
                        }

                        // Pop left paren and discard
                        operatorStack.Pop();
                    }
                }
            }

            // Pop all remaining operators.
            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek().Type == MathTokenType.Parenthesis)
                {
                    // Broken parenthesis
                    return new List<MathToken>();
                }

                postfixTokens.Add(operatorStack.Pop());
            }

            return postfixTokens;
        }

        private static double? ComputePostfixExpression(List<MathToken> tokens)
        {
            Stack<double> stack = new Stack<double>();

            foreach(MathToken token in tokens)
            {
                if (token.Type == MathTokenType.Operator)
                {
                    // There has to be at least two values on the stack to apply
                    if (stack.Count < 2)
                    {
                        return null;
                    }

                    double op1 = stack.Pop();

                    double op2 = stack.Pop();

                    double result;

                    switch (token.Char)
                    {
                        case '-':
                            {
                                result = op2 - op1;
                                break;
                            }
                        case '+':
                            {
                                result = op1 + op2;
                                break;
                            }
                        case '*':
                            {
                                result = op1 * op2;
                                break;
                            }
                        case '/':
                            {
                                if (op1 == 0)
                                {
                                    throw new DivideByZeroException();
                                }
                                else
                                {
                                    result = op2 / op1;
                                }
                                break;
                            }
                        case '^':
                            {
                                result = Math.Pow(op2, op1);
                                break;
                            }
                        default:
                            {
                                return null;
                            }
                    }

                    stack.Push(result);
                }
                else if(token.Type == MathTokenType.Numeric)
                {
                    stack.Push(token.Value);
                }
            }

            // If there is more than one number on the stack, we didn't have enough operations, which is also an error.
            if (stack.Count != 1)
            {
                return null;
            }

            return stack.Peek();
        }
    }
}
