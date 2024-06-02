namespace MikesPaging.AspNetCore;

public class MappingResult
{
    public bool IsFailure => !IsSuccess;

    public bool IsSuccess { get; }

    public IReadOnlyCollection<MappingError> Errors { get; }

    protected internal MappingResult(bool isSuccess, MappingError error)
    {
        if (isSuccess && error != MappingError.None || !isSuccess && error == MappingError.None)
        {
            throw new InvalidOperationException("Unable create result object.");
        }

        IsSuccess = isSuccess;
        Errors = isSuccess ? Array.Empty<MappingError>() : [ error ];
    }

    protected internal MappingResult(bool isSuccess, IEnumerable<MappingError> errors)
    {
        if (isSuccess && errors.Any()
            || !isSuccess && errors.Distinct().Count() != errors.Count()
            || !isSuccess && !errors.Any()
            || !isSuccess && errors.Contains(MappingError.None))
        {
            throw new InvalidOperationException("Unable create result object.");
        }

        IsSuccess = isSuccess;
        Errors = isSuccess ? Array.Empty<MappingError>() : new List<MappingError>(errors);
    }

    public static MappingResult Success() => new(true, MappingError.None);

    public static MappingResult<T> Success<T>(T value) => new(value, true, MappingError.None);

    public static MappingResult Failure(MappingError error) => new(false, error);

    public static MappingResult<T> Failure<T>(MappingError error) => new(default, false, error);

    public static MappingResult Failure(IEnumerable<MappingError> errors) => new(false, errors);

    public static MappingResult<T> Failure<T>(IEnumerable<MappingError> errors) => new(default, false, errors);
}

public class MappingResult<TValue> : MappingResult
{
    private readonly TValue? _value;

    protected internal MappingResult(TValue? value, bool isSuccess, MappingError error) : base(isSuccess, error)
        => _value = value;

    protected internal MappingResult(TValue? value, bool isSuccess, IEnumerable<MappingError> errors) : base(isSuccess, errors)
        => _value = value;

    public TValue Value => IsFailure ?
        throw new InvalidOperationException("The value of failed result can't be accessed.")
        :
        _value!;

    public static implicit operator MappingResult<TValue>(TValue value) => new(value, true, MappingError.None);
}

public sealed record MappingError
{
    public static readonly MappingError None = new(string.Empty, string.Empty);
    public string Code { get; }
    public string Text { get; }

    public MappingError(string code, string text)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(text);
        Text = text;
        Code = code;
    }

    public static implicit operator MappingError((string code, string message) value) => new(value.code, value.message);
}