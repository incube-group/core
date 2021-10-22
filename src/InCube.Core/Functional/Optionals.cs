namespace InCube.Core.Functional
{
    /// <summary>
    /// A collection of additional helper methods to work with <see cref="Maybe{T}" />s, <see cref="Option{T}" />s, etc...
    /// Useful for update DTOs.
    /// </summary>
    public static class Optionals
    {
        /// <summary>
        /// Explicitly set to null.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <returns>An optional.</returns>
        public static Option<Maybe<T>> None<T>()
            where T : class => Maybe<T>.None;

        /// <summary>
        /// Convert nullable to None if null, or some if not null.
        /// </summary>
        /// <typeparam name="T">The optional reference type.</typeparam>
        /// <param name="self">The reference type object.</param>
        /// <returns>An option of maybe of the reference type.</returns>
        public static Option<Maybe<T>> Optionally<T>(this T? self)
            where T : class =>
            self is null ? Option<Maybe<T>>.None : Option.Some(self.ToMaybe());

        /// <summary>
        /// Optionally sets a reference type to the content of optional value.
        /// </summary>
        /// <typeparam name="T">The optional reference type.</typeparam>
        /// <param name="optional">The optional.</param>
        /// <param name="target">The target to replace if there is a value.</param>
        public static void SetOptionally<T>(this Option<Maybe<T>> optional, ref T? target)
            where T : class
        {
            foreach (var mt in optional)
                target = mt.GetValueOrDefault();
        }
    }
}