using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{

    public class Node
    {
        protected string name;
        protected Node parent;
        private List<Node> children = new List<Node>();

        public Node(string name)
        {
            this.name = name;
        }

        public void AddChild(Node node)
        {
            children.Add(node);
        }
        public void RemoveChild(Node node)
        {
            children.Remove(node);
        }
        public Node GetParent()
        {
            return parent;
        }
        public List<Node> GetChildren()
        {
            return children;
        }
        public Node GetChild(int index)
        {
            return children[index];
        }
        public Node GetChild(string name)
        {
            Node child = children.Find(
                delegate (Node component)
                {
                    return component.name == name;
                });

            return child;
        }
        public void Update()
        {
            foreach (Node child in children)
            {
                child.Update();
            }
        }
        public void Initialize()
        {
            foreach (Node child in children)
            {
                child.Initialize();
            }
        }
    }
}
