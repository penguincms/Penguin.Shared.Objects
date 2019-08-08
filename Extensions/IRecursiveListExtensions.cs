using Penguin.Shared.Objects.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Shared.Objects.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class IRecursiveListExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Flattens an IRecursive list
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="target">The list to flatten</param>
        /// <returns>A flattened representation of the target recursive tree</returns>
        public static IEnumerable<T> Flatten<T>(this T target) where T : IRecursiveList<T>
        {
            List<T> toReturn = new List<T>() { target };

            int i = 0;

            yield return target;

            while (i < toReturn.Count())
            {
                foreach (T item in toReturn.ElementAt(i++).Children)
                {
                    toReturn.Add(item);
                    yield return item;
                }
            }
        }
    }
}