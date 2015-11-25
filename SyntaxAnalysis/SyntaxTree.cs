using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis.PH.DataTree;
using System.IO;

namespace SyntaxAnalysis
{
    public class SyntaxTree
    {
        const int MAX_DEPTH = 100;
        const int MAX_LENGTH = 1000;
        private DTreeNode<string> root = new DTreeNode<string>("declares:" + "classes");
        private DTreeNode<string> code;
        private StringBuilder[] strings;
        int[] array;

        public DTreeNode<string> addNode(DTreeNode<string> parentNode, string value)
        {
            return parentNode.Nodes.Add(value);
        }

        public DTreeNode<string> getRootNode()
        {
            return root;
        }

        public void setEntryPoint(DTreeNode<string> entryPointNode)
        {
            code = entryPointNode;

        }

        /*public int printItselfToConsole(DTreeNode<string> node, int depth = 0)
        {
            DTreeNode<string> curr = root;
            int currOffset, offset = 0, counter = 0;

            if (depth > 0 && array[depth - 1] > array[depth]) array[depth] = array[depth - 1];
            foreach(DTreeNode<string> dNode in node.Nodes) 
            {
                counter++;
                //if (array[depth + 1] > array[depth]) array[depth] = array[depth + 1];
                currOffset = printItselfToConsole(dNode, depth + 1);
                if (counter == 1) offset = currOffset;
                //if (offset > array[depth]) array[depth] = offset;
            }
            //if (array[depth + 1] > array[depth]) array[depth] = array[depth + 1];
            if (offset > array[depth]) array[depth] = offset;

            if (depth == 0)
            {
                //Console.SetCursorPosition(array[depth], depth);
                //Console.writelin
                return 0;
            }

            Console.SetCursorPosition(array[depth], depth);
            array[depth] += node.Value.Length + 1;
            Console.Write(node.Value);
            if (counter == 0) return array[depth] - node.Value.Length - 1;
            else return offset;

            /*while (curr != null)
            {
                s = curr.Value;
                Console.SetCursorPosition(x, depth);
                Console.Write(s);
                x += s.Length;
                curr = curr.;
            }
        }*/

        public void printItselfToTheFile(string path)
        {
            array = new int[MAX_DEPTH];
            strings = new StringBuilder[MAX_DEPTH];
            string[] stringsAct = new string[MAX_DEPTH];

            for (int i = 0; i < MAX_DEPTH; i++)
            {
                strings[i] = new StringBuilder(MAX_LENGTH);
                strings[i].Insert(0, " ", MAX_LENGTH);
            }
            printItselfToTheFile(root, 0, 0);
            for (int i = 1; i < MAX_DEPTH; i++)
            {
                stringsAct[i] = strings[i].ToString();
            }

            File.WriteAllLines(path, stringsAct);
            array = null;
            strings = null;
        }

        /*private int printItselfToTheFile(DTreeNode<string> node, int depth = 0)
        {
            DTreeNode<string> curr = root;
            int currOffset, offset = 0, counter = 0;

            if (depth > 0 && array[depth - 1] > array[depth]) array[depth] = array[depth - 1];
            foreach (DTreeNode<string> dNode in node.Nodes)
            {
                offset = findMaxOffset(depth);
                counter++;
                currOffset = printItselfToTheFile(dNode, depth + 1);
                if (counter == 1) offset = currOffset;
            }

            if (depth == 0)
            {
                return 0;
            }

            //Console.SetCursorPosition(array[depth], depth);
            strings[depth]. Insert(array[depth], node.Value);
            array[depth] += node.Value.Length + 1;
            if (offset > array[depth]) array[depth] = offset;
            //Console.Write(node.Value);
            if (counter == 0) return array[depth] - node.Value.Length - 1;
            else return offset;
        }*/

        private void printItselfToTheFile(DTreeNode<string> node, int depth, int offset)
        {
            DTreeNode<string> curr = root;
            int currOffset, counter = 0;

            if (depth > 0 && array[depth - 1] > array[depth]) array[depth] = array[depth - 1];
            foreach (DTreeNode<string> dNode in node.Nodes)
            {
                //offset = findMaxOffset(depth);
                printItselfToTheFile(dNode, depth + 1, findMaxOffset(depth));
            }

            if (array[depth] < offset) array[depth] = offset;
            strings[depth].Insert(array[depth], node.Value);
            if (depth == 0) return;
            array[depth] += node.Value.Length + 1;
        }

        /// <summary>
        /// Finds maximum offset in the strings after num position.
        /// </summary>
        /// <param name="num">Number of string to find maximum offset after it.</param>
        /// <returns>Actual maximum offset.</returns>
        private int findMaxOffset(int num)
        {
            int max = 0;
            while (num < MAX_DEPTH && (array[num] != 0 || max == 0))
            //while (array[num] != 0)
            {
                if (array[num] > max) max = array[num];
                num++;
            }
            return max;
        }

        public void printSomeNodes(DTreeNode<string> node, ref int toPrint)
        {
            
            if (node.Nodes.Count > 0)
            {
                printSomeNodes(node.Nodes[0], ref toPrint);
            }
            Console.WriteLine(LexemTypeHelper.getParsedValue(node.Value));
            if (toPrint < 1) return;
            toPrint--;
            if (toPrint < 1) return;
            if (node.Nodes.Count > 0)
            {
                printSomeNodes(node.Nodes[1], ref toPrint);
            }
        }
    }
}
