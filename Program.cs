using System.CommandLine;

namespace SvgToPng;

class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Create the root command
        var rootCmd = new RootCommand("Tool to convert SVG files to PNG images");

        // Create path to files argument
        var pathArg = new Argument<string?>(
            name: "path",
            description: "Path to SVG files");
        
        pathArg.HelpName = "PATH_TO_SVGS";

        // Create PNG width and height options
        var widthOption = new Option<int?>(
            aliases: new[] { "-w", "--width" },
            description: "Width of PNG file.",
            getDefaultValue: () => 2048);

        widthOption.ArgumentHelpName = "PNG_WIDTH";

        var heightOption = new Option<int?>(
            aliases: new[] { "-h", "--height" },
            description: "Height of PNG file.",
            getDefaultValue: () => 2048);

        heightOption.ArgumentHelpName = "PNG_HEIGHT";

        // Create output option
        var outputOption = new Option<string?>(
            aliases: new[] { "-o", "--output" },
            description: "The output directory to save PNG files.");

        outputOption.ArgumentHelpName = "OUTPUT_DIR";

        
        // Add arguments and options to root command
        rootCmd.AddArgument(pathArg);
        rootCmd.AddOption(widthOption);
        rootCmd.AddOption(heightOption);
        rootCmd.AddOption(outputOption);
        

        return await rootCmd.InvokeAsync(args);
    }
}