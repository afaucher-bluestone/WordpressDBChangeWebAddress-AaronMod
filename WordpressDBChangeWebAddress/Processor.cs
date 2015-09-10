using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordpressDBChangeWebAddress
{
    public class Processor
    {
        private string InputFile { get; set; }
        private string OutFile { get; set; }
        private string InURL { get; set; }
        private string OutURL { get; set; }

		//	Create variables to count the number of replacements / email detections.
		public int regularReplaceCounter = 0;
		public int specialReplaceCounter = 0;
		public int emailDetectCounter = 0;

        public Processor(string inFile, string outfile, string inURL, string outURL)
        {
            InputFile = inFile;
            OutFile = outfile;
            InURL = inURL;
            OutURL = outURL;
        }

        public void ProcessChanges()
        {
			//create a variable fc to store all the text of the file
            string fc = File.ReadAllText(InputFile);

            // This line replaces special regex characters in the input URL with an escapted version so that it doesn't match more
            // literals than it should.
			// Create a variable 'lookForSpecial' that includes the special match of "s:..."
            var lookForSpecial = "s:([0-9]+)(:\"[^\"]*)"+ Regex.Escape(InURL) +"([^\"]*)\"";
			// Create a variable 'lookForEmail' that includes a preceding match of the @ sign
			var lookForEmail = "@" + Regex.Escape(InURL);
			// using statements "dispose" of their objects once the scope is complete
			// dispose of this new StreamWriter OutFile when done (?)
            using (var writer = new StreamWriter(OutFile))
            {
				// start the 'do while' loop
                do
                {
					// new Index var, to keep the position index at the point of the InURL
                    var positionNormal = fc.IndexOf(InURL);
					// new var , to keep the position index at the point of the "special" string (s:...) match
                    var positionSpecial = Regex.Match(fc, lookForSpecial);
					//  new var , to keep the position index at the point of the "email" string
					var positionEmail = Regex.Match(fc, lookForEmail);
					// new empty string
                    string outstring;

					// if there's a "special" (s:... match) and it's before the position of the URL match
                    if (positionSpecial.Success && (positionSpecial.Index < positionNormal || positionNormal == 0))
                    {
						// new variable w/ the output URL.
						// [  ] not sure what positionSpecial.Groups is doing here
                        var replaceString = positionSpecial.Groups[2] + OutURL + positionSpecial.Groups[3];
						// create a variable for the Opening quote position
                        var quotePos = replaceString.IndexOf("\"");
                        //var quotePos2 = replaceString.IndexOf("\"", quotePos + 1);

                        // We have found a string s:<n>:"xxxx"
						// Assign the value of outstring to:
						// s:<length of new string>"<new string>"
						// Note: the opening quote already sits in the new string (replaceString)
                        outstring = fc.Substring(0, positionSpecial.Index) + "s:" + (replaceString.Length - quotePos - 1) +
                                    replaceString + "\"";
                        fc = fc.Substring(positionSpecial.Index + positionSpecial.Value.Length);
						// Add one to the special replacement counter and write it to the console
						specialReplaceCounter++;
						Console.WriteLine("Special replacement (#" + specialReplaceCounter + ")");
					} else if (positionEmail.Success && (positionEmail.Index < positionNormal || positionNormal == 0)) // if an email address is found
					{
						// Assign the value of outstring to:
						// @<InURL>
						// (In other words, don't change it!)
						outstring = fc.Substring(0, positionEmail.Index) + "@" + InURL;
						// Then, move the fc index to the point AFTER the email address
						fc = fc.Substring(positionEmail.Index + positionEmail.Value.Length);
						// Add one to the email detection counter and write it to the console
						emailDetectCounter++;
						Console.WriteLine("Email address - skip (#" + emailDetectCounter + ")");

					} else if (positionNormal > 0) // next match is just a regular replacement
                    {
                        outstring = fc.Substring(0, positionNormal) + OutURL;
                        fc = fc.Substring(positionNormal + InURL.Length);
						// Add one to the regular replacement counter and write it to the console
						regularReplaceCounter++;
						Console.WriteLine("Regular replacement (#" + regularReplaceCounter + ")");
					}
                    else
                    {
                        //No matches left, just write the rest of the file!
                        outstring = fc;
                        fc = null;
					}
                    writer.Write(outstring);

				// do all this while fc is not null ...
				// which only happens when there are no matches left
                } while (! string.IsNullOrEmpty(fc));
            }


//            string newContents = Regex.Replace(fc, InURL, OutURL);

//            File.WriteAllText(OutFile, newContents);
        }
    }
}
