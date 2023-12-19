namespace Nanoray.Shrike;

/// <summary>
/// Stores a reference to another object.
/// </summary>
/// <typeparam name="T">The object type.</typeparam>
public sealed class ObjectRef<T> where T : class
{
    /// <summary>
    /// The currently referenced object.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectRef{T}"/> type.
    /// </summary>
    /// <param name="initialValue">The initially referenced object.</param>
    public ObjectRef(T initialValue)
    {
        this.Value = initialValue;
    }

    /// <summary>
    /// Implicitly boxes an object.
    /// </summary>
    /// <param name="value">The object to box.</param>
    public static implicit operator ObjectRef<T>(T value)
        => new(value);

    /// <summary>
    /// Implicitly unboxes the held object.
    /// </summary>
    /// <param name="value">The reference to unbox.</param>
    public static implicit operator T(ObjectRef<T> value)
        => value.Value;
}

/// <summary>
/// Stores a (nullable) reference to another object.
/// </summary>
/// <typeparam name="T">The object type.</typeparam>
public sealed class NullableObjectRef<T> where T : class
{
    /// <summary>
    /// The currently referenced object.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableObjectRef{T}"/> type.
    /// </summary>
    /// <param name="initialValue">The initially referenced object.</param>
    public NullableObjectRef(T? initialValue = null)
    {
        this.Value = initialValue;
    }

    /// <summary>
    /// Implicitly boxes an object.
    /// </summary>
    /// <param name="value">The object to box.</param>
    public static implicit operator NullableObjectRef<T>(T? value)
        => new(value);

    /// <summary>
    /// Implicitly unboxes the held object.
    /// </summary>
    /// <param name="value">The reference to unbox.</param>
    public static implicit operator T?(NullableObjectRef<T> value)
        => value.Value;
}

/// <summary>
/// Stores a value as a reference type.
/// </summary>
/// <typeparam name="T">The object type.</typeparam>
public sealed class StructRef<T> where T : struct
{
    /// <summary>
    /// The current value.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StructRef{T}"/> type.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    public StructRef(T initialValue)
    {
        this.Value = initialValue;
    }

    /// <summary>
    /// Implicitly boxes a value.
    /// </summary>
    /// <param name="value">The value to box.</param>
    public static implicit operator StructRef<T>(T value)
        => new(value);

    /// <summary>
    /// Implicitly unboxes the held value.
    /// </summary>
    /// <param name="value">The reference to unbox.</param>
    public static implicit operator T(StructRef<T> value)
        => value.Value;
}

/// <summary>
/// Stores a (nullable) value as a reference type.
/// </summary>
/// <typeparam name="T">The object type.</typeparam>
public sealed class NullableStructRef<T> where T : struct
{
    /// <summary>
    /// The current value.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableStructRef{T}"/> type.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    public NullableStructRef(T? initialValue = null)
    {
        this.Value = initialValue;
    }

    /// <summary>
    /// Implicitly boxes a value.
    /// </summary>
    /// <param name="value">The value to box.</param>
    public static implicit operator NullableStructRef<T>(T? value)
        => new(value);

    /// <summary>
    /// Implicitly unboxes the held value.
    /// </summary>
    /// <param name="value">The reference to unbox.</param>
    public static implicit operator T?(NullableStructRef<T> value)
        => value.Value;
}
