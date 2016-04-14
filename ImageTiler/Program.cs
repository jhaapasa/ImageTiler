using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        [Option('o', "output", Required = false, DefaultValue = "tile.png", HelpText = "Output filename for tilemap (PNG format only!")]
        public string OutputFilename { get; set; }

        [ValueList(typeof(List<string>), MaximumElements = 3)]
        public IList<string> InputFiles { get; set; }

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

                Image canvas = new Bitmap(512, 512);
                Graphics graphics = Graphics.FromImage(canvas);
                IEnumerable<string> fileNames = FileSearchExpand.ExpandFileSearchPatterns(options.InputFiles);
                foreach (var fileName in fileNames)
                {
                    Image img = Image.FromFile(fileName);
                    graphics.DrawImage(img, 0, 0);
                }

                canvas.Save(options.OutputFilename, ImageFormat.Png);
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
