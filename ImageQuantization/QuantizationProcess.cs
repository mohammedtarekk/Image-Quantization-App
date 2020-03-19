using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;

namespace ImageQuantization
{
    
    /// <summary>
	///class Edge has 3 properties source,destination,weight
	/// </summary>
	public class Edge
    {
        public int source, destnation;
        public double weight = 0;
        /// <summary>
        ///Constructor of Edge
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="w"></param>
        public Edge(int src, int dest, double w)
        {
            source = src;
            destnation = dest;
            weight = w;
        }

    }


    class QuantizationProcess
    {
		/// <summary>
		/// Calculates eculidean distance between the given two edges
		/// </summary>
		/// <param name="rgb1"></param>
		/// <param name="rgb2"></param>
		/// <returns>double Eculidean Distance</returns>
        private double EculideanDistance(RGBPixel rgb1, RGBPixel rgb2)
        {
            return Math.Sqrt((Math.Pow((rgb1.red - rgb2.red), 2)) + (Math.Pow((rgb1.green - rgb2.green), 2)) + (Math.Pow((rgb1.blue - rgb2.blue), 2)));
        }
        // Hashtable that contains all unique colors 
        public static Hashtable distinctHashtable;
        // Array of distincit Colors 
        public static RGBPixel[] distinctColors;

		public static int[,] vertixNumber;

		public static int width, height;
		/// <summary>
		/// Gets the distinct colors from the matrix of colors
		/// </summary>
		/// <returns>Set of colors</returns>
		public static void GetDistinct(RGBPixel[,] M)
        {
			width = M.GetLength(0);
			height = M.GetLength(1);
            distinctHashtable = new Hashtable();

			distinctColors = new RGBPixel[width * height];

			vertixNumber = new int[width,height];

			int hashFunctionKey = 0;
            int Counter = 0;
            for (int i = 0; i < M.GetLength(0); i++)
            {
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    // Calculates the Key of the hashtable from three integers (red,blue,green) 
                    hashFunctionKey = 8 * (M[i, j].green + M[i, j].blue) * (M[i, j].green + M[i, j].blue + 1) + M[i, j].blue;
                    hashFunctionKey = 5 * (hashFunctionKey + M[i, j].red) * (hashFunctionKey + M[i, j].red + 1) + M[i,j].red;

                    if (!distinctHashtable.ContainsKey(hashFunctionKey))
                    {
                        distinctHashtable.Add(hashFunctionKey, Counter);
                        distinctColors[Counter] = M[i, j];
                        Counter++;
                    }
					vertixNumber[i, j] = (int)distinctHashtable[hashFunctionKey];
                }
            }
        }

        /// Contains index of each edge in the queue
        public static int[] indeciesInQueue;
        
		/// <summary>
		/// Gets the minimum span tree using Prim's algorithm
		/// </summary>
		private void MSTPrim()
        {
            
			//Sumation of wieghts of the MST
            double MSTEdgesSum = 0;

			//Number of distinct colors
			int numberOfDistinctColors = distinctHashtable.Count;

			// result edges of MST
			MSTresult = new MinimumHeap(numberOfDistinctColors - 1);

			/// Temporary edge to inqueue in 'edges'
			Edge tmpEdge;

			/// Priority queue contains edges in order
			MinimumHeap edges = new MinimumHeap(numberOfDistinctColors - 1);


            indeciesInQueue = new int[numberOfDistinctColors];

			//Contains minimum wieght of each vertix
            double[] minimumWieght = new double[numberOfDistinctColors];
            // List of all non Visted Nodes 
            List<int> nonVisted = new List<int>();
            int tmpSource = 0;
            double tmpWieght;
            int tmpDistination;
            /// Loop to fill the minimum wieght of each vertex with the distination between each one of them and the first node which is the tmpSource O(D)
            for ( tmpDistination = 1; tmpDistination < numberOfDistinctColors; tmpDistination++)
            {
                        nonVisted.Add(tmpDistination);
                        tmpWieght = EculideanDistance(distinctColors[tmpSource], distinctColors[tmpDistination]);
                        tmpEdge = new Edge(tmpSource, tmpDistination, tmpWieght);
                        minimumWieght[tmpDistination] = tmpWieght;
                        indeciesInQueue[tmpDistination] = tmpDistination - 1;
                        // insert in the queue
                        edges.Insert(tmpEdge);
            }
            // take the min edge in the queue
            tmpEdge = edges.ExtractMinimum();
            // insert the min edge in the result MST queue
            MSTresult.Insert(tmpEdge);
            // the next source will be the min edge distination 
            tmpSource = tmpEdge.destnation;
            // remove the min edge distination from the nonVisted list as it become Vistied 
            nonVisted.Remove(tmpSource);
            // add the min edge Weight to the Summution of MST edges
            MSTEdgesSum += tmpEdge.weight;
            /// Nested Loop that Calculate the MST  O(Elog(V))
            /// first Loop O(E) that takes all edges in the queue 
            while (edges.HeapSize!=0)
            {
                /// Second Loop that Calculate the distance between the tmpSource and all of the non visted nodes O(Log(V))
                for (int i = 0; i < nonVisted.Count; i++)
                {
                    tmpDistination = nonVisted[i];
                    tmpWieght = EculideanDistance(distinctColors[tmpSource], distinctColors[tmpDistination]);
                    if (tmpWieght < minimumWieght[tmpDistination])
                    {
                        //Update edge at the index found in 'indeciesInQueue[tmpDistination]' with 'tmpSource' and 'tmpWieght'
                        edges.Update(indeciesInQueue[tmpDistination], tmpWieght, tmpSource);
                        //Update the minimum wieght of 'tmpDistination' with tmpWieght
                        minimumWieght[tmpDistination] = tmpWieght;
                    }
                }
                // take the min edge in the queue
                tmpEdge = edges.ExtractMinimum();
                // insert the min edge in the result MST queue
                MSTresult.Insert(tmpEdge);
                // the next source will be the min edge distination 
                tmpSource = tmpEdge.destnation;
                // remove the min edge distination from the nonVisted list as it become Vistied 
                nonVisted.Remove(tmpSource);
                // add the min edge Weight to the Summution of MST edges
                MSTEdgesSum += tmpEdge.weight;
            }
              
			//========================== TEST ===============================================
                      MessageBox.Show(distinctHashtable.Count.ToString() + "\n" + MSTEdgesSum.ToString());
			//========================== TEST ===============================================           
		}
		/// priority queue contains edges of MST
		MinimumHeap MSTresult;

		//colors palett
		RGBPixel[] Palette;

		Hashtable takenVertices;
		public void Cluster(int K)
		{
			//Contains the average of the clusters
			Palette = new RGBPixel[K];

			//Contains the number of colors in each cluster (used for calculating the average)
			int[] RGBCount = new int[K]; 

			//Contains all clustered vertecies
			takenVertices = new Hashtable();
			
			//Containes each edge in the resulte edges
			Edge edge;

			//	number of non-clusterd vertices		 , number of remaining clusters 
			int remainVertecies = distinctHashtable.Count, remainClusters = K, indexInPalette = 0, src, dis, tmpIndex;
			bool srcFound = false, disFound = false;

			//O(D)
			while(remainVertecies != 0)
			{
				
				//O(logn)
				edge = MSTresult.ExtractMinimum();
				src = edge.source;
				dis = edge.destnation;
				//O(1)
				srcFound = takenVertices.ContainsKey(src);
				disFound = takenVertices.ContainsKey(dis);

				//if src & dis or one of them is found in takenVertices
				if (!srcFound || !disFound)
				{
					//if remaining vertices > remaining clusters then add new cluster or extend an old cluster
					if (remainVertecies > remainClusters )
					{
						//if src and dis both are not found in takenVertices
						if (!srcFound && !disFound )
						{
							//if remaining vertices > 0 then add new cluster of(src, dis)
							if(remainClusters > 0)
							{
								RGBCount[indexInPalette] = 2;

								//add the RGB of src in Palette[indexInPalette]
								Palette[indexInPalette].red += distinctColors[src].red;
								Palette[indexInPalette].green += distinctColors[src].green;
								Palette[indexInPalette].blue += distinctColors[src].blue;

								//add the RGB of dis and divide the result over 2 to get the avarege
								Palette[indexInPalette].red = (byte)((Palette[indexInPalette].red + distinctColors[dis].red) / RGBCount[indexInPalette]);
								Palette[indexInPalette].green = (byte)((Palette[indexInPalette].green + distinctColors[dis].green) / RGBCount[indexInPalette]);
								Palette[indexInPalette].blue = (byte)((Palette[indexInPalette].blue + distinctColors[dis].blue) / RGBCount[indexInPalette]);

								//add src & dis to taken vertices
								takenVertices.Add(src, indexInPalette);
								takenVertices.Add(dis, indexInPalette);

								indexInPalette++;
								remainClusters--;
							}
						}
						//if src is found and dis is not found in takenVertices
						else if (srcFound && !disFound)
						{
							//if remaining vertices is not the remaining clusters then add the dis to the cluster of src
							if (remainVertecies != remainClusters)
							{
								tmpIndex = (int)takenVertices[src];

								//calculating the average of the extended cluster
								//Accessing Hashtable O(1)
								Palette[tmpIndex].red = (byte)((Palette[tmpIndex].red*RGBCount[tmpIndex] + distinctColors[dis].red)/++RGBCount[tmpIndex]);
								Palette[tmpIndex].green = (byte)((Palette[tmpIndex].green * RGBCount[tmpIndex] + distinctColors[dis].green) / ++RGBCount[tmpIndex]);
								Palette[tmpIndex].blue = (byte)((Palette[tmpIndex].blue * RGBCount[tmpIndex] + distinctColors[dis].blue) / ++RGBCount[tmpIndex]);


								takenVertices.Add(dis,(int)takenVertices[src]);
							}
						}
						//if src is not found and dis is found in takenVertices
						else
						{
							//if remaining vertices is not the remaining clusters then add the src to the cluster of dis
							if (remainVertecies != remainClusters)
							{
								tmpIndex = (int)takenVertices[dis];

								//calculating the average of the extended cluster
								//Accessing Hashtable O(1)
								Palette[tmpIndex].red = (byte)((Palette[tmpIndex].red * RGBCount[tmpIndex] + distinctColors[src].red) / ++RGBCount[tmpIndex]);
								Palette[tmpIndex].green = (byte)((Palette[tmpIndex].green * RGBCount[tmpIndex] + distinctColors[src].green) / ++RGBCount[tmpIndex]);
								Palette[tmpIndex].blue = (byte)((Palette[tmpIndex].blue * RGBCount[tmpIndex] + distinctColors[src].blue) / ++RGBCount[tmpIndex]);


								takenVertices.Add(src, (int)takenVertices[dis]);
							}
						}
					}
					//if remaining vertices = remaining clusters then each singel vertix is a cluster 
					else
					{
						//if src is not found in takenVertices then add a new cluster of 'src'
						if (!srcFound)
						{
							Palette[indexInPalette].red += distinctColors[src].red;
							Palette[indexInPalette].green += distinctColors[src].green;
							Palette[indexInPalette].blue += distinctColors[src].blue;

							takenVertices.Add(src, indexInPalette);

							indexInPalette++;
							remainClusters--;
						}
						//if dis is not found in takenVertices then add a new cluster of 'dis'
						else
						{
							Palette[indexInPalette].red += distinctColors[dis].red;
							Palette[indexInPalette].green += distinctColors[dis].green;
							Palette[indexInPalette].blue += distinctColors[dis].blue;

							takenVertices.Add(dis, indexInPalette);

							indexInPalette++;
							remainClusters--;
						}
					}
				}
				//number of remaining vertices = number of all vertices - number of taken vertices
				remainVertecies = distinctHashtable.Count - takenVertices.Count;
			}

		}
		/// <summary>
		/// Replaces the color of each pixel in the original matrix with the closest color in the palette
		/// </summary>
		/// <param name="M"></param>
		public void replaceWithPaletteColors(RGBPixel[,] M)
		{
			int indexInDistinct, indexOfCluster;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					//getting the index of the current color in the destincit colors
					indexInDistinct = vertixNumber[i, j];
					//getting the index of the cluster of the current color in the palette
					indexOfCluster = (int)takenVertices[indexInDistinct];
					//replacing the current color with the color of it's cluster
					M[i, j] = Palette[indexOfCluster];
				}

			}
		}
		public void TEST()
		{
			
			MSTPrim();
			/*************** IMPORTANT NOTE ***************
				
				before choosing the pictuer enter the number of clusters(K) 
				in the Gauss Sigma's textBox on the right of the form

 			*****************IMPORTANT NOTE **************/
			//	Cluster(3);
		}


	}
}
