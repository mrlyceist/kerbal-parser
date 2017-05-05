using KerbalParser;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KerbalParserTests
{
    [TestFixture]
    public class FullFolderTest
    {
        private List<KerbalConfig> _configs;
        private int _fileCount;

        private const string BaseDir = "d:\\else\\Kerbal Space Program\\GameData";
        //"C:\\Program Files (x86)\\Steam\\SteamApps\\common\\" +
        //"Kerbal Space Program\\GameData";

        [SetUp]
        public void Init()
        {
            //_fileCount = 0;

            //var dir = new DirectoryInfo(BaseDir);

            _configs = new List<KerbalConfig>();
            //var parser = new Parser(false, true);

            //foreach (var file in
            //    dir.GetFiles("*.cfg", SearchOption.AllDirectories))
            //{
            //    _fileCount++;
            //    _configs.Add(parser.ParseConfig(file.FullName));
            //}
        }

        [TearDown]
        public void Cleanup() { }

        [Test]
        public void TestFileCount()
        {
            _fileCount = 0;

            var dir = new DirectoryInfo(BaseDir);

            _configs = new List<KerbalConfig>();
            var parser = new Parser(false, true);

            foreach (var file in
                dir.GetFiles("*.cfg", SearchOption.AllDirectories))
            {
                if (file.Name == "FireSpitter_TweakScale.cfg") continue;
                _fileCount++;
                _configs.Add(parser.ParseConfig(file.FullName));
            }

            Console.WriteLine(_configs.Count);
            Assert.AreEqual(_fileCount, _configs.Count);
        }

        //[Test]
        //public void TestHasContent()
        //{
        //    foreach (var config in _configs)
        //    {
        //        Console.WriteLine(
        //                          config.FileName + " : " + config.Count +
        //                          " Trees");

        //        Assert.Greater(config.Count, 0, config.FileName);
        //    }
        //}

        [Test]
        public void TestRandomValues()
        {
            var allFileNames = _configs.Select(config => config.FileName)
                                       .ToList();

            string[] filesToTest =
            {
                BaseDir + "\\Squad\\Resources\\ResourcesGeneric.cfg",
                //BaseDir + "\\NASAmission\\Parts\\LaunchEscapeSystem\\part.cfg",
                BaseDir + "\\Squad\\Parts\\Wheel\\roverWheel1\\part.cfg",
                BaseDir + "\\Squad\\Props\\throttle\\prop.cfg"
            };

            foreach (var file in filesToTest)
            {
                Assert.Contains(file, allFileNames);
            }

            var configsToTest = _configs.Where
                (config => filesToTest.Contains(config.FileName)).ToList();

            Assert.AreEqual(filesToTest.Length, configsToTest.Count);

            foreach (var config in configsToTest)
            {
                switch (config.FileName)
                {
                    case BaseDir +
                         "\\Squad\\Resources\\ResourcesGeneric.cfg":
                        Assert.
                            IsTrue(config.First().Values.ContainsKey("name"));
                        Assert.
                            AreEqual(
                                     "LiquidFuel",
                                     config.First().Values["name"].First());
                        break;
                    case BaseDir +
                         "\\NASAmission\\Parts\\LaunchEscapeSystem\\part.cfg":
                        Assert.
                            Greater(config.First().Children.Count, 0);

                        var child = config.First().Children.First();
                        Assert.
                            Greater(child.Children.Count, 0);

                        var subchild = child.Children.First();
                        Assert.
                            Greater(subchild.Children.Count, 0);

                        var subsubchild = subchild.Children.First();
                        Console.WriteLine(
                                          "\\Squad\\Parts\\Wheel\\" +
                                          "roverWheel1\\part.cfg");
                        Console.WriteLine(subsubchild);
                        Console.WriteLine("-------------------");
                        Assert.
                            IsTrue(subsubchild.Values.ContainsKey("clip"));
                        Assert.
                            AreEqual(
                                     "sound_rocket_spurts",
                                     subsubchild.Values["clip"].First());
                        break;
                    case BaseDir +
                         "\\Squad\\Parts\\Wheel\\roverWheel1\\part.cfg":
                        Assert.
                            IsTrue(config.First().Values.ContainsKey("cost"));
                        Assert.
                            AreEqual(
                                     "450",
                                     config.First().Values["cost"].First());
                        Assert.
                            AreEqual(
                                     3,
                                     config.First().Children.Count);
                        break;
                    case BaseDir +
                         "\\Squad\\Props\\throttle\\prop.cfg":

                        Console.WriteLine(
                                          "\\Squad\\Parts\\Wheel\\" +
                                          "roverWheel1\\part.cfg");
                        foreach (var tree in config)
                        {
                            Console.WriteLine(tree);
                        }
                        Console.WriteLine("-------------------");

                        Assert.AreEqual("PROP", config.First().Name);
                        Assert.
                            IsTrue(config.First().Values.ContainsKey("proxy"));
                        Assert.
                            AreEqual(
                                     "0, 0, 0, 	0.0225, 0.0075, 0.0225, 	0, 1, 0",
                                     config.First().Values["proxy"].First());
                        break;
                }
            }
        }
    }
}
