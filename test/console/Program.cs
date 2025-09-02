// See https://aka.ms/new-console-template for more information

using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.ConsoleTests;
using Vertical.Cli.Conversion;

var rootCommand = new RootCommand("arc",
        helpTag: "Creates and manages archive files using GZip or Deflate compression with the option of levering " +
        "using AES (symmetric) or RSA (asymmetric) encryption protection.");


