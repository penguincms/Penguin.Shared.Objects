using Penguin.Extensions.Strings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Shared.Objects
{
    /// <summary>
    /// A recursive object used to build out a tree of objects based on a string representation of the object path
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> where T : class
    {
        #region Properties

        /// <summary>
        /// The nodes that reside beneath this node
        /// </summary>
        public List<TreeNode<T>> Children { get; set; }

        /// <summary>
        /// The character used to delimit nodes on the path
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// The full path of this node
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Whether or not this node has children
        /// </summary>
        public bool HasChildren => this.Children.Any();

        /// <summary>
        /// If false, this nodes existence was inferred from child paths and not part of the passed in list of paths used to
        /// create the tree
        /// </summary>
        public bool IsReal => !(this.Value is null);

        /// <summary>
        /// The last segment of the path representing the name of this node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parent node up one level
        /// </summary>
        public TreeNode<T> Parent { get; set; }

        /// <summary>
        /// The path to this node not including the name of this node.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The object that resides at this level of the tree
        /// </summary>
        public T Value { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Searches the downline of this node and returns a child node with a matching full name
        /// </summary>
        /// <param name="FullName">The full name to search for</param>
        /// <returns>A child node with the matching name, or null if none is found</returns>
        public TreeNode<T> this[string FullName] => this.GetChildByFullName(FullName);

        /// <summary>
        /// Creates a new tree node
        /// </summary>
        /// <param name="value">The object held by the node</param>
        /// <param name="PathFunction">A func used to return the path of this node on the tree</param>
        /// <param name="delimeter">The character used to delimit the nodes in the path (ex "C:\Program Files\My Application" == '\')</param>
        public TreeNode(T value, Func<T, string> PathFunction, char delimeter = '\\') : this(value, (PathFunction ?? throw new ArgumentNullException(nameof(PathFunction))).Invoke(value), delimeter)
        {
        }

        /// <summary>
        /// Creates a new tree node
        /// </summary>
        /// <param name="path">The path of this node</param>
        /// <param name="delimeter">The character used to delimit the nodes in the path (ex "C:\Program Files\My Application" == '\')</param>
        public TreeNode(string path, char delimeter = '\\') : this(null, path, delimeter)
        {
        }

        /// <summary>
        /// Creates a new tree node
        /// </summary>
        /// <param name="value">The object held by the node</param>
        /// <param name="path">The path of this node</param>
        /// <param name="delimeter">The character used to delimit the nodes in the path (ex "C:\Program Files\My Application" == '\')</param>
        public TreeNode(T value, string path, char delimeter = '\\')
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.Value = value;

            this.FullName = delimeter + path.Trim(delimeter);
            this.Delimiter = delimeter;

            if (this.FullName == $"{delimeter}")
            {
                this.Path = string.Empty;
                this.Name = $"{delimeter}";
                this.FullName = this.Name;
            }
            else if (this.FullName.Contains(delimeter))
            {
                this.Path = this.FullName.ToLast(delimeter);

                if (string.IsNullOrEmpty(this.Path))
                {
                    this.Path = $"{delimeter}";
                }

                this.Name = this.FullName.FromLast(delimeter);
            }
            else
            {
                this.Path = $"{delimeter}";
                this.Name = this.FullName;
            }

            this.Children = new List<TreeNode<T>>();
        }

        /// <summary>
        /// Gets a child node recursively by full name.
        /// </summary>
        /// <param name="FullName">The full name of the node to return</param>
        /// <returns>The first node matching the full name, or null</returns>
        public TreeNode<T> GetChildByFullName(string FullName)
        {
            TreeNode<T> found = this.Children.SingleOrDefault(t => t.FullName == FullName);

            if (found != null)
            {
                return found;
            }

            for (int i = 0; i < this.Children.Count; i++)
            {
                found = this.Children[i][FullName];

                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a child node recursively by value using the default comparer
        /// </summary>
        /// <param name="value">The value of the node to return</param>
        /// <returns>The first node matching the value, or null</returns>
        public TreeNode<T> GetChildByValue(T value)
        {
            TreeNode<T> found = this.Children.SingleOrDefault(t => t.Value == value);

            if (found != null)
            {
                return found;
            }

            for (int i = 0; i < this.Children.Count; i++)
            {
                found = this.Children[i].GetChildByValue(value);

                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds a leaf to this node
        /// </summary>
        /// <param name="thisNode">The node to add as a leaf</param>
        public void AddChild(TreeNode<T> thisNode)
        {
            if (thisNode is null)
            {
                throw new ArgumentNullException(nameof(thisNode));
            }

            if (thisNode.Parent != null)
            {
                thisNode.Parent.RemoveChild(thisNode);
            }

            this.Children.Add(thisNode);
            thisNode.Parent = this;
        }

        /// <summary>
        /// Finds a node in the current nodes downline, based on the path of the node
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public TreeNode<T> FindNode(string Path)
        {
            if (this.Path == Path)
            {
                return this;
            }
            else
            {
                return this.Children.Select(c => c.FindNode(Path)).Where(t => t != null).FirstOrDefault();
            }
        }

        /// <summary>
        /// Returns a list of tree nodes leading to this node, from trunk to leaf
        /// </summary>
        /// <returns>A list of tree nodes leading to this node, from trunk to leaf</returns>
        public List<TreeNode<T>> GetBranch()
        {
            List<TreeNode<T>> treeNodes = new List<TreeNode<T>>();

            TreeNode<T> leaf = this;

            while (leaf != null)
            {
                treeNodes.Add(leaf);
                leaf = leaf.Parent;
            }

            treeNodes.Reverse();

            return treeNodes;
        }

        /// <summary>
        /// Removed a child leaf from this node
        /// </summary>
        /// <param name="thisNode">The node to remove</param>
        public void RemoveChild(TreeNode<T> thisNode)
        {
            if (thisNode is null)
            {
                throw new ArgumentNullException(nameof(thisNode));
            }

            this.Children.Remove(thisNode);
            thisNode.Parent = null;
        }

        /// <summary>
        /// Returns the full path of this tree node
        /// </summary>
        /// <returns>The full path of this tree node</returns>
        public override string ToString()
        {
            return this.FullName;
        }

        #endregion Methods
    }
}