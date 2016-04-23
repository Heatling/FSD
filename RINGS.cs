using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RINGSDrawing
{
	/// <summary>
	/// Represents a circle with a position and a raduis.
	/// </summary>
	public class Circle
	{
		public double CenterX	{ get; }
		public double CenterY	{ get; }
		public double Radius	{ get; }

		public Circle(double centerX, double centerY, double radius)
		{
			this.CenterX = centerX;
			this.CenterY = centerY;
			this.Radius = radius;
		}
		public override string ToString()
		{
			return "(X: " + this.CenterX + ", Y: " + this.CenterY + " , R: " + this.Radius + ")";
		}
	}

	class RINGS
	{
		static int drawn = 0;
		static int maxDraw = 300000;
		
		/// <summary>
		/// Makes a RINGS layout of the tree in the given node.
		/// </summary>
		/// <param name="root">The tree to draw</param>
		/// <param name="size">The size of the layout</param>
		/// <returns></returns>
		public static CircleNode MakeLayout(Tag root, double size)
		{
			CircleNode layout;
			double origin = size;
			
			if(root != null)
			{
				Circle tempRootPos;
				//Draw root first
				tempRootPos = new Circle(origin, origin, origin);
				//Console.WriteLine("Root position: "+tempRootPos);
				drawn++;
				layout = new CircleNode(tempRootPos, null,
					DrawChildrenOfNode(root, origin, origin, origin));
				return layout;
			}
			return null;
			
		}
		/// <summary>
		/// Draws the children of the given node, in the circle 
		/// parameters given.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="centerX"></param>
		/// <param name="centerY"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		static CircleNode[] DrawChildrenOfNode(Tag node, double centerX, double centerY, double radius)
		{
			Console.WriteLine("Layout progress: " + (++drawn * 100) / maxDraw);
			/*if (radius < 1)
			{
				return new CircleNode[] { };
			}*/
			int childrenDrawn = 0;
			Tag[] children = (Tag[])node.GetChildren();
			CircleNode[] childrenCircleNodes = new CircleNode[children.Length];
			//Console.WriteLine("Number of children: " + children.Length);
			int tempMaxChildrenInLevel;
			double direction, childRadius;

			//Console.WriteLine("Starting sort.");
			sortByNumberOfChildrenLargestFirst(children);
			//Console.WriteLine("Ending sort");
			while (childrenDrawn < children.Length)
			{
				tempMaxChildrenInLevel = findMaxChildrenInLevel(children, childrenDrawn);
				//Console.WriteLine("Max children in level: " + tempMaxChildrenInLevel);
				childRadius = radius - (radius/(Math.Sin(Math.PI/tempMaxChildrenInLevel)+1));
				//Console.WriteLine("Child Radius: " + childRadius);

				for (int i = 0; i<tempMaxChildrenInLevel; i++)
				{
					if (tempMaxChildrenInLevel == 1)
					{
						Circle c = new Circle(
							centerX,
							centerY,
							radius*0.9);
						//Console.WriteLine("Circle: " + c);
						childrenCircleNodes[i + childrenDrawn] =
							new CircleNode(c, children[i + childrenDrawn],
								DrawChildrenOfNode(children[i + childrenDrawn],
												c.CenterX, c.CenterY, c.Radius));
					}
					else
					{
						direction = 0 + (((2 * Math.PI) / tempMaxChildrenInLevel) * i);
						//Console.WriteLine("Direction: " + direction);
						Circle c = new Circle(
								centerX + Math.Cos(direction) * (radius - childRadius),
								centerY + Math.Sin(direction) * (radius - childRadius),
								childRadius);
						//Console.WriteLine("Circle: " + c);
						childrenCircleNodes[i + childrenDrawn] =
							new CircleNode(c, children[i + childrenDrawn],
								DrawChildrenOfNode(children[i + childrenDrawn],
												c.CenterX, c.CenterY, c.Radius));
					}
				}
				childrenDrawn += tempMaxChildrenInLevel;
				//Console.WriteLine("new children drawn: " + childrenDrawn);
				radius = radius - 2 * childRadius;
				//Console.WriteLine("New radius: " + radius);

			}
			return childrenCircleNodes;
		}

		/// <summary>
		/// Sorts the given lists of nodes so that the largest is 
		/// at the lowest position.
		/// The size of a node is specified by the number of direct children it has.
		/// </summary>
		/// <param name="nodes"></param>
		public static void sortByNumberOfChildrenLargestFirst(Node[] nodes)
		{
			Array.Sort(nodes, delegate (Node x, Node y) {
				return x.NumberOfChildren() - y.NumberOfChildren();
			});
			Array.Reverse(nodes);

		}

		/// <summary>
		/// Calculates the percentage area left, if the given number of circles
		/// occupy the periphery
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static double areaLeftInCenter(int n)
		{
			return Math.Pow(1 - Math.Sin(Math.PI / n), 2)/Math.Pow(1+Math.Sin(Math.PI/ n),2);
		}
		
		/// <summary>
		/// Calculates the number of nodes to put in the periphery based on the number of
		/// children they have.
		/// Only looks at the nodes at and after the given index.
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="firstChild"></param>
		/// <returns></returns>
		static int findMaxChildrenInLevel(Node[] nodes, int firstChild)
		{
			//Console.WriteLine("findMaxChildrenInLevel : nodes[" + nodes.Length +
			//"], firstChild[" + firstChild + "]");
			double tempAreaLeftInCenter, tempChildDecendentFraction, tempChildrenOfLevel, tempTotalChildren;
			for (int i = 3; i<nodes.Length-firstChild; i++)
			{
				//Console.WriteLine("i = " + i);
				tempAreaLeftInCenter = areaLeftInCenter(i);
				//Console.WriteLine("tempAreaLeftInCenter = " + tempAreaLeftInCenter);

				tempChildrenOfLevel = (double)numberOfDecendents(nodes, firstChild, firstChild + i);
				//Console.WriteLine("Children of level = " + tempChildrenOfLevel);
				tempTotalChildren = (double)numberOfDecendents(nodes, firstChild, nodes.Length);
				//Console.WriteLine("Total children = " + tempTotalChildren);

				if (tempChildrenOfLevel <= 0 || tempTotalChildren <= 0)
				{
					//Console.WriteLine("No children found, break");
					break;
				}

				tempChildDecendentFraction = tempChildrenOfLevel / tempTotalChildren;

				//Console.WriteLine("tempChildDecendentFraction = " + tempChildDecendentFraction);
				if (tempChildDecendentFraction >= 1.0 - tempAreaLeftInCenter)
				{
					return i;
				}
			}
			//Console.WriteLine("No k found, return " + (nodes.Length - firstChild));
			return nodes.Length-firstChild;
		}

		/// <summary>
		/// Summates the total number of direct children of the given
		/// list of nodes.
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="firstNode"></param>
		/// <param name="lastNodeExclusive"></param>
		/// <returns></returns>
		public static int numberOfChildren(Node[] nodes, int firstNode, int lastNodeExclusive)
		{
			int sum = 0;
			for(int i = firstNode; i<lastNodeExclusive; i++)
			{
				sum += nodes[i].NumberOfChildren();
			}
			return sum;
		}

		/// <summary>
		/// Calculates the sum of total decendents of the chosed nodes.
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="firstNode"></param>
		/// <param name="lastNodeExclusive"></param>
		/// <returns></returns>
		static int numberOfDecendents(Node[] nodes, int firstNode, int lastNodeExclusive)
		{
			int sum = 0;
			for(int i = firstNode; i<lastNodeExclusive; i++)
			{
				sum += nodes[i].NumberOfDecendents();
			}
			return sum;
		}
	}
}
