using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RINGSDrawing
{
	

	public class Tag : Node
	{
		public Dictionary<String, String> Properties { get; }
		private List<Tag> Children;

		public Tag(Dictionary<String,String> properties)
		{
			this.Properties = properties;
			Children = new List<Tag>();
		}

		public override int NumberOfChildren()
		{
			return this.Children.Count;
		}

		public override Node[] GetChildren()
		{
			return Children.ToArray();
		}

		public override int NumberOfDecendents()
		{
			int sum = this.NumberOfChildren();
			foreach(Tag t in Children)
			{
				sum = t.NumberOfDecendents();
			}
			return sum;
		}

		public void addChildTag(Tag child)
		{
			this.Children.Add(child);
		}
	}

	public class MatchException : Exception
	{
		public override string Message
		{
			get
			{
				if (this.prev != null)
				{
					return "At " + line + ", " + column + " " + base.Message + "\n" + prev.Message;
				}
				else
				{
					return "At " + line + ", " + column + " " + base.Message;
				}
			}
		}
		int line, column;
		MatchException prev;

		public MatchException(string message, String source, int errorAtChar) : base(message)
		{
			this.prev = null;
			line = 1;
			column = 1;
			for(int i = 0; i<errorAtChar; i++)
			{
				if(source.ElementAt(i) == '\n')
				{
					line++;
					column = 1;
				}
				else
				{
					column++;
				}
			}
		}
		
		public void addPrev(MatchException prev)
		{
			this.prev = prev;
		}
	}

	public class HomeMadeXMLReader
	{
		public static Tag extractFileSystem(string fileLocation, string path)
		{
			string text = System.IO.File.ReadAllText(fileLocation);

			Tag temp = new Tag(null);

			getTagFromSubstring(text, 0, temp);

			return (Tag) temp.GetChildren()[0];
		}

		public static int getTagFromSubstring(String source, int start, Tag insertTo)
		{
			int currentChar = start;
			string matched1;
			MatchException prev;

			currentChar = startToTrimedPos(source, currentChar);

			match(source, currentChar++, "<");

			currentChar = startToTrimedPos(source, currentChar);
			matched1 = match(source, currentChar, "drive", "directory", "file");
			currentChar += matched1.Count();

			switch (matched1)
			{
				case "drive":
					{
						Dictionary<String, String> d = new Dictionary<String, String>();
						d.Add("type", "drive");
						currentChar = matchProperty(source, currentChar, "name", d);
						match(source, currentChar++, ">");
						Tag newTag = new Tag(d);

						while (true)
						{
							try {
								currentChar = getTagFromSubstring(source, currentChar, newTag);
							}
							catch(MatchException err)
							{
								prev = err;
								break;
							}
						}

						insertTo.addChildTag(newTag);
						currentChar = startToTrimedPos(source, currentChar);
						try
						{
							match(source, currentChar, "</drive>");
						}
						catch (MatchException err)
						{
							err.addPrev(prev);
							throw err;
						}
						
						currentChar += "</drive>".Count();
						break;
					}
				case "directory":
					{
						Dictionary<String, String> d = new Dictionary<String, String>();
						d.Add("type", "directory");
						currentChar = matchProperty(source, currentChar, "name", d);
						match(source, currentChar++, ">");
						Tag newTag = new Tag(d);

						while (true)
						{
							try
							{
								currentChar = getTagFromSubstring(source, currentChar, newTag);
							}
							catch (MatchException err)
							{
								Console.WriteLine("Directory children failed. Trying unauthorized.");
								prev = err;
								try
								{
									currentChar = startToTrimedPos(source, currentChar);
									match(source, currentChar, "Unauthorized");
									currentChar += "Unauthorized".Count();
								}
								catch (MatchException err2)
								{
									Console.WriteLine("Unauthorized failed.");
									err2.addPrev(prev);
									prev = err2;
									break;
								}
								Console.WriteLine("Unauthorized succeeded.");
							}
						}

						insertTo.addChildTag(newTag);

						currentChar = startToTrimedPos(source, currentChar);
						try
						{
							match(source, currentChar, "</directory>");
						}
						catch (MatchException err)
						{
							err.addPrev(prev);
						}
						
						currentChar += "</directory>".Count();
						break;
					}
				case "file":
					{
						Dictionary<String, String> d = new Dictionary<String, String>();
						d.Add("type", "file");
						currentChar = matchProperty(source, currentChar, "name", d);
						match(source, currentChar++, ">");
						Tag newTag = new Tag(d);

						insertTo.addChildTag(newTag);
						currentChar = startToTrimedPos(source, currentChar);
						match(source, currentChar, "</file>");
						currentChar += "</file>".Count();
						break;
					}

			}
			
			return currentChar;
		}
		
		public static string match(string source, int charPos, params String[] values)
		{
			foreach(string t in values)
			{
				if (source.Substring(charPos).StartsWith(t))
				{
					return t;
				}
			}
			String message = "{";
			foreach(string t in values)
			{
				message += t + ",";
			}
			message = message.Substring(0, message.Count() - 1) + "}";
			throw new MatchException("Failed in matching " + message, source, charPos);
		}

		public static int matchProperty(string source, int startPos, string property,
									Dictionary<String, String> insertInto)
		{
			
			startPos = startToTrimedPos(source, startPos);
			match(source, startPos, property);
			startPos += property.Count();

			startPos = startToTrimedPos(source, startPos);
			match(source, startPos++, "=");

			startPos = startToTrimedPos(source, startPos);
			match(source, startPos++, "\"");

			int valueEndPos = source.Substring(startPos).IndexOf('\"');
			if(valueEndPos < 0)
			{
				throw new MatchException("No end to property value", source, startPos);
			}

			String propertyValue = source.Substring(startPos, valueEndPos);
			startPos += valueEndPos;

			match(source, startPos++, "\"");

			insertInto.Add(property, propertyValue);

			return startPos;
		}

		public static int startToTrimedPos(string source, int charPos)
		{
			while (	source.ElementAt(charPos) == ' ' ||
					source.ElementAt(charPos) == '\t' ||
					source.ElementAt(charPos) == '\r' ||
					source.ElementAt(charPos) == '\n')
			{
				charPos++;
			}
			return charPos;
		}
	}

	public class XMLMismatchException : Exception
	{
		int line, column;

		public override string Message
		{
			get
			{
				return "" + this.line + ":" + this.column + " " + base.Message;
			}
		}

		public XMLMismatchException(String message, int line, int column) : base(message)
		{
			this.line = line;
			this.column = column;
		}

		public XMLMismatchException(String message, XmlReader reader) : this(message, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition)
		{
		}
	}

	public class XMLReaderToTree
	{
		public static Tag extractFileSystem(string fileLocation, string path)
		{
			using (XmlReader reader = XmlReader.Create(new FileStream(fileLocation, FileMode.Open)))
			{
				while (reader.Read())
				{
					reader.MoveToContent();
					if (reader.IsStartElement())
					{
						return extractTreeFromXML(reader.ReadSubtree());
					}
				}
				throw new XMLMismatchException("XML file not formatted correctly.", reader);
			}
		}

		public static Tag extractTreeFromXML(XmlReader reader)
		{
			Tag result;
			reader.MoveToContent();

			switch (reader.NodeType)
			{
				case XmlNodeType.Element:
					{
						Dictionary<string, string> d = new Dictionary<string, string>();
						d.Add("type", reader.Name);
						

						for(int i = 0; i< reader.AttributeCount; i++)
						{
							reader.MoveToAttribute(i);
							d.Add(reader.Name, reader.Value);
						}
						reader.MoveToElement();
						result = new Tag(d);

						//read children
						if (reader.Read())
						{
							reader.MoveToContent();
							while (reader.NodeType != XmlNodeType.EndElement)
							{
								if (reader.NodeType == XmlNodeType.Text)
								{
									String temp = reader.ReadContentAsString().Trim();
									if (!temp.Equals("Unauthorized"))
									{
										throw new XMLMismatchException("Unknown text: " + temp, reader);
									}
									break;
								}
								else
								{


									Tag child = extractTreeFromXML(reader.ReadSubtree());
									if (child != null)
									{
										result.addChildTag(child);
									}
									if (reader.Read())
									{
										reader.MoveToContent();
									}
									else
									{
										throw new XMLMismatchException("Element was not closed properly", reader);
									}
								}
							}
						}
						else
						{
							throw new XMLMismatchException("Element was not closed propertly", reader);
						}
						

						//No more children
						//Should now be on the end element of the current element
						String type;
						d.TryGetValue("type",out type);

						if (reader.NodeType != XmlNodeType.EndElement || !reader.Name.Equals(type))
						{
							throw new XMLMismatchException("Expected end element '" + type + 
											"' but got '" + reader.Name + "'", reader);
						}
						break;
					}
				default:
					{
						throw new XMLMismatchException("Subelement not recognized", reader);
						break;
					}
			}
			reader.Close();
			return result;
		}
	}

}
