using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RINGSDrawing
{
	public abstract class Node
	{
		//Fields
		//Constructors
		public Node()
		{
		}

		//Abstract methods
		abstract public int NumberOfChildren();

		abstract public Node[] GetChildren();

		abstract public int NumberOfDecendents();
	}

	public class StaticNode: Node
	{
		//Fields
		private Node[] Chrildren { get; }

		//Constructors
		public StaticNode(Node[] children): base()
		{
			this.Chrildren = children;
		}

		//Methods
		public override int NumberOfChildren()
		{
			return this.Chrildren.Length;
		}

		public override Node[] GetChildren()
		{
			return this.Chrildren;
		}

		public override int NumberOfDecendents()
		{
			int sum = 0;
			foreach(Node child in this.Chrildren){
				sum += child.NumberOfDecendents();
			}
			return sum + this.NumberOfChildren();
		}
	}
}
