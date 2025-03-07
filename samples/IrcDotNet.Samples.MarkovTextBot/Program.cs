﻿using System;
using IrcDotNet.Samples.Common;

namespace IrcDotNet.Samples.MarkovTextBot
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            IrcBot bot = null;

            try
            {
                // Write information about program.
                Console.WriteLine(ProgramInfo.AssemblyTitle);
                Console.WriteLine("Version {0}", ProgramInfo.AssemblyVersion);
                Console.WriteLine(ProgramInfo.AssemblyCopyright);
                Console.WriteLine();

                // Create and run bot.
                bot = new MarkovChainTextBot();
                bot.Run();
            }
#if !DEBUG
            catch (Exception ex)
            {
                ConsoleUtilities.WriteError("Fatal error: " + ex.Message);
            }
#endif
            finally
            {
                if (bot is not null)
                    bot.Dispose();
            }
        }
    }
}
