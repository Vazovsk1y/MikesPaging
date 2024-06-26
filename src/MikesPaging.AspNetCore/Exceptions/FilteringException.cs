﻿using System.Diagnostics.CodeAnalysis;
using MikesPaging.AspNetCore.Exceptions.Base;

namespace MikesPaging.AspNetCore.Exceptions;

public class FilteringException : MikesPagingException
{
    internal FilteringException(string? message) : base(message)
    {
    }

    internal FilteringException(string? message, Exception? inner) : base(message, inner)
    {

    }
    internal new static void ThrowIf([DoesNotReturnIf(true)]bool condition, string? message)
    {
        if (condition)
        {
            throw new FilteringException(message);
        }
    }

    internal new static void ThrowIf([DoesNotReturnIf(true)]bool condition, string? message, Exception? inner)
    {
        if (condition)
        {
            throw new FilteringException(message, inner);
        }
    }
}