using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis
{
    class SyntaxTree
    {
        private LinkedList<string> tree = new LinkedList<string>();
        private LinkedListNode<string> head = new LinkedListNode<string>("head");
        private LinkedListNode<string> code = new LinkedListNode<string>("main");

        public SyntaxTree()
        {
            tree.AddFirst(head);
        }

        public LinkedListNode<string> addNode(LinkedListNode<string> parentNode, string value) {
            return tree.AddAfter(parentNode, value);
        }

        public LinkedListNode<string> getHeadNode()
        {
            return head;
        }

        public void setEntryPoint(LinkedListNode<string> entryPointNode)
        {
            code = entryPointNode;
        }

    }
}
