using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Xml;

namespace RINGSDrawing
{
	class Program
	{
		static void r1Main(string[] args)
		{
			Node[] children = new Node[9];
			for(int i = 0; i<children.Length; i++)
			{
				if(i <6)
				{
					children[i] = new StaticNode(
									new Node[] { new StaticNode(new Node[] { })
												, new StaticNode(new Node[] {
													new StaticNode(new Node[] { })
													, new StaticNode(new Node[] { })
												})
												, new StaticNode(new Node[] { })
												, new StaticNode(new Node[] { })
									});
				}
				else
				{

					children[i] = new StaticNode(new Node[] { });
				}
				
			}

			Node root = new StaticNode(children);
			CircleNode layout = RINGS.MakeLayout(root, 350);
			Console.WriteLine("Circles:");
			Circle tempC;
			
			Console.ReadKey();
			RINGSForm f = (new RINGSForm(700,700));
			f.Show();
			f.DrawAllCircles(layout);
			Console.ReadKey();
		}

		static void t2Main(string[] args)
		{
			string t = "<drive name = \"C:\">\n"+
				"<directory name = \"d1\">Unauthorized</directory>" +
				"<directory name = \"d2\"><file name = \"file2\"></file><directory name = \"file3\"></directory></directory>" +
					"</drive>";

			Tag r = new Tag(null);

			HomeMadeXMLReader.getTagFromSubstring(t, 0, r);

			foreach(Tag child in r.GetChildren())
			{
				printTagAndChildren(child, 0);
			}
			Console.ReadKey();
		}
		
		static void t3Main(String[] args)
		{
			//Real world test
			Tag cDrive = HomeMadeXMLReader.extractFileSystem(
				@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\fileSystem ss 26-02-16.xml", "");
			
			CircleNode layout = RINGS.MakeLayout(cDrive, 350);
			Console.WriteLine("Circles:");
			Circle tempC;

			Console.ReadKey();
			RINGSForm f = (new RINGSForm(700,700));
			f.Show();
			f.DrawAllCircles(layout);
			Console.ReadKey();
		}

		static void t4Main(string[] args)
		{
			string t = "<drive name = \"C:\">\n" +
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
			Console.WriteLine("Saving image");
			f.Refresh();
			f.drawToFile(@"C:\Users\Emad\Desktop\fileSystem.png");
			Console.ReadKey();
		}

		static void Main(string[] args)
		{

			Tag r = XMLReaderToTree.extractFileSystem(
				@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\FS SS 15-03-16.xml", "");

			//printTagAndChildren(r, 0);
			Console.WriteLine("Loaded tree.");
			CircleNode layout = RINGS.MakeLayout(r, 400);
			Console.WriteLine("Created layout.");
			Circle tempC;

			RINGSForm f = new RINGSForm(800,800);
			f.Show();
			f.DrawAllCircles(layout);
			f.drawToFile(@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\FS SS 15-03-16.png");
			Console.ReadKey();

		}

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
