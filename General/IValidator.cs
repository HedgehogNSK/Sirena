namespace Hedgey.Sirena.Bot;

public interface IValidator<T>
{
    /// <summary>
    /// Determines whether the provided object is valid according to specific criteria.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <returns>True if the object is valid; otherwise, false.</returns>
    bool IsValid(T obj);
}