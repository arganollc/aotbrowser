using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PygmentSharp.Core.Utils
{
    /// <summary>
    /// Represents a slice of an array. A slice is a readonly view of an underlying
    /// array that can be treated like an independent collection: but it doesn't involve
    /// copy array parts all around.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal struct Slice<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Gets the start index into the underlying array
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Gets the length of the slice
        /// </summary>
        public int Length { get; }

        private readonly T[] _inner;

        /// <summary>
        /// Constructs a new slice of an array
        /// </summary>
        /// <param name="inner">the underlying array</param>
        /// <param name="start">The starting index at which this slice will begin</param>
        /// <param name="length">The length of the slice</param>
        public Slice(T[] inner, int start, int length)
        {
            _inner = inner;
            Start = start;

            if (length < 0)
                Length = 0;
            else
                Length = Math.Min(length, (_inner.Length - start));
        }

        /// <summary>
        /// Constructs a new slice of an array, from a <paramref name="start" /> index to the end
        /// </summary>
        /// <param name="inner">The underlying array</param>
        /// <param name="start">The starting index of the slice</param>
        public Slice(T[] inner, int start)
        {
            _inner = inner;
            Start = start;
            Length = _inner.Length - start;
        }

        /// <summary>
        /// Constructs a slice over an entire array
        /// </summary>
        /// <param name="inner">The underlying array</param>
        public Slice(T[] inner)
        {
            _inner = inner;
            Start = 0;
            Length = inner.Length;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return _inner[Start + i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a value at the specified <paramref name="index"/> of the slice
        /// </summary>
        /// <param name="index">The index to read</param>
        /// <returns>The value from the underlying array</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException();
                if (index >= Length)
                    throw new IndexOutOfRangeException();

                return _inner[Start + index];
            }
        }

        public static implicit operator Slice<T>(T[] inner)
        {
            return new Slice<T>(inner);
        }

        public Slice<T> SubSlice(int start)
        {
            int length = (Length - start);
            return new Slice<T>(_inner, Start + start, length);
        }

        public int Count => Length;

    }

    internal static class ArrayExtensions
    {
        /// <summary>
        /// Slices an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inner">The underlying array</param>
        /// <param name="start">The index in which to start the slice</param>
        /// <param name="length">The length of the slice</param>
        /// <returns></returns>
        public static Slice<T> Slice<T>(this T[] inner, int start, int length)
        {
            return new Slice<T>(inner, start, length);
        }

        /// <summary>
        /// Slices an array starting at a given index. If <paramref name="start"/> is negative,
        /// the slicing starts from that many items from the end of the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inner"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static Slice<T> Slice<T>(this T[] inner, int start)
        {
            var startIndex = start < 0 ? inner.Length + start : start;
            return new Slice<T>(inner, startIndex);
        }

    }
}
