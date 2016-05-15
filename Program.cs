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
			//t5Main(args);
			//evaluateFS_SS_15_03_16();	
			//evaluateFig2Complete();	
			evaluateScreenshots();
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
				file.WriteLine("File size\tStaticness\tValue/size distance");
				double[][] eval = evaluateLayout(layout);
				for (int i = 0; i < 6; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						file.Write(eval[j][i]);
						file.Write("\t");
					}
					file.Write("\n");
				}
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
				file.WriteLine("File size\tStaticness\tValue/size distance");
				double[][] eval = evaluateLayout(layout);
				for(int i = 0; i<6; i++)
				{ 
					for (int j =0; j<3; j++)
					{
						file.Write(eval[j][i]);
						file.Write("\t");
					}
					file.Write("\n");
				}
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
				"FS SS ZEINA",
				"FS SS STAT",
				"FS SS BABA"
			};

			int drawingSize = 8000;
			Tag r;
			CircleNode[] layouts = new CircleNode[screenshots.Length];
			double[][][] evaluations = new double[screenshots.Length][][];

			for (int i = 0; i<screenshots.Length; i++)
			{
				r = XMLReaderToTree.extractDirectory(
				screenshotPath+"\\" + screenshots[i] +".xml", "");
				layouts[i]  = RINGS.MakeLayout(r, drawingSize);
				evaluations[i] = evaluateLayout(layouts[i]);
			}

			double[][] compiledEvaluation = new double[3][];
			for(int i = 0; i<compiledEvaluation.Length; i++)
			{
				compiledEvaluation[i] = new double[6];
			}

			for(int i = 0; i<compiledEvaluation.Length; i++)
			{
				for(int j = 0; j<compiledEvaluation[0].Length; j++)
				{
					double sum = 0;
					for(int k = 0; k<evaluations.Length; k++)
					{
						sum += evaluations[k][i][j];
					}
					compiledEvaluation[i][j] = sum / evaluations.Length;
				}
			}

			using (System.IO.StreamWriter file =
			new System.IO.StreamWriter(@"C:\Users\Emad\Dropbox\DTU\Bachelor projekt\Drawing algorithms\Evaluations\evaluations-master-all.txt", false))
			{
				file.WriteLine("File size\tStaticness\tValue/size distance");
				for (int i = 0; i < compiledEvaluation[0].Length; i++)
				{
					for (int j = 0; j < compiledEvaluation.Length; j++)
					{
						file.Write(compiledEvaluation[j][i]);
						file.Write("\t");
					}
					file.Write("\n");
				}
			}
			Console.Write("DONE");
			Console.ReadLine();
		}

		/// <summary>
		/// Returns the evaluation results of the given layout.
		/// The results are formatted as follows:
		/// results[0] is file size results
		/// results[1] is staticness results
		/// results[3] is value/size distance results
		/// results[x][0] is the minimum
		/// results[x][1] is the first quantile (25%)
		/// results[x][2] is the second quantile (50% or median)
		/// results[x][3] is the third quantile (75%)
		/// results[x][4] is the maximum
		/// results[x][5] is the average
		/// </summary>
		/// <param name="layout"></param>
		/// <returns></returns>
		public static double[][] evaluateLayout(CircleNode layout)
		{
			double[] fileSizeQuantiles = Evaluations.getEvaluationFileRadius(layout);
			
			double[] staticnessQuantiles = Evaluations.getEvaluationStaticness(layout);

			double[] valueSizeDistancesQuantiles = Evaluations.distanceBetweenRelativeValueAndSizeMedian(layout);
			
			return new double[][]{fileSizeQuantiles, staticnessQuantiles, valueSizeDistancesQuantiles };
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
