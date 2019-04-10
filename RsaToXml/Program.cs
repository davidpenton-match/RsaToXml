using System;
using System.Collections.Generic;
using System.IO;

namespace RsaToXml
{
	class Program
	{
		static int Main(string[] args)
		{
			if (TryParseArgs(args, out var files) || VerifyFiles(files) == false)
				return Usage.Display();

			Runner.Run(files);

			return 0;
		}

		static bool TryParseArgs(string[] args, out string[] files)
		{
			files = null;
			List<string> lFiles = null;

			var l = args?.Length ?? 0;

			if (l == 0)
				return true;

			for (int i = 0; i < l; i++)
			{
				var a = args[i];
				if (a == "-h" || a == "--help")
					return true;

				if (lFiles == null)
					lFiles = new List<string> { a };
				else
					lFiles.Add(a);
			}

			files = lFiles.ToArray();
			return false;
		}

		static bool VerifyFiles(string[] files)
		{
			var l = files.Length;

			for(int i = 0; i < l; i++)
			{
				if (File.Exists(files[i]) == false)
				{
					Console.WriteLine("Error - file cannot be read:");
					Console.WriteLine(files[i]);
					return false;
				}
			}

			return true;
		}
	}
}
