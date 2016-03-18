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
		Bitmap bitmap;
		
		public RINGSForm(int height, int width)
		{
			InitializeComponent();
			this.SetBounds(0, 0, height, width);
			bitmap = new Bitmap(height, width);
			Graphics.FromImage(bitmap).Clear(Color.White);
		}

		public void DrawAllCircles(CircleNode node)
		{
			using (Graphics g = Graphics.FromImage(this.bitmap))
			{
				DrawCircle(node, g, Pens.Red);
			}
			this.CreateGraphics().DrawImage(bitmap, Point.Empty);
		}

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
			foreach (CircleNode n in node.GetChildren())
			{
				
				if (color == Pens.Red)
				{
					DrawCircle(n, graphics, Pens.Blue);
					graphics.DrawLine(Pens.Blue, (int)c.CenterX, (int)c.CenterY,
								(int)n.CircleValue.CenterX, (int)n.CircleValue.CenterY);
				}
				else
				{
					DrawCircle(n, graphics, Pens.Red);
					graphics.DrawLine(Pens.Red, (int)c.CenterX, (int)c.CenterY,
								(int)n.CircleValue.CenterX, (int)n.CircleValue.CenterY);
				}
				
			}
		}
		
		public void drawToFile(string fileName)
		{
			bitmap.Save(fileName, ImageFormat.Png);
		}

	}
}
