using System.Collections.Generic;

namespace Penguin.Shared.Objects.Interfaces
{
    /// <summary>
    /// An interface to denote a recursive One-To-Many object structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRecursiveList<T> where T : IRecursiveList<T>
    {
        #region Properties

        /// <summary>
        /// The child objects of the parent type
        /// </summary>
        List<T> Children { get; set; }

        /// <summary>
        /// The optional parent of this node
        /// </summary>
        T Parent { get; set; }

        #endregion Properties
    }
}