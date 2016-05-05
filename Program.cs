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
			//evaluateFS_SS_15_03_16();	
			//evaluateFig2Complete();	
			//evaluateScreenshots();
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
				@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\FS SS 15-03-16.xml", "");

			//printTagAndChildren(r, 0);
			int drawingSize =8000;
			Console.WriteLine("Loaded tree.");
			CircleNode layout = RINGS.MakeLayout(r, drawingSize);
			Console.WriteLine("Created layout.");

			RINGSForm f = new RINGSForm(drawingSize*2, drawingSize * 2);
			f.Show();
			f.DrawAllCircles(layout);
			f.drawToFile(@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\FS SS 15-03-16.png");
			Console.ReadLine();

		}

		public static void evaluateFS_SS_15_03_16()
		{
			Tag r = XMLReaderToTree.extractDirectory(
				@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\FS SS 15-03-16.xml", "");
			int drawingSize = 8000;
			Console.WriteLine("Loaded tree.");
			CircleNode layout = RINGS.MakeLayout(r, drawingSize);
			Console.WriteLine("Created layout.");
			using (System.IO.StreamWriter file =
			new System.IO.StreamWriter(@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\Drawing algorithms\Evaluations\evaluations-FS SS 15-03-16.txt", false))
			{
				evaluateLayout(layout, file);
			}
			Console.Write("DONE");
			Console.ReadLine();
		}
		
		public static void evaluateFig2Complete()
		{
			Tag r = XMLReaderToTree.extractDirectory(
				@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots\RINGS-fig2-complete.xml", "");
			int drawingSize = 8000;
			Console.WriteLine("Loaded tree.");
			CircleNode layout = RINGS.MakeLayout(r, drawingSize);
			Console.WriteLine("Created layout:");
			using (System.IO.StreamWriter file =
			new System.IO.StreamWriter(@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\Drawing algorithms\Evaluations\evaluations-Fig2Complete.txt", false))
			{
				evaluateLayout(layout, file);
			}
			Console.Write("DONE");
			Console.ReadLine();
		}

		public static void evaluateScreenshots()
		{
			string screenshotPath = @"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\File system screenshots";
			string[] screenshots = new string[] {
				"FS SS 15-03-16",
				"FS SS MOM",
				"FS SS ZEINA"
			};

			int drawingSize = 800;
			Tag r;
			CircleNode[] layouts = new CircleNode[screenshots.Length];

			for (int i = 0; i<screenshots.Length; i++)
			{
				r = XMLReaderToTree.extractDirectory(
				screenshotPath+"\\" + screenshots[i] +".xml", "");
				layouts[i]  = RINGS.MakeLayout(r, drawingSize);
			}
			using (System.IO.StreamWriter file =
			new System.IO.StreamWriter(@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\Drawing algorithms\Evaluations\evaluations-LessThanOneMinusF-all.txt", false))
			{
				for (int i = 0; i < layouts.Length; i++)
				{
					file.WriteLine("Evaluation of '" + screenshots[i] + "':");
					evaluateLayout(layouts[i], file);
					file.WriteLine();
				}
			}
			Console.Write("DONE");
			Console.ReadLine();
		}

		public static void evaluateLayout(CircleNode layout, StreamWriter resultOutput)
		{
			double fileSizeOverDepthAverage = Evaluations.getAvarageFileRadiusOverDepth(layout);
			double[] fileSizeOverDepthQuantiles = Evaluations.getMedianFileRadiusOverDepth(layout);
			
			double staticnessAverage = Evaluations.getStaticnessAverage(layout);
			double[] staticnessQuantile = Evaluations.getStaticnessMedian(layout);

			double valueSizeDistancesAverage = Evaluations.distanceBetweenRelativeValueAndSizeAverage(layout);
			double[] valueSizeDistancesQuantiles = Evaluations.distanceBetweenRelativeValueAndSizeMedian(layout);

			string average = "average";
			string[] quantiles = new string[] { "Min", "Q1", "Q2", "Q3", "Max" };

			resultOutput.WriteLine("Size over Depth\tStaticness\tValue/size distance");
			for(int i = 0; i<5; i++)
			{
				resultOutput.WriteLine(fileSizeOverDepthQuantiles[i] + "\t" + staticnessQuantile[i] + "\t" + valueSizeDistancesQuantiles[i]);
			}
			resultOutput.WriteLine(fileSizeOverDepthAverage + "\t" + staticnessAverage + "\t" + valueSizeDistancesAverage);

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

		public static string arrayValuesToString(double[] array)
		{
			string result = "[";
			for(int i = 0; i<array.Length;i++)
			{
				result += array[i] + ", ";
			}
			return result.Substring(0, result.Length - 2)+"]";
		}

	}
	
		
}
