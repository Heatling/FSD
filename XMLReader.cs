using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RINGSDrawing
{
	
	/// <summary>
	/// Specifies an XML tag
	/// </summary>
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
			String type;
			this.Properties.TryGetValue("type", out type);
			if (this.Children.Count == 0 &&
				type.Equals("file"))
			{
				return 1;
			}
			else
			{
				return this.Children.Count;
			}
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
				sum += t.NumberOfDecendents();
			}
			return sum;
		}

		/// <summary>
		/// Specify a tag that is in the instaces tag.
		/// </summary>
		/// <param name="child"></param>
		public void addChildTag(Tag child)
		{
			this.Children.Add(child);
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
		/// <summary>
		/// Extracts the directory defined by the given path.
		/// </summary>
		/// <param name="fileLocation"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Tag extractDirectory(string fileLocation, string path)
		{
			using (XmlReader reader = XmlReader.Create(new FileStream(fileLocation, FileMode.Open)))
			{
				while (reader.Read())
				{
					reader.MoveToContent();
					if (reader.IsStartElement())
					{
						Tag r = extractTreeFromXML(reader.ReadSubtree());
						reader.Dispose();
						return r;
					}
				}
				reader.Dispose();
				throw new XMLMismatchException("XML file not formatted correctly.", reader);
			}
		}

		/// <summary>
		/// Extracts the directory tree the given xml reader is currently pointing to.
		/// When this function returns, the reader is pointing to the end of the tree 
		/// that was extracted.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
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
