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
  Path to the next pem file to convert

Sample of how to generate PEM files:

openssl genrsa -out private.pem 2048
openssl rsa -in private.pem -outform PEM -pubout -out public.pem
"
		;

		public static int Display()
		{
			Console.WriteLine(USAGE, Process.GetCurrentProcess().MainModule.ModuleName);
			return -1;
		}
	}
}
