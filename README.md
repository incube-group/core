# InCube Core

[InCube] Core is a set of C# libraries that provides extensions for standard
collection types, functional programming, value parsing and formatting and more.

## Functional Programming

The [Functional] package of [InCube] Core provides read-only wrapper types in C#
that are common in other functional programming languages.

| Type | Description |
| --- | --- |
| `Option<T>` | A wrapper designed along the lines of [`Nullable<T>`] that supports *struct and reference types*. |
| `Maybe<T>` | An optimized version of `Option<T>` that supports *reference types* only. |
| `Either<TL, T>` | A union type of either `TL` or `T`. |
| `Try<T>` | Holds the result of some computation of type `T` or an `Exception`. |

All of the data types above implement the [`IEnumerable`] interface. They can be
viewed as a container which holds at most one valid element `t` of type `T`.
[InCube] Core provides container-like extension methods for all these data types
as well as for the standard type [`Nullable<T>`].

| Method | Description |
| --- | --- |
| `Empty` | Create an empty wrapper. |
| `HasValue` | Indicates that the wrapper holds a valid element `t`.
| `Value` | Returns `t` or throws an `InvalidOperationException`.
| `GetValueOrDefault` | Returns either `t` or a default value, which may be produced by a function delegate. The delegate may be used to throw a more meaningful exception than by accessing `Value` directly.
| `Select` | Apply a mapping function to `t`.
| `SelectMany` | Apply a mapping function to `t` which in turn returns a wrapper type (corresponds to `flatMap`).|
| `ForEach` | Consume `t` in an [`Action`]. |
| `Match` | Either apply a mapping function to `t` or produce a default element through a function delegate. |
| `Where` | Remove `t` from the container unless it satisfies some predicate. |
| `Any` | Indicates that `t` exists and satisfies some predicate. |
| `All` | Indicates that `t` does not exist or satisfies some predicate. |
| `OrElse` | Fall back to another wrapper if the container is empty. |

## Collections

The [Collections] package of [InCube] Core contains [LINQ] type extension methods for many standard collection.

### Extensions for Enumerable Types and Lists

| Method | Description |
| --- | --- |
| `Empty` | Create an empty wrapper. |
| `MkString` | String formatting of an [`IEnumerable`].|
| `AsReadOnlyCollection`, `AsReadOnlyList` | Efficiently convert any [`ICollection`] and [`IList`] into [`IReadOnlyCollection`] and [`IReadOnlyList`], respectively.|
| `ForEach` | Apply an action to all elements of an [`IEnumerable`]. |
| `MaxBy`, `MinBy` | Search for the maximum / minimum of an [`IEnumerable`] by a selection key. |
| `ArgMax`, `ArgMin` | Find the index of the maximum / minimum in an [`IEnumerable`]. |
| `LowerBound`, `UpperBound` | Correspond to [`std::lower_bound`] and [`std::upper_bound`] from the [C++ Algorithms Library]. The implementation is based on [`BinarySearch`].|
| `ParSelect` | A parallelized version of [`Select`]. |
| `ParRemoveAll` | A parallelized version of [`RemoveAll`]. |
| `Unzip` | Split collections of tuples into their components. |
| `Split` | Separate an [`IEnumerable`] into two parts by means of a predicate. |
| `Iterate`, `Repeat`, `Generate` | Produce an [`IEnumerable`].
| `Slice` | Select segments of arrays and lists. |
| `Scan` | Generate a new sequence by mapping an [`IEnumerable`] with state. This is essentially a mix of [`Select`] and [`Aggregate`].
| `FirstOption`, `SingleOption` | Access the first or single element of this [`IEnumerable`] as an `Option<T>`.
| `Flatten` | Join an [`IEnumerable`] of containers into a single container. |

### Extensions for Dictionaries

| Method | Description |
| --- | --- |
| `Empty` | Create an empty read-only dictionary. |
| `GetOrDefault` | Return a default value if a key is not found. |
| `GetOrThrow` | Throw a meaningful exception if a key is not found. |
| `GetOption`, `GetMaybe`, `GetNullable` | Return an empty wrapper type if a key is not found. |
| `ToDictionary` | Convert an [`IEnumerable`] of tuples to a dictionary. |
| `AsReadOnlyDictionary` | Efficiently convert any [`IDictionary`] into [`IReadOnlyDictionary`].
| `AsSorted` | Convert any any [`IDictionary`] into [`SortedDictionary`]

<!-- 
## Adding InCube Core to your build

## Learn about InCube Core

- Our user guide, [InCube Core Explained]

-->

## Links

- [GitHub project](https://github.com/incube-group/core)
- [Issue tracker: Report a defect or feature request](https://github.com/incube-group/core/issues/new)

[InCube]: https://www.incubegroup.com
[InCube Core Explained]: https://github.com/incube-group/core/wiki/Home
[Collections]: src/InCube.Core/Collections/
[Functional]: src/InCube.Core/Functional/
[LINQ]: https://docs.microsoft.com/en-us/dotnet/api/system.linq
[C++ Algorithms Library]: https://en.cppreference.com/w/cpp/algorithm
[`std::lower_bound`]: https://en.cppreference.com/w/cpp/algorithm/lower_bound
[`std::upper_bound`]: https://en.cppreference.com/w/cpp/algorithm/upper_bound
[`BinarySearch`]: https://docs.microsoft.com/en-us/dotnet/api/system.array.binarysearch
[`RemoveAll`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.removeall
[`IEnumerable`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1
[`Select`]: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.select
[`Aggregate`]: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.aggregate
[`IDictionary`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2
[`IList`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1
[`ICollection`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1
[`IReadOnlyList`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1
[`IReadOnlyCollection`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1
[`IReadOnlyDictionary`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2
[`Nullable<T>`]: https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1
[`Action`]: https://docs.microsoft.com/en-us/dotnet/api/system.action-1
[`SortedDictionary`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2