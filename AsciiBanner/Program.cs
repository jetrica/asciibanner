using CommandLine;
using Figgle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace AsciiBanner
{
    class Program
    {
        static void Main(string[] args)
        {
            var result =
                Parser.Default.ParseArguments<MakeOptions, ListOptions>(args)
                .MapResult(
                  (MakeOptions options) => RunMake(options),
                  (ListOptions options) => RunList(options),
                  errors => HandleParsingErrors(errors));
           
        }

        private static int HandleParsingErrors(IEnumerable<Error> errs)
        {
            if (errs.Any(e => e is VersionRequestedError))
                return -1;
            if (errs.Any(e => e is HelpRequestedError))
            {
                Console.WriteLine("Usage examples:");
                Console.WriteLine();
                Console.WriteLine("AsciiBanner list | Shows all possible fonts that can be used for Ascii banner.");
                Console.WriteLine("AsciiBanner list -p 50 | Shows all possible fonts that can be used for Ascii banner, 50 fonts per page.");
                Console.WriteLine("AsciiBanner list -t \"My Text\" | Shows all possible fonts that can be used for Ascii banner with text \"My Text\".");
                Console.WriteLine("AsciiBanner make -t \"My Text\" | Creates banner with text \"My Text\" in Standard font.");
                Console.WriteLine("AsciiBanner make -t \"My Text\" -c | Creates banner with text \"My Text\" in Standard font and copies it to system clipboard.");
                Console.WriteLine("AsciiBanner make -t \"My Text\" -f Basic | Creates banner with text \"My Text\" in Basic font.");
                Console.WriteLine("AsciiBanner make -t \"My Text\" -m | Creates banner with text \"My Text\" in Standard font, surrounded with /* and */ c#/MSSQL comments.");
                return -1;
            }
            return -2;

        }

        private static int RunList(ListOptions options)
        {
            List<PropertyInfo> figgleFontsList = typeof(FiggleFonts).GetProperties(BindingFlags.Public | BindingFlags.Static).OrderBy(p => p.Name).ToList();

            var groupedFontList = figgleFontsList.Select((item, inx) => new { item, inx })
                            .GroupBy(x => x.inx / options.FontsPerPage)
                            .Select(g => g.Select(x => x.item));

            for (int i= 0; i< groupedFontList.Count(); i++)
            {
                foreach (var font in groupedFontList.ElementAt(i))
                {
                    var fontObject = font.GetValue(null);
                    var renderMethod = font.PropertyType.GetMethods().First(m => m.Name == "Render");
                    var renderedText = renderMethod.Invoke(fontObject, new object[] { options.SampleText,  0 });

                    Console.WriteLine(font.Name);
                    Console.WriteLine(renderedText);
                }


                int min = i * options.FontsPerPage + 1;
                int max = min + groupedFontList.ElementAt(i).Count() - 1;

                Console.WriteLine();
                Console.WriteLine("Available fonts {0}-{1} of {2}", min,max, figgleFontsList.Count);
                Console.WriteLine("Press <Enter> to continue");
                Console.ReadLine();
            }
            return 0;
        }

        private static int RunMake(MakeOptions opts)
        {
            string bannerString = string.Empty;
            var fontProp = typeof(FiggleFonts).GetProperties(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(f => f.Name.ToLower() == opts.Font.ToLower());

            if (fontProp == null)
            {
                Console.WriteLine("Wrong font name. Please run with list verb and specify existing font.");
                return -1;
            }
            else
            {
                var fontObject = fontProp.GetValue(null);
                var renderMethod = fontProp.PropertyType.GetMethods().First(m => m.Name == "Render");
                bannerString = renderMethod.Invoke(fontObject, new object[] { opts.Text, 0 }).ToString();
            }

            if (!string.IsNullOrEmpty(opts.Prefix) || !string.IsNullOrEmpty(opts.Suffix))
                bannerString = $"{opts.Prefix}{Environment.NewLine}{bannerString}{Environment.NewLine}{opts.Suffix}";

            if (opts.SurroundWithComment)
                bannerString = $"/*{Environment.NewLine}{bannerString}{Environment.NewLine}*/";

            if (opts.CopyToClipboard)
            {
                TextCopy.Clipboard.SetText(bannerString);
            }

            Console.WriteLine(bannerString);
            Console.WriteLine();
            return 0;
        }
    }
}
