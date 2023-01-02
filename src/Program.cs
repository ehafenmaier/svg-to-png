using Svg;
using System.CommandLine;
using System.Drawing.Imaging;

namespace SvgToPng;

class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Create the root command
        var rootCmd = new RootCommand("Tool to convert SVG files to PNG images");

        // Create path to files argument
        var pathArg = new Argument<DirectoryInfo>(
            name: "path",
            description: "Path to SVG files")
        {
            HelpName = "PATH_TO_SVGS"
        };

        // Create PNG width and height options
        var widthOption = new Option<int>(
            aliases: new[] { "-w", "--width" },
            description: "Width of PNG file.",
            getDefaultValue: () => 2048)
        {
            ArgumentHelpName = "PNG_WIDTH",
            IsRequired = false
        };

        var heightOption = new Option<int>(
            aliases: new[] { "-h", "--height" },
            description: "Height of PNG file.",
            getDefaultValue: () => 2048)
        {
            ArgumentHelpName = "PNG_HEIGHT",
            IsRequired = false
        };

        // Create output option
        var outputOption = new Option<DirectoryInfo?>(
            aliases: new[] { "-o", "--output" },
            description: "The output directory to save PNG images.\n" + 
                         "If --output is not set the PNG image will be saved next to SVG file.")
        {
            ArgumentHelpName = "OUTPUT_DIR",
            IsRequired = false
        };


        // Add arguments and options to root command
        rootCmd.AddArgument(pathArg);
        rootCmd.AddOption(widthOption);
        rootCmd.AddOption(heightOption);
        rootCmd.AddOption(outputOption);

        // Create handler for root command
        rootCmd.SetHandler((svgPath, pngWidth, pngHeight, pngPath) =>
        {
            ProcessFiles(svgPath, pngWidth, pngHeight, pngPath);
        }, 
            pathArg, 
            widthOption, 
            heightOption, 
            outputOption);

        return await rootCmd.InvokeAsync(args);
    }
    
    /// <summary>
    /// Process SVG files
    /// </summary>
    /// <param name="path">Path to SVG files</param>
    /// <param name="width">Width of PNG image</param>
    /// <param name="height">Height of PNG image</param>
    /// <param name="output">Path to output PNG files</param>
    private static void ProcessFiles(
        DirectoryInfo path,
        int width, 
        int height, 
        DirectoryInfo? output)
    {
        var subDirectories = path.GetDirectories();
        foreach (var subDir in subDirectories)
        {
            ProcessFiles(subDir, width, height, output);
        }

        Console.WriteLine("Processing files in directory: {0}", path.FullName);

        var svgFiles = path.GetFiles("*.svg");
        foreach (var file in svgFiles)
        {
            // Get file name without extension using range operator
            var fileName = file.Name[..^4];
            // Construct PNG image name from file name
            var imageName = fileName + ".png";
            // Open SVG file
            var svgDoc = SvgDocument.Open<SvgDocument>(file.FullName, null);
            // Draw bitmap image
            var bitmap = svgDoc.Draw(width, height);
            // Define output path to save PNG image
            var outputPath = output?.FullName ?? file.DirectoryName;
            // Save PNG image
            bitmap.Save(Path.Combine(outputPath, imageName), ImageFormat.Png);
            
            Console.WriteLine("PNG created: {0}", Path.Combine(outputPath, imageName));
        }
    }
}