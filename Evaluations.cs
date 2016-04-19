using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RINGSDrawing
{
	class Evaluations
	{
		public class EvaluationException : Exception
		{
			public EvaluationException(string message):base(message)
			{
				
			}
		}

		//Evaluations

		/// <summary>
		/// Gets the average of the size of each file as a fraction of its depth.
		/// </summary>
		/// <param name="layout"></param>
		/// <returns></returns>
		public static double getAvarageFileRadiusOverDepth(CircleNode layout)
		{
			
			List<double> fileSizes = new List<double>();
			List<int> depths = new List<int>();

			addFilesToList(layout, fileSizes, depths, 0);

			if(fileSizes.Count() != depths.Count())
			{
				throw new EvaluationException("File sizes and depths dont sync");
			}

			double sizeOverDepthSum = 0;

			for(int i = 0; i<fileSizes.Count(); i++)
			{
				sizeOverDepthSum += fileSizes.ElementAt(i)/depths.ElementAt(i);
			}

			return sizeOverDepthSum/fileSizes.Count();
		}

		public static double getMedianFileRadiusOverDepth(CircleNode layout)
		{
			List<double> fileSizes = new List<double>();
			List<int> depths = new List<int>();

			addFilesToList(layout, fileSizes, depths, 0);

			if (fileSizes.Count() != depths.Count())
			{
				throw new EvaluationException("File sizes and depths dont sync");
			}

			List<double> sizeOverDepth = new List<double>();

			for(int i = 0; i<fileSizes.Count(); i++)
			{
				sizeOverDepth.Add(fileSizes.ElementAt(i) / depths.ElementAt(i));
			}

			if(fileSizes.Count() != sizeOverDepth.Count())
			{
				throw new EvaluationException("Wrong number of fractions for size over depth");
			}
			sizeOverDepth.Sort();

			return sizeOverDepth.ElementAt((sizeOverDepth.Count() / 2)-1);
		}

		public static double getStaticnessAverage(CircleNode layout)
		{
			List<int> nodeStaticness = new List<int>();

			if(layout.GetChildren().Count() > 0)
			{
				calculateMutualStaticness((CircleNode[])layout.GetChildren(), nodeStaticness);
			}
			else
			{
				return -1;
			}

			double sum = 0;
			foreach(int t in nodeStaticness)
			{
				sum += t;
			}
			return sum / nodeStaticness.Count();
		}

		public static double getStaticnessMedian(CircleNode layout)
		{
			List<int> nodeStaticness = new List<int>();

			if (layout.GetChildren().Count() > 0)
			{
				calculateMutualStaticness((CircleNode[])layout.GetChildren(), nodeStaticness);
			}
			else
			{
				return -1;
			}
			nodeStaticness.Sort();

			return nodeStaticness.ElementAt(nodeStaticness.Count() / 2);
		}

		//Helper methods
		public static void calculateMutualStaticness(CircleNode[] siblings, List<int> resultStore)
		{
			RINGS.sortByNumberOfChildrenLargestFirst(siblings);

			for(int i = 1; i<siblings.Count(); i++)
			{
				resultStore.Add(siblings.ElementAt(i - 1).SourceTag.NumberOfChildren() 
						- siblings.ElementAt(i).SourceTag.NumberOfChildren());
			}

			foreach(CircleNode s in siblings)
			{
				calculateMutualStaticness((CircleNode[])s.GetChildren(), resultStore);
			}
		}


		/// <summary>
		/// If the given node is a file, extracts its radius and adds the given depth to the depth list.
		/// If it is a directory, recursively calls itself for each of its children.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="addToSizes"></param>
		/// <param name="addToDepths"></param>
		/// <param name="depthToAdd"></param>
		public static void addFilesToList(CircleNode layout, List<double> addToSizes, List<int> addToDepths, int depthToAdd)
		{
			if (layout.GetChildren().Count() > 0)
			{
				foreach (CircleNode n in layout.GetChildren())
				{
					addFilesToList(n, addToSizes, addToDepths, depthToAdd + 1);
				}
			}
			else
			{
				if (layout.SourceTag.Properties["type"].Equals("file"))
				{
					addToSizes.Add(layout.CircleValue.Radius);
					addToDepths.Add(depthToAdd);
				}
			}
		}

	}
}
