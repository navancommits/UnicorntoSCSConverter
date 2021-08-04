using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UnicorntoSCSConverter
{
    public partial class UnicorntoSCSConverter : System.Web.UI.Page
    {
        private string ModuleNameLine = string.Empty;
        private string referencesLine = string.Empty;
        private string startingLine = "{";
        private string itemsEndLine = "\r\t}";
        private string endLine = "\r}";
        private bool includesPresent=false;
        private string includeModuleName = string.Empty;
        private string includeModulePath = string.Empty;
        private string includeModuleDB = string.Empty;
        private int predicateStartLineNum ;
        private int predicateEndLineNum;
        private string strConvertedConcatIncludeLines;
        private string endArrayBracket;
        private string referenceName;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void GetPredicateLineNumbers()
        {
            string strConfigText = txtConfig.Text;
            string[] lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int intPredicateLineNumTracker = 0;

            foreach (var line in lstConfig)
            {
                intPredicateLineNumTracker += 1;
                if (line.ToLowerInvariant().Contains("<predicate>"))
                {
                    predicateStartLineNum = intPredicateLineNumTracker;
                    includesPresent = true;
                }

                if (line.ToLowerInvariant().Contains("</predicate>"))
                {
                    predicateEndLineNum = intPredicateLineNumTracker;
                }
            }
        }

        protected void btnConvert_Click(object sender, EventArgs e)
        {
            string convertedLine = string.Empty;
            if (string.IsNullOrWhiteSpace(txtConfig.Text)) return;

            GetPredicateLineNumbers();
            int intLineNumTracker = 0;
            string strConfigText = txtConfig.Text;
            string[] lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lstConfig)
            {
                intLineNumTracker += 1;
                CategorizeLine(line, intLineNumTracker);

                if (intLineNumTracker > predicateStartLineNum && intLineNumTracker < predicateEndLineNum)
                {
                    if (intLineNumTracker - 1 == predicateStartLineNum) strConvertedConcatIncludeLines += "\r\t\t" + "\"includes\": [";

                    strConvertedConcatIncludeLines += GetInfoforInclude(line, intLineNumTracker);
                }

                if (intLineNumTracker == predicateEndLineNum) endArrayBracket += "\r\t\t" + "]";
            }

            convertedLine = startingLine + "\r\t" + ModuleNameLine + "\r\t" + referencesLine + "\r\t" + "\"items\": {";

            convertedLine += strConvertedConcatIncludeLines;

            convertedLine += endArrayBracket;

            //closing brackets
            convertedLine += itemsEndLine + endLine;

            txtJson.Text = convertedLine;
        }

        private int IndexofnthOccurence(string mainString, char subString,int intOccurrence)
        {
            var str = mainString;
            var ch = subString;
            var n = intOccurrence;
            var result = str
                .Select((c, i) => new { c, i })
                .Where(x => x.c == ch)
                .Skip(n - 1)
                .FirstOrDefault();
            return result?.i ?? -1;
        }

        private string GetInfoforInclude(string currline,int lineNum)
        {
            string convertedLine = string.Empty;

            if (currline.ToLowerInvariant().Contains("include") && currline.ToLowerInvariant().Contains("name=") &&
                currline.ToLowerInvariant().Contains("database=") && currline.ToLowerInvariant().Contains("path="))
            {
                includeModuleName = ExtractValueBetweenQuotes(currline, "name=");
                includeModulePath = ExtractValueBetweenQuotes(currline, "path=");
                includeModuleDB = ExtractValueBetweenQuotes(currline, "database=");

                convertedLine += "\r\t\t\t{";

                convertedLine += "\r\t\t\t\t \"name\" : " + includeModuleName + ",";
                convertedLine += "\r\t\t\t\t \"path\" : " + includeModulePath + ",";
                convertedLine += "\r\t\t\t\t \"database\" : " + includeModuleDB;

                convertedLine += "\r\t\t\t}";

                if (lineNum + 1 < predicateEndLineNum) convertedLine += ",";
            }

            return convertedLine;
        }
        
        private void CategorizeLine(string currline,int intLineNum)
        {
            if (currline.ToLowerInvariant().Contains("configuration") && currline.ToLowerInvariant().Contains("name"))
            {
                var intFirst=IndexofnthOccurence(currline, '"', 1);
                var intSecond = IndexofnthOccurence(currline, '"', 2);

                var intStringLen = intSecond - intFirst;
                string modulename= currline.Substring(intFirst, intStringLen+1);

                ModuleNameLine= "\"namespace\": " + modulename + ",";
            }

            if (currline.ToLowerInvariant().Contains("dependencies"))
            {
                referenceName = ExtractValueBetweenQuotes(currline, "dependencies=");

                referencesLine = "\"references\": [" + referenceName + "],";
            }
        }

        private string ExtractValueBetweenQuotes(string currline, string subStringStart)
        {
            int intIncludeModuleName = currline.LastIndexOf(subStringStart, StringComparison.Ordinal);
            string stringforIncludeName = currline.Substring(intIncludeModuleName);

            var intFirst = IndexofnthOccurence(stringforIncludeName, '"', 1);
            var intSecond = IndexofnthOccurence(stringforIncludeName, '"', 2);

            var intStringLen = intSecond - intFirst;
            return stringforIncludeName.Substring(intFirst, intStringLen + 1);
        }
    }
}