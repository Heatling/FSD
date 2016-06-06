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
		
		public static double[] getEvaluationFileRadius(CircleNode layout)
		{
			List<double> fileSizes = new List<double>();
			List<int> depths = new List<int>();

			addFilesToList(layout, fileSizes, depths, 0);

			if (fileSizes.Count() != depths.Count())
			{
				throw new EvaluationException("File sizes and depths dont sync");
			}
			
			fileSizes.Sort();

			double sizeSum = 0;

			for (int i = 0; i < fileSizes.Count(); i++)
			{
				sizeSum += fileSizes.ElementAt(i);
			}

			return new double[] {
				fileSizes.ElementAt(0),
				fileSizes.ElementAt(fileSizes.Count()/4),
				fileSizes.ElementAt(fileSizes.Count()/2),
				fileSizes.ElementAt((int)(fileSizes.Count()*0.75)),
				fileSizes.ElementAt(fileSizes.Count()-1),
				sizeSum / fileSizes.Count()
		};
		}
		
		public static double[] getEvaluationStaticness(CircleNode layout)
		{
			List<int> nodeStaticness = new List<int>();

			if (layout.GetChildren().Count() > 0)
			{
				calculateMutualStaticness((CircleNode[])layout.GetChildren(), nodeStaticness);
			}
			else
			{
				return null;
			}
			nodeStaticness.Sort();

			double sum = 0;
			foreach (int t in nodeStaticness)
			{
				sum += t;
			}
			
			return new double[] {
				nodeStaticness.ElementAt(0),
				nodeStaticness.ElementAt(nodeStaticness.Count()/4),
				nodeStaticness.ElementAt(nodeStaticness.Count()/2),
				nodeStaticness.ElementAt((int)(nodeStaticness.Count()*0.75)),
				nodeStaticness.ElementAt(nodeStaticness.Count()-1),
				sum / nodeStaticness.Count()
			};
		}

		public static double[] distanceBetweenRelativeValueAndSizeMedian(CircleNode layout)
		{
			List<double> distances = new List<double>();

			if (layout.GetChildren().Count() > 0)
			{
				calculateDistanceBetweenRelativeValueAndSize((CircleNode[])layout.GetChildren(), distances);
			}
			else
			{
				return null;
			}
			distances.Sort();
			double sum = 0;
			for (int i = 0; i < distances.Count(); i++)
			{
				sum += distances.ElementAt(i);
			}
			
			return new double[] {
				distances.ElementAt(0),
				distances.ElementAt(distances.Count()/4),
				distances.ElementAt(distances.Count()/2),
				distances.ElementAt((int)(distances.Count()*0.75)),
				distances.ElementAt(distances.Count()-1),
				sum / distances.Count()
			};

		}

		//Helper methods
		public static void calculateDistanceBetweenRelativeValueAndSize(CircleNode[] siblings, List<double> resultStore)
		{
			
			//Sort by child size
			Array.Sort(siblings, delegate (CircleNode x, CircleNode y)
			{
				return x.SourceTag.NumberOfChildren() - y.SourceTag.NumberOfChildren();
			});
			Array.Reverse(siblings);

			//Extract tags
			Tag[] siblingTags = new Tag[siblings.Count()];
			for (int i=0; i<siblings.Count(); i++)
			{
				siblingTags[i] = siblings[i].SourceTag;
			}
			if(siblingTags.Count() != siblings.Count())
			{
				throw new EvaluationException("calculateDistancesBetweenRelativevalueAndSize: tag array not correct length");
			}

			List<int> ChildrenInLevel = new List<int>();

			int index = 0;
			while(index < siblings.Count())
			{
				double currentLevelChildRadius = siblings[index].CircleValue.Radius;
				int currentLevelstartIndex = index;
				int currentLevelEndIndexExclusive;
				//Find all children in level
				for (currentLevelEndIndexExclusive = index + 1; currentLevelEndIndexExclusive<siblings.Count() && currentLevelChildRadius == siblings[currentLevelEndIndexExclusive].CircleValue.Radius; currentLevelEndIndexExclusive++) { }

				//Count children in level
				ChildrenInLevel.Add(currentLevelEndIndexExclusive - currentLevelstartIndex);

				int totalChildren = RINGS.numberOfChildren(siblingTags, 0, siblingTags.Count());

				for (int i = currentLevelstartIndex; i<currentLevelEndIndexExclusive; i++)
				{
					int k = ChildrenInLevel.Last();
					double relativeValue;
					if (totalChildren < 1)
					{
						relativeValue = 1.0 / (double) siblings.Count();
					}
					else
					{
						relativeValue = (double)RINGS.numberOfChildren(siblingTags, i, i + 1) / (double)totalChildren;
					}
					//calculate relative size
					
					double relativeSize ;
					relativeSize = 1;
					for(int j = 0; j<ChildrenInLevel.Count()-1;j++)
					{
						relativeSize *= RINGS.areaLeftInCenter(
							ChildrenInLevel.ElementAt(j));
					}
					if(k == 1)
					{
						relativeSize *= 0.9;
					}
					else
					{
						relativeSize *= (1 - RINGS.areaLeftInCenter(k)) / k;
					}
					
					
					if (relativeValue > relativeSize)
					{
						resultStore.Add(relativeValue - relativeSize);
					}
					else
					{
						resultStore.Add(relativeSize - relativeValue);
					}
					
				}
				index = currentLevelEndIndexExclusive;
			}
			foreach (CircleNode s in siblings)
			{
				if (s.GetChildren().Count() > 0)
				{
					calculateDistanceBetweenRelativeValueAndSize((CircleNode[])s.GetChildren(), resultStore);
				}
			}
		}

		public static void calculateMutualStaticness(CircleNode[] siblings, List<int> resultStore)
		{
			Tag[] siblingTags = new Tag[siblings.Length];
			for(int i = 0; i<siblings.Length; i++)
			{
				siblingTags[i] = siblings[i].SourceTag;
			}

			RINGS.sortByValueLargestFirst(siblingTags);

			for(int i = 1; i<siblingTags.Length; i++)
			{
				int stat = siblingTags[i - 1].NumberOfChildren()
						- siblingTags[i].NumberOfChildren();
				if (stat == 0)
				{
					resultStore.Add(1);
				}
				else
				{
					resultStore.Add(stat);
				}
				
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
