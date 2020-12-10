namespace InCube.Core.Numerics
{
    /// <summary>
    /// A covariant range type.
    /// </summary>
    /// <typeparam name="T">Type of the range.</typeparam>
    public interface IRange<out T> : IHasMin<T>, IHasMax<T>
    {
    }
}