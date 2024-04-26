using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace NipClip.Classes.NipLang
{
    class LanguageProcessor
    {
        public string Process(List<ClipboardEntry> list, string replacementString)
        {
            int i = 0;
            bool complete = false;
            string before = "";
            string middle = "";
            string after = "";

            int completeStartCounter = 0;
            int completeEndCounter = 0;

            while (i < replacementString.Count())
            {
                if (replacementString[i] == '$')
                {
                    if ((i + 1) < replacementString.Count() && replacementString[i + 1] == 'N')
                    {
                        if ((i + 2) < replacementString.Count() && (replacementString[i + 2] == '0' || replacementString[i + 2] == '1'))
                        {
                            complete = true;


                            if (replacementString[i + 2] == '0')
                            {
                                completeStartCounter = i + 2;
                                before = replacementString.Substring(0, i);
                                i++;
                                continue;
                            }

                            if (replacementString[i + 2] == '1')
                            {
                                completeEndCounter = i + 2;
                                after = replacementString.Substring(i + 3);
                                middle = replacementString.Substring(completeStartCounter + 1, i - (completeStartCounter + 1));
                                break;
                            }
                        }

                        before = replacementString.Substring(0, i);
                        after = replacementString.Substring(i + 2);
                        break;
                    }
                }
                i++;
            }

            if (complete)
            {
                string output = "";

                output += before;

                ClipboardEntry last = list.Last();
                foreach (ClipboardEntry element in list)
                {
                    output += element.Content;

                    if (!element.Equals(last))
                    {
                        output += middle;
                    }
                }
                output += after;
                output = output.Replace("\n", Environment.NewLine);

                return output;
            }
            else
            {
                List<string> output = new List<string>();

                foreach (ClipboardEntry element in list)
                {
                    output.Add(before + element.Content + after);
                }

                string outputString = "";

                foreach (string str in output)
                {
                    outputString += str;
                }
                outputString = outputString.Replace("\\n", Environment.NewLine);

                return outputString;
            }

            return "Error";
        }
    }
}
