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

        [Option('w', "tile-width", Required = false, DefaultValue = 128, HelpText = "Width of single tile in pixels")]
        public int TileWidth { get; set; }

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
                IEnumerable<string> fileNames = FileSearchExpand.ExpandFileSearchPatterns(options.InputFiles);
                List<Image> imageList = new List<Image>();

                foreach (var fileName in fileNames)
                {
                    imageList.Add(Image.FromFile(fileName));
                }

                int imageCount = imageList.Count();
                double minAspect = imageList.Select(i => (double)i.Size.Width / (double)i.Size.Height).Min();
                int tileHeight = Convert.ToInt32(Math.Round(options.TileWidth / minAspect));
                int rows = imageCount / options.Columns + (imageCount % options.Columns > 0 ? 1 : 0);
                Image canvas = new Bitmap(options.TileWidth * options.Columns, rows * tileHeight);
                Graphics graphics = Graphics.FromImage(canvas);

                int row = 0;
                int col = 0;

                foreach (var i in imageList)
                {
                    graphics.DrawImage(i, col * options.TileWidth, row * tileHeight, options.TileWidth, tileHeight);
                    col = (col + 1) % options.Columns;

                    if (col == 0)
                    {
                        row += 1;
                    }
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
