using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RINGSDrawing
{
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
		
		public static CircleNode MakeLayout(Node root, double size)
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
				layout = new CircleNode(tempRootPos,
					DrawChildrenOfNode(root, origin, origin, origin));
				return layout;
			}
			return null;
			
		}

		static CircleNode[] DrawChildrenOfNode(Node node, double centerX, double centerY, double radius)
		{
			Console.WriteLine("Layout progress: " + (++drawn * 100) / maxDraw);
			if (radius < 1)
			{
				return new CircleNode[] { };
			}
			int childrenDrawn = 0;
			Node[] children = node.GetChildren();
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
							radius);
						//Console.WriteLine("Circle: " + c);
						childrenCircleNodes[i + childrenDrawn] =
							new CircleNode(c,
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
							new CircleNode(c,
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

		static void sortByNumberOfChildrenLargestFirst(Node[] nodes)
		{
			Array.Sort(nodes, delegate (Node x, Node y) {
				return x.NumberOfChildren() - y.NumberOfChildren();
			});
			Array.Reverse(nodes);

		}

		static double areaLeftInCenter(int n)
		{
			return Math.Pow(1 - Math.Sin(Math.PI / n), 2)/Math.Pow(1+Math.Sin(Math.PI/ n),2);
		}
		//
		// Summary:
		//		increase.
		//Returns:
		//		Returns.
		static int findMaxChildrenInLevel(Node[] nodes, int firstChild)
		{
			double tempAreaTaken, tempChildDecendentFraction;
			for (int i = 3; i<nodes.Length-firstChild; i++)
			{
				//Console.WriteLine("Area taken: i = " + i);
				tempAreaTaken = areaLeftInCenter(i);
				//Console.WriteLine("Area taken: tempAreaTaken = " + tempAreaTaken);
				try {
					tempChildDecendentFraction = numberOfChildren(nodes, firstChild, firstChild + i)
												/ numberOfChildren(nodes, firstChild, nodes.Length);
				}catch(DivideByZeroException err)
				{
					tempChildDecendentFraction = -1;
				}
				if (tempChildDecendentFraction >= tempAreaTaken)
				{
					return i;
				}
			}
			
			return nodes.Length-firstChild;
		}

		static int numberOfChildren(Node[] nodes, int firstNode, int lastNodeExclusive)
		{
			int sum = 0;
			for(int i = firstNode; i<lastNodeExclusive; i++)
			{
				sum += nodes[i].NumberOfChildren();
			}
			return sum;
		}
	}
}
