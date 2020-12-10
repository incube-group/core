namespace InCube.Core.Numerics
{
    /// <summary>
    /// Interface of an object that has a minimum.
    /// </summary>
    /// <typeparam name="T">Type of minimum.</typeparam>
    public interface IHasMin<out T>
    {
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        T Min { get; }
    }
}