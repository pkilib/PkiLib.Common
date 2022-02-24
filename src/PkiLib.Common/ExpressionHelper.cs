using System;
using System.Linq.Expressions;

namespace Org.PkiLib.Common
{
    public static class ExpressionHelper
    {
        public static Expression<Func<TSource, TConvertedResult>> ConvertResult<TSource, TResult, TConvertedResult>(Expression<Func<TSource, TResult>> expression)
        {
            return Expression.Lambda<Func<TSource, TConvertedResult>>(Expression.Convert(expression.Body, typeof(TConvertedResult)), expression.Parameters);
        }
    }
}
