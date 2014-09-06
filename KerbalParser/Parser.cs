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
								"Parse error: Unexpected '{' at: " +
								_lineNumber + " " + line
								);
						}
					}

					node = new KerbalNode(nodeName, _currentNode);

					if (_currentNode != null)
					{
						_currentNode.Children.Add(node);
					}

					_currentNode = node;

					ParseTree(sr);
				}

				if (line.Trim().Contains("="))
				{
					var tokens = line.Trim().Split('=');

					if (tokens.Length != 2)
					{
						throw new Exception(
							"Parse error: Unexpected '=' sign at: " +
							_lineNumber + ", " + line);
					}

					var property = tokens[0].Trim();
					var value = tokens[1].Trim();

					if (String.IsNullOrEmpty(property) ||
					    String.IsNullOrEmpty(value))
					{
						throw new Exception(
							"Parse error: Unexpected '=' sign at: " +
							_lineNumber + ", " + line);
					}

					if (_currentNode == null)
					{
						throw new Exception(
							"Parse error: Unexpected property/value outside" +
							"node at: " + _lineNumber + ", " + line);
					}

					_currentNode.Values.Add(property, value);
				}

				if (line.Trim().Contains("}"))
				{
					if (_currentNode == null)
					{
						throw new Exception(
							"Parse error: Unexpected '}' sign at:" +
							_lineNumber + ", " + line);
					}

					_currentNode = _currentNode.Parent;
				}

				previousLine = line;
			}
			return node;
		}
	}
}
