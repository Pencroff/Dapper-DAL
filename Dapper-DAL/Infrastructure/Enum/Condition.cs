using System;

namespace Dapper_DAL.Infrastructure.Enum
{
    public enum Condition
    {
        IsNull,
        IsNotNull,
        Equal,
        NotEqual,
        Less,
        Greater,
        LessEqual,
        GreaterEqual,
        Between,
        Like,
        In
    }

    public static class ConditionExtension
    {
        public static string GetString(this Condition e)
        {
            var result = string.Empty;
            switch (e)
            {
                case Condition.IsNull:
                    return "IS NULL";
                    break;
                case Condition.IsNotNull:
                    return "IS NOT NULL";
                    break;
                case Condition.Equal:
                    return "=";
                    break;
                case Condition.NotEqual:
                    return "<>";
                    break;
                case Condition.Less:
                    return "<";
                    break;
                case Condition.Greater:
                    return ">";
                    break;
                case Condition.LessEqual:
                    return "<=";
                    break;
                case Condition.GreaterEqual:
                    return ">=";
                    break;
                case Condition.Between:
                    return "BETWEEN";
                    break;
                case Condition.Like:
                    return "LIKE";
                    break;
                case Condition.In:
                    return "IN";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("e");
            }
            return result;
        }
    }
}