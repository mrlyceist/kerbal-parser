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
				var kerbalRoot = ParseTree(sr);

				// Not a headless files - Split children into separate trees
				if (kerbalRoot.Values.Count == 0)
				{
					foreach (var tree in kerbalRoot.Children)
					{
						kerbalConfig.Add(tree);
					}
				}
				else
				{
					// Headless file
					kerbalConfig.Add(kerbalRoot);
				}
			}
			catch (ParseErrorException e)
			{
				throw new ParseErrorException(
					e.Message + "\nFile: " +
					kerbalConfig.FileName);
			}

			return kerbalConfig;
		}

		public KerbalNode ParseTree(StreamReader sr)
		{
			KerbalNode node;

			var headNodeName =
				Path.GetFileNameWithoutExtension(_configFile);

			if (headNodeName != null)
			{
				headNodeName = headNodeName.ToUpper();
			}

			if (ValidateNodeName(headNodeName))
			{
				node = new KerbalNode(headNodeName);
			}
			else
			{
				throw new ParseErrorException(
					"Parse error: Invalid node name \"" +
					headNodeName + "\" at, " + _lineNumber + ": " +
					_currentLine
					);
			}

			string line;
			string previousLine = null;
			var depth = 1;

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
							throw new ParseErrorException(
								"Parse error: Unexpected '{' at: " +
								_lineNumber + " " + _currentLine
								);
						}
					}

					if (!ValidateNodeName(nodeName))
					{
						throw new ParseErrorException(
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
						throw new ParseErrorException(
							"Parse error: Unexpected '=' sign at: " +
							_lineNumber + ", " + _currentLine);
					}

					if (tokens[1].Contains("}"))
					{
						var subtokens = tokens[1].Trim().Split('}');
						tokens[1] = subtokens[0];
						if (subtokens.Length > 2)
						{
							throw new ParseErrorException(
								"Parse error: Unexpected '}' sign at: " +
								_lineNumber + ", " + _currentLine);
						}
						line = "}" + subtokens[1];
					}

					var property = tokens[0].Trim();
					var value = tokens[1].Trim();

					if (String.IsNullOrEmpty(property))
					{
						throw new ParseErrorException(
							"Parse error: Unexpected '=' sign at: " +
							_lineNumber + ", " + _currentLine);
					}

					if (node == null)
					{
						throw new ParseErrorException(
							"Parse error: Unexpected property/value" +
							"outside node at: " + _lineNumber + ", " +
							_currentLine);
					}

					AddItems(property, value, node.Values);
				}

				if (line.Trim().Contains("}"))
				{
					if (node == null)
					{
						throw new ParseErrorException(
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
			if (depth > 2)
			{
				throw new ParseErrorException(
					"Parse Error: Missing matching bracket at: " +
					_lineNumber);
			}

			return node;
		}

		private bool ValidateNodeName(string nodeName)
		{
			string[] exceptions =
			{
				"running_closed",
				"running_open",
				"engage",
				"flameout",
				"atmosphereCurve",
				"powerCurve",
				"velocityCurve",
				"steeringCurve",
				"torqueCurve",
				"Thrust",
				"power_open",
				"power_closed",
				"indicators",
				"indicator",
				"increments",
				"ContractBackstory",
				"LeadIns",
				"Situations",
				"Excuses",
				"Circumstances",
				"Characters",
				"CharacterAttributes",
				"Predicates",
				"ObjectPredicates",
				"FactLeadIns",
				"Facts",
				"BriefingConclusions",
				"Bridges",
				"Adjectives",
				"Adverbs",
				"Adjunctives"
			};

			var n = nodeName.Trim();

			if (IsModuleManagerNode(n))
			{
				throw new ParseErrorException(
					"Parse error: Invalid node name (ModuleManager) \"" + n +
					"\". Are you tring to parse a ModuleManager config? " +
					"ModuleManager parsing is currently not supported. :" +
					_lineNumber + ", " + _currentLine);
			}

			return Regex.IsMatch(n, @"^[A-Z0-9_]+$") || exceptions.Contains(n);
		}

		private static bool IsModuleManagerNode(string nodeName)
		{
			var n = nodeName.Trim();

			return Regex.IsMatch(n, @"(^[@%+-])[\S]+");
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
