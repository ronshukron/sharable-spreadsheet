using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadedBinarySearchTree
{
    class Node
    {
        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }
        public Node Parent { get; set; }
        public int Data { get; set; }
    }

    class ThreadedBinarySearchTree
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public Node Root { get; set; }
        public void Add(int value)
        {
            cacheLock.EnterWriteLock();
            try
            {
                Node before = null, after = this.Root;

                while (after != null)
                {
                    before = after;
                    if (value < after.Data) //Is new node in left tree? 
                        after = after.LeftNode;
                    else if (value > after.Data) //Is new node in right tree?
                        after = after.RightNode;
                    else
                    {
                        //Exist same value
                        return;
                    }
                }
                Node newNode = new Node();
                newNode.Data = value;

                if (this.Root == null)//Tree ise empty
                    this.Root = newNode;
                else
                {
                    if (value < before.Data)
                    {
                        before.LeftNode = newNode;
                        newNode.Parent = before;
                    }
                    else
                    {
                        before.RightNode = newNode;
                        newNode.Parent = before;
                    }
                }

            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public bool search(int value)
        {
            cacheLock.EnterReadLock();
            try
            {
                return Find(value, this.Root);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        private bool Find(int value, Node parent)
        {
            while (parent != null)
            {
                if (value == parent.Data) return true;
                if (value < parent.Data)
                    parent = parent.LeftNode;
                else
                    parent = parent.RightNode;
            }

            return false;
        }

        private Node FindReturnNode(int value, Node parent)
        {
            while (parent != null)
            {
                if (value == parent.Data) return parent;
                if (value < parent.Data)
                    parent = parent.LeftNode;
                else
                    parent = parent.RightNode;
            }

            return null;
        }
        private int checkIfLeftOrRightChild(Node node_to_remove)
        {
            if (node_to_remove.Parent == null)
                return 2;
            if (node_to_remove.Parent.RightNode == node_to_remove)
                return 1;
            return 0;
        }
        public void remove(int num) {
            cacheLock.EnterWriteLock();
            try
            {
                Node node_to_remove = FindReturnNode(num, this.Root);
                if (node_to_remove == null)
                {
                    return;
                }
                else
                {
                    // node is a leaf
                    if (node_to_remove.LeftNode == null && node_to_remove.RightNode == null)
                    {
                        if (checkIfLeftOrRightChild(node_to_remove) == 2)
                            this.Root = null;
                        else if (checkIfLeftOrRightChild(node_to_remove) == 1)
                        {
                            node_to_remove.Parent.RightNode = null;
                        }
                        else
                            node_to_remove.Parent.LeftNode = null;
                    }
                    // only has a left child
                    else if (node_to_remove.LeftNode != null && node_to_remove.RightNode == null)
                    {
                        // what node i am for my parent? left or right
                        if(node_to_remove == this.Root)
                        {
                            this.Root = node_to_remove.LeftNode;
                            this.Root.Parent = null;
                        }
                        else if(node_to_remove.Parent.LeftNode == node_to_remove)
                        {
                            node_to_remove.Parent.LeftNode = node_to_remove.LeftNode;
                            node_to_remove.LeftNode.Parent = node_to_remove.Parent;
                            node_to_remove = null;
                        }
                        else
                        {
                            node_to_remove.Parent.RightNode = node_to_remove.LeftNode;
                            node_to_remove.LeftNode.Parent = node_to_remove.Parent;
                            node_to_remove = null;
                        }
                    }
                    // only has right child 
                    else if (node_to_remove.LeftNode == null && node_to_remove.RightNode != null)
                    {
                        // what node i am for my parent? left or right
                        if (node_to_remove == this.Root)
                        {
                            this.Root = node_to_remove.RightNode;
                            this.Root.Parent = null;
                        }
                        else if (node_to_remove.Parent.LeftNode == node_to_remove)
                        {
                            node_to_remove.Parent.LeftNode = node_to_remove.RightNode;
                            node_to_remove.RightNode.Parent = node_to_remove.Parent;
                            node_to_remove = null;
                        }
                        else
                        {
                            node_to_remove.Parent.RightNode = node_to_remove.RightNode;
                            node_to_remove.RightNode.Parent = node_to_remove.Parent;
                            node_to_remove = null;
                        }
                    }
                    // has 2 children
                    else if (node_to_remove.LeftNode != null && node_to_remove.RightNode != null)
                    {
                        Node min_node_sub_tree = MinValue(node_to_remove.RightNode);
                        int min_val_sub_tree = MinValueInt(node_to_remove.RightNode);
                        if (min_node_sub_tree == node_to_remove.RightNode && min_node_sub_tree.RightNode != null)
                        {
                            node_to_remove.RightNode = min_node_sub_tree.RightNode;
                            min_node_sub_tree.RightNode.Parent = node_to_remove;
                            node_to_remove.Data = min_val_sub_tree;
                            min_node_sub_tree = null;
                        }
                        else if (min_node_sub_tree.RightNode != null)
                        {
                            min_node_sub_tree.Parent.LeftNode = min_node_sub_tree.RightNode;
                            min_node_sub_tree.RightNode.Parent = min_node_sub_tree.Parent;
                            node_to_remove.Data = min_val_sub_tree;
                            min_node_sub_tree = null;
                        }
                        else
                        {
                            node_to_remove.Data = min_val_sub_tree;
                            if (node_to_remove.RightNode == min_node_sub_tree)
                                node_to_remove.RightNode = null;
                            else
                                node_to_remove.LeftNode = null;

                            min_node_sub_tree = null;

                        }

                    }
            }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
            
        
        }

        private Node MinValue(Node node)
        {
            int minv = node.Data;
            Node before = node;

            while (node.LeftNode != null)
            {
                before = node;
                minv = node.LeftNode.Data;
                node = node.LeftNode;
            }

            return before;
        }
        private int MinValueInt(Node node)
        {
            int minv = node.Data;

            while (node.LeftNode != null)
            {
                minv = node.LeftNode.Data;
                node = node.LeftNode;
            }

            return minv;
        }
        public void clear() {
            try
            {
                while (this.Root != null)
                {
                    remove(this.Root.Data);
                }
            }
            finally
            {
            }
        }

        public void print() {
            cacheLock.EnterReadLock();
            try
            {
                Node curr = this.Root;
                if (curr == null)
                    return;
                Stack<Node> stack = new Stack<Node>();
                while (stack.Count > 0 || curr != null)
                {
                    while (curr != null)
                    {
                        stack.Push(curr);
                        curr = curr.LeftNode;
                    }
                    curr = stack.Pop();

                    Console.Write(curr.Data + " ");

                    curr = curr.RightNode;

                }
                Console.WriteLine();
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

    }
}
