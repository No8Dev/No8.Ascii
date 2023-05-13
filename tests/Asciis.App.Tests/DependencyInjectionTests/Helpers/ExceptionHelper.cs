﻿using System;

namespace Asciis.App.Tests.DependencyInjectionTests.Helpers;

public static class ExceptionHelper
{
    public static Exception? Record(Action action)
    {
        try
        {
            action.Invoke();

            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}