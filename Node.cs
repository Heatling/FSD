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

		/// <summary>
		/// Returns the number of direct children of the instance.
		/// </summary>
		/// <return></return>
		abstract public int NumberOfChildren();

		/// <summary>
		/// The current children of the instance.
		/// </summary>
		/// <returns>
		///
		/// </returns>
		abstract public Node[] GetChildren();

		/// <summary>
		/// The number of total decendants of this node. 
		/// Decendents are the children and their children, and so on recursively.
		/// </summary>
		/// <returns>
		/// 
		/// </returns>
		abstract public int NumberOfDecendents();
	}

	public class StaticNode: Node
	{
		//Fields
		/// <summary>
		/// The children of the the instance.
		/// </summary>
		private Node[] Chrildren { get; }

		//Constructors
		/// <summary>
		/// Create a new instance with the given children.
		/// </summary>
		/// <param name="children"></param>
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
