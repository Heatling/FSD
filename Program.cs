using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Xml;

namespace RINGSDrawing
{
	class Program
	{
		static void Main(string[] args)
		{
			//t4Main(args);
			t5Main(args);
			//Console.WriteLine("F(9)= " + RINGS.areaLeftInCenter(9));
			//Console.ReadKey();
		}
		
		/// <summary>
		/// Runs the RINGS algorithm on a small directory.
		/// </summary>
		/// <param name="args"></param>
		public static void t4Main(string[] args)
		{
			string t = "<drive name = \"C:\">\n" +
					"<directory name = \"d0\"> </directory>" +
					"<directory name = \"d0\"> </directory>" +
					"<directory name = \"d1\">Unauthorized</directory>" +
					"<directory name = \"d2\"><file name = \"file2\"></file><directory name = \"file3\"></directory></directory>" +
					"</drive>";

			
			Tag r = XMLReaderToTree.extractTreeFromXML(XmlReader.Create(new StringReader(t)));

			printTagAndChildren(r, 0);
			
			Console.ReadKey();
			CircleNode layout = RINGS.MakeLayout(r, 350);
			Console.WriteLine("Circles:");
			Circle tempC;

			Console.ReadKey();
			RINGSForm f = (new RINGSForm(700,700));
			f.Show();
			f.DrawAllCircles(layout);
			Console.ReadKey();
			//Console.WriteLine("Saving image");
			//f.Refresh();
			//f.drawToFile(@"C:\Users\Emad\Desktop\fileSystem.png");
			Console.ReadKey();
		}

		/// <summary>
		/// Loads the system creenshot and runs the RINGS algorithm, saving the result
		/// in a PNG file.
		/// </summary>
		/// <param name="args"></param>
		public static void t5Main(string[] args)
		{

			Tag r = XMLReaderToTree.extractDirectory(
				@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\RINGS-fig2-complete.xml", "");

			//printTagAndChildren(r, 0);
			int drawingSize =200;
			Console.WriteLine("Loaded tree.");
			CircleNode layout = RINGS.MakeLayout(r, drawingSize);
			Console.WriteLine("Created layout.");

			RINGSForm f = new RINGSForm(drawingSize*2, drawingSize * 2);
			f.Show();
			f.DrawAllCircles(layout);
			f.drawToFile(@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\FS SS 15-03-16.png");
			Console.ReadLine();

		}

		/// <summary>
		/// Prints a textual representation of the given node and its decendents.
		/// </summary>
		/// <param name="root"></param>
		/// <param name="indentation"></param>
		public static void printTagAndChildren(Tag root, int indentation)
		{
			string t = "";
			for(int i = 0; i<indentation; i++)
			{
				t += "\t";
			}

			String type, name;
			root.Properties.TryGetValue("type", out type);
			root.Properties.TryGetValue("name", out name);
			Console.WriteLine(t + "Type: " + type + ", name: " + name + "{");

			foreach(Tag child in root.GetChildren())
			{
				printTagAndChildren(child, indentation + 1);
			}

			Console.WriteLine(t + "}");
		}

	}
	
		
}
