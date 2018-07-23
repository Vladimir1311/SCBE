using System;
namespace SituationCenterCore.Extensions
{
    public static class Objectextensions
    {
        public static T With<T>(this T value, Action<T> action)
        {
            action(value);
            return value;
        }
    }
}
