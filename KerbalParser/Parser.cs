using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KerbalParser
{
	/// <summary>
	/// Parser instance for generating KerbalConfig objects.
	/// User <code>ParseConfig</code> member to generate data from a cfg.
	/// </summary>
	public class Parser
	{
		private int _lineNumber;
		private string _currentLine;
		private string _configFile;

		public KerbalConfig ParseConfig(String configFile)
		{
			_lineNumber = 0;
			_currentLine = null;
			_configFile = configFile;

			var kerbalConfig = new KerbalConfig(configFile);

			var sr = new StreamReader(configFile);

			try
			{
				KerbalNode kerbalNode;
				while ((kerbalNode = ParseTree(sr)) != null)
				{
					kerbalConfig.Add(kerbalNode);
				}
			}
			catch (Exception e)
			{
				throw new Exception(
					e.Message + "\nFile: " +
					kerbalConfig.FileName);
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

					if (tokens.Length < 2)
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

					if (String.IsNullOrEmpty(property))
					{
						throw new Exception(
							"Parse error: Unexpected '=' sign at: " +
							_lineNumber + ", " + _currentLine);
					}

					if (node == null)
					{
						if (depth == 0)
						{
							var nodeName =
								Path.GetFileNameWithoutExtension(_configFile);

							if (nodeName != null)
							{
								nodeName = nodeName.ToUpper();
							}

							if (ValidateNodeName(nodeName))
							{
								node = new KerbalNode(nodeName);
							}
							else
							{
								throw new Exception(
									"Parse error: Invalid node name \"" +
									nodeName + "\" at, " + _lineNumber + ": " +
									_currentLine
									);
							}
						}
						else
						{
							throw new Exception(
								"Parse error: Unexpected property/value" +
								"outside node at: " + _lineNumber + ", " +
								_currentLine);
						}
					}

					AddItems(property, value, node.Values);
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

					// Remove first leading bracket
					if (line.Trim().Substring(0, 1).Equals("}"))
						line = line.Trim().Substring(1);

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

			// Parse error on missing matching bracket unless it's the last
			// bracket of the file, in which case the file is "closed"
			if (depth > 1)
			{
				throw new Exception(
					"Parse Error: Missing matching bracket at: " +
					_lineNumber);
			}

			return node;
		}

		private static bool ValidateNodeName(string nodeName)
		{
			return Regex.IsMatch(nodeName.Trim(), @"^[a-zA-Z_]+$");
		}

		private static void AddItems(
			string key,
			string value,
			IDictionary<string, List<string>> dictionary)
		{
			if (dictionary.ContainsKey(key) && dictionary[key] != null)
			{
				dictionary[key].Add(value);
			}
			else
			{
				var values = new List<string> { value };
				dictionary.Add(key, values);
			}
		}
	}
}
