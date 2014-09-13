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
		private int _skipDepth;
		private readonly bool _validateNodeNames;
		private readonly bool _ignoreModuleManager;

		public Parser(
			bool validateNodeNames = false,
			bool ignoreModuleManager = false)
		{
			_validateNodeNames = validateNodeNames;
			_ignoreModuleManager = ignoreModuleManager;
		}

		public KerbalConfig ParseConfig(String configFile)
		{
			_lineNumber = 0;
			_currentLine = null;
			_configFile = configFile;
			_skipDepth = -1;

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
			var headNodeName =
				Path.GetFileNameWithoutExtension(_configFile);

			if (headNodeName != null)
			{
				headNodeName = headNodeName.ToUpper();
			}

			// Don't validate head node since a file name can be anything
			var node = new KerbalNode(headNodeName);

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

				if (_skipDepth > -1 && _skipDepth < depth &&
				    line.Trim().Contains("{"))
				{
					depth++;
				}

				if (line.Trim().Contains("{") && _skipDepth < 0)
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

					if (IsModuleManagerNode(nodeName))
					{
						_skipDepth = depth;
						depth++;
					}
					else
					{
						if (!ValidateNodeName(nodeName))
						{
							throw new ParseErrorException(
								"Parse error: Invalid node name \"" +
								nodeName + "\" at, " + _lineNumber + ": " +
								_currentLine
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
				}

				if (line.Trim().Contains("=") && _skipDepth < 0)
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

				if (line.Trim().Contains("}") && _skipDepth < 0)
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

				if (_skipDepth > -1 && _skipDepth < depth &&
				    line.Trim().Contains("}"))
				{
					depth--;
					if (depth == _skipDepth)
					{
						_skipDepth = -1;
					}
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
			var n = nodeName.Trim();

			if (!_validateNodeNames)
			{
				return Regex.IsMatch(n, @"^[^{\s]+$");
			}

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

			return Regex.IsMatch(n, @"^[A-Z_]+$") || exceptions.Contains(n);
		}

		private bool IsModuleManagerNode(string nodeName)
		{
			if (_ignoreModuleManager) return false;

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
