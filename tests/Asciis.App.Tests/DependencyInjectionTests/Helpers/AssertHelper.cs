using System;

namespace Asciis.App.Tests.DependencyInjectionTests.Helpers;

internal class AssertHelper
{
    internal static TException ThrowsException<TException>(Action callback)
        where TException : Exception
    {
        try
        {
            callback();
            throw new InvalidOperationException("No exception was thrown.");
        }
        catch (Exception ex)
        {
            if (ex is TException exception)
                return exception;
            else
                throw new InvalidOperationException(
                    $"Exception of type '{typeof(TException)}' expected, but got exception of type '{ex.GetType()}'.", ex);
        }
    }
}