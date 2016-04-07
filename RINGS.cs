using System;
using System.Drawing;
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
		public Color FillColor	{ get; set; }

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
		public static CircleNode MakeLayout(Node root, double size)
		{
			CircleNode layout;
			double origin = size;
			
			if(root != null)
			{
				Circle tempRootPos;
				//Draw root first
				tempRootPos = new Circle(origin, origin, origin);
				tempRootPos.FillColor = Color.FromArgb(0xffffff);
				//Console.WriteLine("Root position: "+tempRootPos);
				drawn++;
				layout = new CircleNode(tempRootPos,
					DrawChildrenOfNode(root, origin, origin, origin, tempRootPos.FillColor));
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
		static CircleNode[] DrawChildrenOfNode(Node node, double centerX, double centerY, 
						double radius, Color parentColor)
		{
			Console.WriteLine("Layout progress: " + (++drawn * 100) / maxDraw);
			
			int childrenDrawn = 0;
			Node[] children = node.GetChildren();
			CircleNode[] childrenCircleNodes = new CircleNode[children.Length];
			//Console.WriteLine("Number of children: " + children.Length);
			int tempMaxChildrenInLevel;
			double direction, childRadius;

			//Console.WriteLine("Starting sort.");
			sortByNumberOfChildrenLargestFirst(children);
			//Console.WriteLine("Ending sort");
			
			Color childColor = getNextColor(parentColor);

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
							radius);
						c.FillColor = childColor;
						//Console.WriteLine("Circle: " + c);
						childrenCircleNodes[i + childrenDrawn] =
							new CircleNode(c,
								DrawChildrenOfNode(children[i + childrenDrawn],
												c.CenterX, c.CenterY, c.Radius, childColor));
					}
					else
					{
						direction = 0 + (((2 * Math.PI) / tempMaxChildrenInLevel) * i);
						//Console.WriteLine("Direction: " + direction);
						Circle c = new Circle(
								centerX + Math.Cos(direction) * (radius - childRadius),
								centerY + Math.Sin(direction) * (radius - childRadius),
								childRadius);
						c.FillColor = childColor;
						//Console.WriteLine("Circle: " + c);
						childrenCircleNodes[i + childrenDrawn] =
							new CircleNode(c,
								DrawChildrenOfNode(children[i + childrenDrawn],
												c.CenterX, c.CenterY, c.Radius, childColor));
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
		static void sortByNumberOfChildrenLargestFirst(Node[] nodes)
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
			double tempAreaTaken, tempChildDecendentFraction, tempChildrenOfLevel, tempTotalChildren;
			for (int i = 3; i<nodes.Length-firstChild; i++)
			{
				//Console.WriteLine("Area taken: i = " + i);
				tempAreaTaken = areaLeftInCenter(i);
				//Console.WriteLine("Area taken: tempAreaTaken = " + tempAreaTaken);

				tempChildrenOfLevel = (double)numberOfChildren(nodes, firstChild, firstChild + i);
				//Console.WriteLine("Children of level = " + tempChildrenOfLevel);
				tempTotalChildren = (double) numberOfChildren(nodes, firstChild, nodes.Length);
				//Console.WriteLine("Total children = " + tempTotalChildren);

				if (tempChildrenOfLevel <= 0 || tempTotalChildren <= 0)
				{
					//Console.WriteLine("No children found, break");
					break;
				}

				tempChildDecendentFraction = tempChildrenOfLevel / tempTotalChildren;

				//Console.WriteLine("tempChildDecendentFraction = " + tempChildDecendentFraction);
				if (tempChildDecendentFraction >= 1.0 - tempAreaTaken)
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
		static int numberOfChildren(Node[] nodes, int firstNode, int lastNodeExclusive)
		{
			int sum = 0;
			for(int i = firstNode; i<lastNodeExclusive; i++)
			{
				sum += nodes[i].NumberOfChildren();
			}
			return sum;
		}

		/// <summary>
		/// Returns the next color to use, given a starting color.
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public static Color getNextColor(Color col)
		{
			int newRed = (col.R / 2) +(col.G / 2);
			int newGreen = (col.G / 2);
			int newBlue = (col.B / 2) + (col.G / 2);
			return Color.FromArgb(newRed, newGreen, newBlue);
		}
	}
}
