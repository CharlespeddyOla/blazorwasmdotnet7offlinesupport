using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Helpers
{
    public class PINGenerator
    {
		private Random RandGen = new Random();
		public string Generate(int iLength = 6)
		{
			//Valid characters for the PIN.
			char[] cValidChars = "ABCDFGHJKLMNPQRSTVWXYZ0123456789".ToCharArray();
			string sGeneratedPIN = "";
			Regex letterMatch = new Regex(@"^[a-zA-Z]+$");
			Regex numberMatch = new Regex(@"^[0-9]+$");

			for (int i = 0; i < iLength; i++)
			{
				sGeneratedPIN += cValidChars[RandGen.Next(0, cValidChars.Length - 1)];
				if (letterMatch.IsMatch(sGeneratedPIN) || numberMatch.IsMatch(sGeneratedPIN))
				{
					if (i == iLength - 1)
					{
						//Invalid PIN, reset
						//Console.WriteLine(sGeneratedPIN);
						sGeneratedPIN = "";
						i = 0;
						//Console.WriteLine("Bad PIN");
					}
				}
			}

			return sGeneratedPIN;
		}
	}
}
