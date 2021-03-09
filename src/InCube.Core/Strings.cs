namespace InCube.Core
{
    /// <summary>
    /// A collection of extension methods aimed at <see cref="string"/>s.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Returns the <paramref name="alternative"/> in case <paramref name="self"/> is null or white space.
        /// </summary>
        /// <param name="self">The string to replace with an alternative in case it is null or white space.</param>
        /// <param name="alternative">The alternative to use as replacement in case <paramref name="self"/> is null or white space.</param>
        /// <returns>A nullable <see cref="string"/>.</returns>
        public static string? OrElseIfNullOrWhiteSpace(this string? @self, string? alternative) => string.IsNullOrWhiteSpace(@self) ? alternative : @self;
    }
}