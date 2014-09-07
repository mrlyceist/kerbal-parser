using System;
using System.IO;

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

					var parentNode = node;

					node = new KerbalNode(nodeName, parentNode);
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

					if (node == null)
					{
						throw new Exception(
							"Parse error: Unexpected property/value outside" +
							"node at: " + _lineNumber + ", " + line);
					}

					node.Values.Add(property, value);
				}

				if (line.Trim().Contains("}"))
				{
					if (node == null)
					{
						throw new Exception(
							"Parse error: Unexpected '}' sign at:" +
							_lineNumber + ", " + line);
					}

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
			return node;
		}
	}
}
