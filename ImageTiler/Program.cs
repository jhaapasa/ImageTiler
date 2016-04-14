using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTiler
{
    public class Options
    {
        [Option('c', "columns", Required = false, DefaultValue = 4, HelpText = "Number of columns")]
        public int Columns { get; set; }

        [Option('w', "width", Required = false, DefaultValue = 1024, HelpText = "Width of tiled image in pixels")]
        public int Width { get; set; }

        [Option('o', "output", Required = false, DefaultValue = "tile.png", HelpText = "Output filename for tilemap (extension used for format selection")]
        public string OutputFilename { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        [ParserState]
        public IParserState LastParserState { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                ParseOptions(args, options);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
        }

        private static void ParseOptions(string[] args, Options options)
        {
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                foreach (var s in options.LastParserState.Errors)
                {
                    Console.WriteLine(s.ToString());
                }
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
        }

    }
}
