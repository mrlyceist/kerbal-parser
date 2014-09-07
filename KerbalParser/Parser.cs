using System;
using System.IO;
using System.Text.RegularExpressions;

namespace KerbalParser
{
	public class Parser
	{
		private int _lineNumber;
		private string _currentLine;

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
				_currentLine = line; // Used for error info only

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
								_lineNumber + " " + _currentLine
								);
						}
					}

					if (!ValidateNodeName(nodeName))
					{
						throw new Exception(
							"Parse error: Invalid node name \"" + nodeName +
							"\" at, " + _lineNumber + ": " + _currentLine
							);
					}

					if (tokens.Length > 1)
					{
						line = tokens[1];
					}

					var parentNode = node;

					node = new KerbalNode(nodeName, parentNode);

					depth++;
				}

				if (line.Trim().Contains("="))
				{
					var tokens = line.Trim().Split('=');

					if (tokens.Length != 2)
					{
						throw new Exception(
							"Parse error: Unexpected '=' sign at: " +
							_lineNumber + ", " + _currentLine);
					}

					if (tokens[1].Contains("}"))
					{
						var subtokens = tokens[1].Trim().Split('}');
						tokens[1] = subtokens[0];
						if (subtokens.Length > 2)
						{
							throw new Exception(
								"Parse error: Unexpected '}' sign at: " +
								_lineNumber + ", " + _currentLine);
						}
						line = "}" + subtokens[1];
					}

					var property = tokens[0].Trim();
					var value = tokens[1].Trim();

					if (String.IsNullOrEmpty(property) ||
					    String.IsNullOrEmpty(value))
					{
						throw new Exception(
							"Parse error: Unexpected '=' sign at: " +
							_lineNumber + ", " + _currentLine);
					}

					if (node == null)
					{
						throw new Exception(
							"Parse error: Unexpected property/value outside" +
							"node at: " + _lineNumber + ", " + _currentLine);
					}

					node.Values.Add(property, value);
				}

				if (line.Trim().Contains("}"))
				{
					if (node == null)
					{
						throw new Exception(
							"Parse error: Unexpected '}' sign at:" +
							_lineNumber + ", " + _currentLine);
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
