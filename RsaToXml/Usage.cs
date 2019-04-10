using System;
using System.Diagnostics;

namespace RsaToXml
{
	public class Usage
	{
		const string USAGE =
@"
Usage: {0} [options]
Usage: {0} [pem-file] [next-pem-file] ...

Options:
  -h|--help        Display help.

pem-file:
  Path to the pem file to convert
next-pem-file
  Path to the next pem file to convert"
		;

		public static int Display()
		{
			Console.WriteLine(USAGE, Process.GetCurrentProcess().MainModule.ModuleName);
			return -1;
		}
	}
}
