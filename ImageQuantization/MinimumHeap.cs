using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class MinimumHeap
    {
        public Edge[] arr;
        public int HeapSize; 

        public MinimumHeap(int capacity)
        {
            HeapSize = 0;
            arr = new Edge[capacity];
        }

        /// <summary>
        /// This function swaps two Edges
        /// </summary>
        public static void Swap(ref Edge x, ref Edge y,int i,int j)
        {
            Edge temp;           
            temp = x;
            x = y;
            y = temp;
        }

        /// <summary>
        /// This function gives the givin node the Min Heap property
        /// </summary>
        public void Min_Heapify(int index)
        {
            int right = (2 * index) + 2; //index of right child
            int left = (2 * index) + 1; //index of left child
            int min;

            if (left <= HeapSize && arr[left].weight < arr[index].weight)
                min = left;
            else
                min = index;

            if (right <= HeapSize && arr[right].weight < arr[min].weight)
                min = right;

            if (min != index)
            {
                Swap(ref arr[index], ref arr[min],index,min);
                QuantizationProcess.indeciesInQueue[arr[index].destnation] = index;
                QuantizationProcess.indeciesInQueue[arr[min].destnation] = min;
                Min_Heapify(min);
            }
        }

        /// <summary>
        /// This function makes a min heap out of the array
        /// </summary>
        public void Build_Heap()
        {
            for (int i = arr.Length / 2; i >= 0; i--)
                Min_Heapify(i);
        }

        /// <summary>
        /// This function returns the minimum element
        /// </summary>
        public Edge HeapMinimum()
        {
            return arr[0];
        }

        /// <summary>
        /// This function returns the minimum element and remove it and re-heap it
        /// </summary>
        public Edge ExtractMinimum()
        {
            Edge min;
            if (HeapSize == 0)
                return null;

            min = arr[0];
            arr[0] = arr[HeapSize-1];
            HeapSize--;
            Min_Heapify(0);

            return min;
        }

        /// <summary>
        /// This function modifies the nodes to its proper position if any node value has been decreased
        /// </summary>
        public void HeapDecreaseKey(int index,Edge key)
        {
            arr[index] = key;
            while (index != 0 && arr[(index - 1) / 2].weight > arr[index].weight)
            {
                Swap(ref arr[index], ref arr[(index - 1) / 2],index+1,((index - 1) / 2)+1);
                QuantizationProcess.indeciesInQueue[arr[index].destnation] = index;
                QuantizationProcess.indeciesInQueue[arr[(index - 1) / 2].destnation] = (index - 1) / 2;
                index = (index - 1) / 2;
            }
        }

        /// <summary>
        /// This function inserts a new node and re-heap the tree
        /// </summary>
        
        public void Insert(Edge key)
        {
            HeapSize++;
            arr[HeapSize - 1] = null;
            HeapDecreaseKey(HeapSize - 1, key);
        }
        /// <summary>
        /// Update the wieght and the source of a specific edge in the queue and modifies the nodes to its proper position 
        /// </summary>
        /// <param name="index"> index of the egde in the queue </param>
        /// <param name="newWeight"> the new wieght that will be updated with </param>
        /// <param name="newSource"> the new source that will nbe updated with </param>
        public void Update(int index,double newWeight, int newSource)
        {
            arr[index].weight = newWeight;
            arr[index].source = newSource;
            HeapDecreaseKey(index, arr[index]);
        }
    }
}
