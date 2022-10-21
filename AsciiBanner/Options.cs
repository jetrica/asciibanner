using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace AsciiBanner
{
    [Verb("make", HelpText = "Make banner", Hidden = false)]
    public class MakeOptions
    {
        [Option('t', longName:"Text", HelpText = "Text to make banner from.", Required = true)]
        public string Text { get; set; }

        [Option('f', longName: "Font", HelpText = "Font which will be used for rendering banner. View list of fonts with list verb.", Required = false, Default = "Standard")]
        public string Font { get; set; }

        [Option('c', longName:"CopyToClipboard", Default = false, HelpText ="Copy generated banner to clipboard.", Required = false)]
        public bool CopyToClipboard { get; set; }
        [Option('m', longName:"SurroundWithComment", Default = false, HelpText ="Surrond banner with block comment /*  */.", Required = false)]
        public bool SurroundWithComment { get; set; }
        [Option('p', longName:"Prefix", HelpText ="Prefix before banner, for example <pre>.", Required = false)]
        public string Prefix { get; set; }
        [Option('s', longName:"Suffix", HelpText ="Suffix after banner, for example </pre>.", Required = false)]
        public string Suffix { get; set; }
    }

    [Verb("list", HelpText = "List available fonts for banner", Hidden = false)]
    public class ListOptions
    {
        [Option('t', longName: "Text", Default = "Sample123.", HelpText = "Sample text to show in every font.", Required = false)]
        public string SampleText { get; set; }

        [Option('p', longName: "Page", Default = 20, HelpText = "How many fonts to show on one page.", Required = false)]
        public int FontsPerPage { get; set; }

    }
}
