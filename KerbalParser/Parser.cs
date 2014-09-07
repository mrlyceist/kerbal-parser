using System;
using System.IO;
using System.Text.RegularExpressions;

namespace KerbalParser
{
	public class Parser
	{
		private int _lineNumber;

		public KerbalConfig ParseConfig(String configFile)
		{
			var kerbalConfig = new KerbalConfig(configFile);

			var sr = new StreamReader(configFile);
			KerbalNode kerbalNode;

			while ((kerbalNode = ParseTree(sr)) != null)
			{
				kerbalConfig.Add(kerbalNode);
			}

			return kerbalConfig;
		}

		public KerbalNode ParseTree(StreamReader sr)
		{
			KerbalNode node = null;

			string line;
			string previousLine = null;
			var depth = 0;

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

					if (!ValidateNodeName(nodeName))
					{
						throw new Exception(
							"Parse error: Invalid node name \"" + nodeName +
							"\" at, " + _lineNumber + ": " + line
							);
					}

					var parentNode = node;

					node = new KerbalNode(nodeName, parentNode);

					depth++;
				}

				else if (line.Trim().Contains("="))
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

					if (node == null)
					{
						throw new Exception(
							"Parse error: Unexpected property/value outside" +
							"node at: " + _lineNumber + ", " + line);
					}

					node.Values.Add(property, value);
				}

				else if (line.Trim().Contains("}"))
				{
					if (node == null)
					{
						throw new Exception(
							"Parse error: Unexpected '}' sign at:" +
							_lineNumber + ", " + line);
					}

					depth--;

					// Reached the end of current tree start reading the
					// next one.
					if (node.Parent == null)
					{
						return node;
					}

					node = node.Parent;
				}

				previousLine = line;
			}

			if (depth > 0)
			{
				throw new Exception(
					"Parse Error: Missing matching bracket at: " +
					_lineNumber);
			}

			return node;
		}

		private static bool ValidateNodeName(string nodeName)
		{
			return Regex.IsMatch(nodeName.Trim(), @"^[a-zA-Z]+$");
		}
	}
}
