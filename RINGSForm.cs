using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RINGSDrawing
{

	public class CircleNode : StaticNode
	{
		public Circle CircleValue { get; }
		public CircleNode(Circle circle, CircleNode[] children): base(children)
		{
			this.CircleValue = circle;
		}
	}


	public partial class RINGSForm : Form
	{
		static int drawn = 0;
		static int maxDraw = 300000;
		static Image fileThumb = Image.FromFile(@"resources/images/file-thumbnail.png");

		Bitmap bitmap;
		
		/// <summary>
		/// Creates a window with the given height and width.
		/// </summary>
		/// <param name="height"></param>
		/// <param name="width"></param>
		public RINGSForm(int height, int width)
		{
			InitializeComponent();
			this.SetBounds(0, 0, height, width);
			bitmap = new Bitmap(height, width);
			Graphics.FromImage(bitmap).Clear(Color.White);
		}

		/// <summary>
		/// Draws all the given circles onto the window.
		/// Additionally draws lines between a node and its children.
		/// </summary>
		/// <param name="node"></param>
		public void DrawAllCircles(CircleNode node)
		{
			using (Graphics g = Graphics.FromImage(this.bitmap))
			{
				DrawCircle(node, g, Pens.Red);
			}
			this.CreateGraphics().DrawImage(bitmap, Point.Empty);
		}

		/// <summary>
		/// Recursively draws the given circle onto the given graphics in the given color.
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="graphics"></param>
		/// <param name="color"></param>
		public void DrawCircle(CircleNode node, System.Drawing.Graphics graphics, System.Drawing.Pen color)
		{
			Console.WriteLine("Drawing Progress: " + (drawn++ * 100) / maxDraw);
			System.Drawing.Rectangle tempRec;
			Circle c = node.CircleValue;

			//Console.WriteLine("Drawing circle: " + c);
			tempRec = new System.Drawing.Rectangle(
							(int)(c.CenterX - c.Radius), (int)(c.CenterY - c.Radius),
							2 * (int)(c.Radius), 2 * (int)(c.Radius));
			graphics.DrawEllipse(color, tempRec);

			Pen colorToUse = (color == Pens.Red) ? Pens.Blue : Pens.Red;

			foreach (CircleNode n in node.GetChildren())
			{
				if (n.NumberOfChildren() > 0)
				{
					DrawCircle(n, graphics, colorToUse);
				}
				else
				{
					
					if (n.CircleValue.Radius >= fileThumb.Height)
					{
						tempRec = new System.Drawing.Rectangle(
							(int)(n.CircleValue.CenterX - (fileThumb.Height/2)), 
							(int)(n.CircleValue.CenterY - (fileThumb.Height/2)),
							fileThumb.Height, fileThumb.Height);
						graphics.DrawImage(fileThumb, tempRec);
					}
					else
					{
						graphics.DrawLine(colorToUse, (int)c.CenterX, (int)c.CenterY,
								(int)n.CircleValue.CenterX, (int)n.CircleValue.CenterY);
					}
				}
			}
		}
		
		/// <summary>
		/// Saves the current bitmap drawing into the given file.
		/// </summary>
		/// <param name="fileName"></param>
		public void drawToFile(string fileName)
		{
			bitmap.Save(fileName, ImageFormat.Png);
		}

	}
}
