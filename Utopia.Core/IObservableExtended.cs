namespace Utopia.Core;

public interface IObservableExtended<out T> : IObservable<T>
{
    /// <summary>
    ///     上一次推送的值.即最新值.
    /// </summary>
    T LastestValue { get; }
}
