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
        private bool excludesPresent = false;
        private string includeModuleName = string.Empty;
        private string includeModulePath = string.Empty;
        private string includeModuleDB = string.Empty;
        private int predicateStartLineNum ;
        private int predicateEndLineNum;
        private string strConvertedConcatIncludeLines;
        private string endArrayBracket;
        private string referenceName;
        private string[] lstConfig;
        private string ruleList;
        private int intLineNumTracker=0;
        private int intlastInclude = 0;
        private bool RulesListed = false;
        private int intCurrentInclude = 0;
        private int intIncludeCount = 0;
        private int intRuleCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {

        }



        private void GetPredicateLineNumbers()
        {
            string strConfigText = txtConfig.Text;
            string[] lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int intLineNumTrackerIndex = 0;

            foreach (var line in lstConfig)
            {
                if (line.ToLowerInvariant().Contains("<predicate>"))
                {
                    predicateStartLineNum = intLineNumTrackerIndex;
                    includesPresent = true;
                }

                if (line.ToLowerInvariant().Contains("</predicate>"))
                {
                    predicateEndLineNum = intLineNumTrackerIndex;
                    break;
                }

                if (line.ToLowerInvariant().Contains("include"))
                {
                    intlastInclude = intLineNumTrackerIndex;
                }

                intLineNumTrackerIndex += 1;
            }
        }

        protected void btnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtConfig.Text)) return;
            ruleList = string.Empty;

            GetPredicateLineNumbers();
            //int intLineNumTracker = 0;
            string strConfigText = txtConfig.Text;
            lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            CountInclude();

            for (intLineNumTracker = 0; intLineNumTracker <= predicateEndLineNum; intLineNumTracker++)
            //foreach (var line in lstConfig)
            {
                //intLineNumTracker += 1;
                var line = lstConfig[intLineNumTracker];
                CategorizeLine(line);

                if (intLineNumTracker >= predicateStartLineNum && intLineNumTracker <= predicateEndLineNum)
                {
                    if (intLineNumTracker == predicateStartLineNum) strConvertedConcatIncludeLines += "\r\t\t" + "\"includes\": [";

                    strConvertedConcatIncludeLines += GetInfoforInclude();
                }               

                if (intLineNumTracker == predicateEndLineNum) endArrayBracket += "\r\t\t" + "]";
            }

            var convertedLine = startingLine + "\r\t" + ModuleNameLine + "\r\t" + referencesLine + "\r\t" + "\"items\": {";

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

        private void CountInclude()
        {
            int includeCount = 0;

            for (intLineNumTracker = 0; intLineNumTracker <= predicateEndLineNum; intLineNumTracker++)
            {
                string currline = lstConfig[intLineNumTracker];

                if (currline.ToLowerInvariant().Contains("<include")) includeCount += 1;

            }

            intIncludeCount= includeCount;
        }

        private void CountRulesinInclude(int includeStartlineNumber, int includeEndlineNumber)
        {
            int ruleCount = 0;

            for (intLineNumTracker = includeStartlineNumber; intLineNumTracker <= includeEndlineNumber; intLineNumTracker++)
            {
                string currline = lstConfig[intLineNumTracker];

                if (currline.ToLowerInvariant().Contains("<except")) ruleCount += 1;

            }

            intRuleCount = ruleCount;
        }

        private string GetInfoforInclude()
        {
            string convertedLine = string.Empty;
            string currline = lstConfig[intLineNumTracker];

            if (currline.ToLowerInvariant().Contains("include") && currline.ToLowerInvariant().Contains("name=") &&
                currline.ToLowerInvariant().Contains("database=") && currline.ToLowerInvariant().Contains("path="))
            {
                includeModuleName = ExtractValueBetweenQuotes(currline, "name=");
                includeModulePath = ExtractValueBetweenQuotes(currline, "path=");
                includeModuleDB = ExtractValueBetweenQuotes(currline, "database=");

                convertedLine += "\r\t\t\t{";

                convertedLine += "\r\t\t\t\t \"name\" : " + includeModuleName + ",";
                convertedLine += "\r\t\t\t\t \"path\" : " + includeModulePath + ",";
                convertedLine += "\r\t\t\t\t \"database\" : " + includeModuleDB ;

                if (currline.Trim().ToLowerInvariant().Substring(currline.Trim().Length - 2, 2)!="/>")
                {
                    convertedLine += ",";
                    excludesPresent = true;
                    //CountRulesinInclude(intLineNumTracker,)

                    convertedLine+=BuildRules(convertedLine);
                }

                if (!string.IsNullOrWhiteSpace(ruleList)) convertedLine +=  ruleList;
                ruleList=string.Empty;

                convertedLine += "\r\t\t\t}";

                intCurrentInclude += 1;

                if (intCurrentInclude >= intIncludeCount)
                {
                    convertedLine += "";
                }
                else
                {
                   convertedLine += ",";
                }
            }

            return convertedLine;
        }



        private string BuildRules(string convertedlines)
        {
            RulesListed = false;
            do
            {
                if (!IsBlankLine(intLineNumTracker))
                {
                    var currline = lstConfig[intLineNumTracker];

                    if (currline.ToLowerInvariant().Contains("exclude") && currline.ToLowerInvariant().Contains("children=\"true\""))
                    {
                        convertedlines = "\r\t\t\t\t \"scope\" : \"SingleItem\"";
                        var nextline = lstConfig[intLineNumTracker + 1];
                        if (!nextline.ToLowerInvariant().Contains("except")) return convertedlines;

                    }

                    var prevline = lstConfig[intLineNumTracker - 1];
                    if (currline.ToLowerInvariant().Contains("except") && prevline.ToLowerInvariant().Contains("exclude"))
                    {
                        //these must be serialized too
                        if (string.IsNullOrWhiteSpace(ruleList))
                        {
                            ruleList += "\r\t\t\t\t \"rules\": [";
                        }

                        do
                        {
                            currline = lstConfig[intLineNumTracker];
                            var extractChildtoInclude = ExtractValueBetweenQuotes(currline, "name=", true);

                            ruleList += "\r\t\t\t\t\t\t {";
                            ruleList += "\r\t\t\t\t\t\t\t \"scope\" : \"ItemandDescendants\",";
                            ruleList += "\r\t\t\t\t\t\t\t \"path\" : " + extractChildtoInclude;
                            ruleList += "\r\t\t\t\t\t\t }";

                            if (lstConfig[intLineNumTracker + 1].Trim() != "</exclude>" && !RulesListed)
                            {
                                ruleList += ",";
                            }

                            intLineNumTracker += 1;

                        } while (lstConfig[intLineNumTracker].Trim() != "</exclude>");

                        if (lstConfig[intLineNumTracker + 1].Trim() != "</include>")
                        {
                            ruleList += "\r\t\t\t\t\t\t {";
                            ruleList += "\r\t\t\t\t\t\t\t \"scope\" : \"ignored\",";
                            ruleList += "\r\t\t\t\t\t\t\t \"path\" : \"*\"";
                            ruleList += "\r\t\t\t\t\t\t }";

                            RulesListed = true;
                        }
                    }

                    //if (currline.ToLowerInvariant().Contains("exclude") && currline.ToLowerInvariant().Contains("childrenofpath"))
                    //{
                    //    if (string.IsNullOrWhiteSpace(ruleList))
                    //    {
                    //        ruleList += "\r\t\t\t\t \"rules\": [";
                    //    }

                    //    //extract path to ignore
                    //    var extractChildrentoIgnore = ExtractValueBetweenQuotes(currline, "childrenOfPath=");
                    //    ruleList += "\r\t\t\t\t\t\t {";
                    //    ruleList += "\r\t\t\t\t\t\t\t \"scope\" : \"ignored\",";
                    //    ruleList += "\r\t\t\t\t\t\t\t \"path\" : " + extractChildrentoIgnore;
                    //    ruleList += "\r\t\t\t\t\t\t }";

                    //    if (lstConfig[intLineNumTracker + 1].Trim() != "</include>")
                    //    {
                    //        ruleList += ",";
                    //    }
                    //}

                    intLineNumTracker++;
                }

            } while (lstConfig[intLineNumTracker].Trim()!="</include>");

            if (!string.IsNullOrWhiteSpace(ruleList) && lstConfig[intLineNumTracker].Trim() == "</include>")
            {
                ruleList += "\r\t\t\t\t ]";
            }

            return string.Empty;
        }

        private bool IsBlankLine(int currLine)
        {
            if (string.IsNullOrWhiteSpace(lstConfig[currLine])) return true;

            return false;
        }

        private void CategorizeLine(string currline)
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

            if (currline.ToLowerInvariant().Contains("</include>")) intIncludeCount += 1;
        }

        private string ExtractValueBetweenQuotes(string currline, string subStringStart,bool path=false)
        {
            int intIncludeModuleName = currline.LastIndexOf(subStringStart, StringComparison.Ordinal);
            string stringforIncludeName = currline.Substring(intIncludeModuleName);

            var intFirst = IndexofnthOccurence(stringforIncludeName, '"', 1);
            var intSecond = IndexofnthOccurence(stringforIncludeName, '"', 2);

            var intStringLen = intSecond - intFirst;

            if (path) {

                string value = stringforIncludeName.Substring(intFirst, intStringLen + 1);

                return ReplaceFirst(value, "\"", "\"/");

            }

            return stringforIncludeName.Substring(intFirst, intStringLen + 1);
        }

        public static string ReplaceFirst(string str, string term, string replace)
        {
            int position = str.IndexOf(term);
            if (position < 0)
            {
                return str;
            }
            str = str.Substring(0, position) + replace + str.Substring(position + term.Length);
            return str;
        }
    }
}