using System;
using System.Collections.Generic;
using System.IO;

namespace KerbalParser
{
	public class Parser
	{
		private KerbalNode _currentNode;
		private int _lineNumber;

		public IList<KerbalNode> ParseConfig(String configFile)
		{
			IList<KerbalNode> kerbalNodes = new List<KerbalNode>();

			var sr = new StreamReader(configFile);
			KerbalNode kerbalNode;

			while ((kerbalNode = ParseTree(sr)) != null)
			{
				kerbalNodes.Add(kerbalNode);
			}

			return kerbalNodes;
		}

		public KerbalNode ParseTree(StreamReader sr)
		{
			KerbalNode node = null;

			// Start reading the first line
			string line;
			string previousLine = null;

			while ((line = sr.ReadLine()) != null)
			{
				_lineNumber++;

				// Ignore comments and empty lines
				if (line.Trim().StartsWith("//") ||
				    String.IsNullOrEmpty(line))
				{
					continue;
				}

				Console.WriteLine(_lineNumber + ": " + line);

				if (line.Trim().Contains("{"))
				{
					var tokens = line.Trim().Split('{');

					var nodeName = tokens[0].Trim();

					if (String.IsNullOrEmpty(nodeName))
					{
						if (previousLine != null)
						{
							nodeName = previousLine.Trim();
						}
						else
						{
							throw new Exception(
								"Parse error: " + "Line: " +
								_lineNumber + " " + line
								);
						}
					}

					node = new KerbalNode(nodeName, _currentNode);

					_currentNode = node;

					Console.WriteLine(">> GOING DEEPER! : " + node);
					Console.WriteLine(">> CURRENT NODE : " + _currentNode.Name);
					ParseTree(sr);
				}

				previousLine = line;
			}
			return node;
		}
	}
}
