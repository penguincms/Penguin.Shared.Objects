using System;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Shared.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class IEnumerableExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Converts an IEnumerable of strings to a tree node structure
        /// </summary>
        /// <param name="target">The target IEnumerable</param>
        /// <param name="Delimeter">The delimiter to use for nodes in the path</param>
        /// <param name="comparer">The string comparer to use when testing the path for equality</param>
        /// <returns>A tree node structure of the original IEnumerable</returns>
        public static TreeNode<string> ToTree(this IEnumerable<string> target, char Delimeter = '\\', StringComparer comparer = null)
        {
            return ToTree(target, (s) => s, Delimeter, comparer);
        }

        /// <summary>
        /// Converts a generic object IEnumerable to a tree node structure
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="target">The IEnumerable to convert</param>
        /// <param name="Path">A function that acts on the object to return a representation of its path in the tree</param>
        /// <param name="Delimeter">The delimiter to use for nodes in the path</param>
        /// <param name="comparer">The string comparer to use when testing the path for equality</param>
        /// <returns>A tree node structure of the original IEnumerable</returns>
        public static TreeNode<T> ToTree<T>(this IEnumerable<T> target, Func<T, string> Path, char Delimeter = '\\', StringComparer comparer = null) where T : class
        {
            if (!target.Any())
            {
                TreeNode<T> root = new TreeNode<T>($"{Delimeter}", Delimeter);
                return root;
            }

            if (comparer == null)
            {
                comparer = StringComparer.OrdinalIgnoreCase;
            }

            Dictionary<string, TreeNode<T>> working = new Dictionary<string, TreeNode<T>>(comparer);

            foreach (TreeNode<T> thisNode in target.Select(v => new TreeNode<T>(v, Path, Delimeter)))
            {
                working.Add(thisNode.FullName, thisNode);
            }

            int i = 0;

            while (i < working.Count)
            {
                TreeNode<T> thisNode = working.ElementAt(i).Value;

                if (!working.ContainsKey(thisNode.Path))
                {
                    working.Add(thisNode.Path, new TreeNode<T>(thisNode.Path, Delimeter));
                }

                i++;
            }

            foreach (TreeNode<T> thisNode in working.Select(t => t.Value))
            {
                if (!string.IsNullOrEmpty(thisNode.Path))
                {
                    working[thisNode.Path].AddChild(thisNode);
                }
            }

            return working.First(t => string.IsNullOrEmpty(t.Value.Path)).Value;
        }
    }
}