# InCube Core

[InCube] Core is a set of C# libraries that provides extensions for functional
programming, standard collection types, value parsing and formatting, and more.

## Adding InCube Core to your build

Binary packages of [InCube] Core are available on [nuget.org]. Add the following lines to your `.csproj` in order to use the library in your [.Net Core] build:

```xml
  <ItemGroup>
    <PackageReference Include="InCube.Core" Version="1.0.0" />
  </ItemGroup>
```

## Functional Programming

The [Functional] package of [InCube] Core provides read-only wrapper types in C#
which are common in other functional programming languages.

| Type | Description |
| --- | --- |
| [`Option<T>`] | A wrapper designed along the lines of [`Nullable<T>`] that supports *struct and reference types*. See [`OptionDemo`] for examples.|
| [`Maybe<T>`] | An optimized version of [`Option<T>`] that supports *reference types* only. |
| [`Either<TL, T>`] | A union type of either `TL` or `T`. |
| [`Try<T>`] | Holds the result of some computation of type `T` or an `Exception`. |

All of the data types above implement the [`IEnumerable<T>`] interface. They can be
viewed as a container which holds at most one valid element `t` of type `T`.
[InCube] Core provides container-like extension methods for all these data types
as well as for [`Nullable<T>`].

| Method | Description |
| --- | --- |
| `Empty` | Create an empty wrapper. |
| `HasValue` | Indicates whether a wrapper holds a valid element `t`.
| `Value` | Returns either `t` or throws an [`InvalidOperationException`].
| `GetValueOrDefault` | Returns either `t` or a default value. |
| `GetValueOr` | Returns either `t` or produces a new value by calling a function delegate. The delegate may be used to throw a more meaningful exception than by accessing `Value` directly.
| `Select` | Apply a mapping function to `t`.
| `SelectMany` | Apply a mapping function to `t` that returns a wrapper type and flatten (corresponds to `flatMap`).|
| `ForEach` | Consume `t` in an [`Action`]. |
| `Match` | Either apply a mapping function to `t` or produce a default element calling a function delegate. |
| `Where` | Remove `t` from the container unless it satisfies some predicate. |
| `Any` | Indicates that `t` exists and satisfies some predicate. |
| `All` | Indicates that `t` does not exist or satisfies some predicate. |
| `OrElse` | Fall back to another wrapper if the container is empty. |

## Collections

The [Collections] package of [InCube] Core provides [LINQ] type extension methods for many standard containers.

### Extensions for [Enumerables]

| Method | Description |
| --- | --- |
| `MkString` | Format an [`IEnumerable`] as a string.|
| `ForEach` | Apply an action to all elements of an [`IEnumerable`]. |
| `MaxBy`, `MinBy` | Search for the maximum / minimum of an [`IEnumerable`] by a selection key. |
| `ArgMax`, `ArgMin` | Find the index of the maximum / minimum in an [`IEnumerable`]. |
| `Split` | Separate an [`IEnumerable`] into two parts by means of a predicate. |
| `ToEnumerable`, `Iterate`, `Generate` | Produce a new [`IEnumerable`].
| `Scan` | Generate a new sequence by mapping an [`IEnumerable`] with state. This is essentially a mix of [`Select`] and [`Aggregate`].
| `FirstOption`, `SingleOption` | Access the first or single element of this [`IEnumerable`] as an [`Option<T>`].
| `Flatten` | Join an [`IEnumerable`] of containers into a single sequence. |

### Extensions for [Lists]

| Method | Description |
| --- | --- |
| `Last`, `LastOption` | Access to the last element of a list either directly or as an [`Option<T>`], respectively. |
| `AsReadOnlyCollection`, `AsReadOnlyList` | Efficiently convert any [`ICollection`] and [`IList`] into [`IReadOnlyCollection`] and [`IReadOnlyList`], respectively.|
| `Items`, `Slice` | Select indexed subsets and segments of sequences. |
| `LowerBound`, `UpperBound` | Correspond to [`std::lower_bound`] and [`std::upper_bound`] from the [C++ Algorithms Library]. The implementation is based on [`BinarySearch`].|
| `ParSelect` | A parallelized version of [`Select`]. |
| `ParRemoveAll` | A parallelized version of [`RemoveAll`]. |
| `ParGenerate` | Initialize an array in parallel using a function delegate. |

### Extensions for [Dictionaries]

| Method | Description |
| --- | --- |
| `Empty` | Create an empty read-only dictionary. |
| `GetOrDefault` | Return a default value if a key is not found. |
| `GetOrThrow` | Throw a meaningful exception if a key is not found. |
| `GetOption`, `GetMaybe`, `GetNullable` | Return an empty wrapper type if a key is not found. |
| `ToDictionary` | Convert an [`IEnumerable`] of tuples to a [`Dictionary`]. |
| `AsReadOnlyDictionary` | Efficiently convert any [`IDictionary`] into an [`IReadOnlyDictionary`].
| `AsSorted` | Convert any [`IDictionary`] into a [`SortedDictionary`]

### Extensions for [Tuples]

| Method | Description |
| --- | --- |
| `MakePair`, `MakeTuple`, `MakeValueTuple` | Factories for [`KeyValuePair`], [`Tuple`], and [`ValueTuple`] which help inferring type arguments.|
| `ZipAsTuple` | Combine multiple sequences into a single sequence of tuples. |
| `Unzip` | Split sequences of tuples into their components. |
| `ZipWithIndex` | Zip a sequence with the index of each element. |
| `ZipI` | A variant of [`Zip`] which passes the index of each element as third argument to the function delegate. |
| `Zip3`, `Zip4` | Variants of [`Zip`] for 3 and 4 input sequences, respectively. |
| `TupleSelect` | A variant of [`Select`] specialized for mapping tuples. |
| `Keys`, `Values` | Select the keys or values in a sequence of key-value pairs, respectively. |
| `MapValues` | Apply a function delegate to the values in a sequence of key-value pairs. |
| `AsTuple`, `AsKeyValuePair` | Convert between sequences of [`ValueTuple`]s and [`KeyValuePair`]s. |

### Numerical Extensions and Statistics

| Type / Method | Description |
| --- | --- |
| [`Histogram<T>`], `MakeHistogram` | Compute a histogram from a collection of comparable elements. |
| `Rank`, `VectorRank` | Compute the rank of one or multiple elements in a collection. |
| [`Range<T>`] | Represents a closed interval and provides typical operations like overlap and intersection. |

## Utility Classes

| Type | Description |
| --- | --- |
| [`Preconditions`] | Provides methods for checking input arguments very similar to [`Preconditions.java`] in [Google Guava].
| [`Disposables`] | Manage many [`IDisposable`] objects as a single disposable collection. |

<!-- 
## Adding InCube Core to your build

## Learn about InCube Core

- Our user guide, [InCube Core Explained]

-->

## Links

- [GitHub project]
- [Issue tracker: Report a defect or feature request](https://github.com/incube-group/core/issues/new)

[InCube]: https://www.incubegroup.com
[Github project]: https://github.com/incube-group/core
[InCube Core Explained]: https://github.com/incube-group/core/wiki/Home
[Functional]: src/InCube.Core/Functional/
[`Option<T>`]: src/InCube.Core/Functional/Option.cs
[`OptionDemo`]: src/InCube.Core.Demo/Functional/OptionDemo.cs
[`Maybe<T>`]: src/InCube.Core/Functional/Maybe.cs
[`Try<T>`]: src/InCube.Core/Functional/Try.cs
[`Either<TL, T>`]: src/InCube.Core/Functional/Either.cs
[`Histogram<T>`]: src/InCube.Core/Numerics/Histogram.cs
[`Range<T>`]: src/InCube.Core/Numerics/Range.cs
[Collections]: src/InCube.Core/Collections/
[Enumerables]: src/InCube.Core/Collections/Enumerables.cs
[Lists]: src/InCube.Core/Collections/Lists.cs
[Dictionaries]: src/InCube.Core/Collections/Dictionaries.cs
[Tuples]: src/InCube.Core/Collections/Tuples.cs
[LINQ]: https://docs.microsoft.com/en-us/dotnet/api/system.linq
[C++ Algorithms Library]: https://en.cppreference.com/w/cpp/algorithm
[`std::lower_bound`]: https://en.cppreference.com/w/cpp/algorithm/lower_bound
[`std::upper_bound`]: https://en.cppreference.com/w/cpp/algorithm/upper_bound
[`BinarySearch`]: https://docs.microsoft.com/en-us/dotnet/api/system.array.binarysearch
[`RemoveAll`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.removeall
[`IEnumerable<T>`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1
[`IEnumerable`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1
[`Select`]: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.select
[`Aggregate`]: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.aggregate
[`IDictionary`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2
[`IList`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1
[`ICollection`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1
[`IReadOnlyList`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1
[`IReadOnlyCollection`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1
[`IReadOnlyDictionary`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2
[`Dictionary`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2
[`Nullable<T>`]: https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1
[`Action`]: https://docs.microsoft.com/en-us/dotnet/api/system.action-1
[`SortedDictionary`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2
[`KeyValuePair`]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2
[`Tuple`]: https://docs.microsoft.com/en-us/dotnet/api/system.tuple-2
[`ValueTuple`]: https://docs.microsoft.com/en-us/dotnet/api/system.valuetuple-2
[`Zip`]: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.zip
[`Preconditions.java`]: https://github.com/google/guava/blob/master/guava/src/com/google/common/base/Preconditions.java
[Google Guava]: https://github.com/google/guava
[`IDisposable`]: https://docs.microsoft.com/en-us/dotnet/api/system.idisposable
[`Preconditions`]: src/InCube.Core/Preconditions.cs
[`Disposables`]: src/InCube.Core/Disposables.cs
[`InvalidOperationException`]: https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception
[nuget.org]: https://www.nuget.org/packages/InCube.Core
[.Net Core]: https://dotnet.microsoft.com/download