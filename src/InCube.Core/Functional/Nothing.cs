namespace InCube.Core.Functional
{
    /// <summary>
    /// Represents the Bottom type, i.e., the inner type of <see cref="Option.None"/> and <see cref="Maybes.None"/>.
    /// </summary>
    // ReSharper disable once ConvertToStaticClass
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Nothing
    {
        private Nothing()
        {
            // no instantiation
        }
    }
}