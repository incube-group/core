namespace InCube.Core.Numerics
{
    /// <summary>
    /// Interface of an object that has a maximum.
    /// </summary>
    /// <typeparam name="T">Type of maximum.</typeparam>
    public interface IHasMax<out T>
    {
        /// <summary>
        /// Gets the maximum.
        /// </summary>
        T Max { get; }
    }
}