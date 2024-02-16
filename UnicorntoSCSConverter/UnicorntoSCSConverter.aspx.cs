using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UnicorntoSCSConverter
{

    internal class Predicate
    {
        internal int StartLineIndex{ get; set; }
        internal int EndLineIndex{ get; set; }
        internal int PredicateNumber { get; set; }
    }

    internal class Configuration
    {
        internal int StartLineIndex { get; set; }
        internal int EndLineIndex { get; set; }
        internal int ConfigurationNumber { get; set; }
    }

    internal class Include
    {
        internal int StartLineIndex { get; set; }
        internal int EndLineIndex { get; set; }
        internal int PredicateNumber { get; set; }
        internal int IncludeNumber { get; set; }
    }

    internal class Exclude
    {
        internal int StartLineIndex { get; set; }
        internal int EndLineIndex { get; set; }
        internal int PredicateNumber { get; set; }
        internal int IncludeNumber { get; set; }
        internal int ExcludeNumber { get; set; }
    }

    internal class Except
    {
        internal int StartLineIndex { get; set; }
        internal int EndLineIndex { get; set; }
        internal int PredicateNumber { get; set; }
        internal int IncludeNumber { get; set; }
        internal int ExcludeNumber { get; set; }
        internal int ExceptNumber { get; set; }
    }

    internal class Configurations
    {
        internal int StartLineIndex { get; set; }
        internal int EndLineIndex { get; set; }
    }

    internal class TagInfo
    {
        internal int StartLineIndex { get; set; }
        internal int EndLineIndex { get; set; }
        internal string TagType { get; set; }
        internal int TagTypeCount { get; set; }
        internal int ParentTagPosition { get; set; }
    }

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
        private List<Predicate> PredicateList;
        private List<Include> IncludeList;
        private List<Exclude> ExcludeList;
        private List<Except> ExceptList;
        private List<Configuration> ConfigurationList;
        private Configuration configuration;
        private Predicate predicate;
        private Configurations configurations;
        private Include include;
        private Exclude exclude;
        private Except except;
        private TagInfo tagInfo;
        private int configsTagCount;
        private int predicatesTagCount;
        private int includesTagCount;
        private int exceptsTagCount;
        private List<TagInfo> tags;
        private int predicateNumber=0;
        private int includeNumber = 0;
        private int excludeNumber = 0;
        private int exceptNumber = 0;
        private int configurationNumber = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            PredicateList = new List<Predicate>();
            IncludeList = new List<Include>();
            ExceptList = new List<Except>();
            ExcludeList = new List<Exclude>();
            ConfigurationList = new List<Configuration>();
        }

        private void GetLineNumbers()
        {
            string strConfigText = txtConfig.Text;
            string[] lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int intLineNumTrackerIndex = 0;

            foreach (var line in lstConfig)
            {

                if (line.ToLowerInvariant().Contains("<configurations>"))
                {
                    configurations = new Configurations
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };

                }

                if (line.ToLowerInvariant().Contains("</configurations>"))
                {
                    configurations.EndLineIndex = intLineNumTrackerIndex;
                }

                if (line.ToLowerInvariant().Contains("<configuration ") && line.ToLowerInvariant().Contains("name"))
                {
                    configurationNumber += 1;
                    configuration = new Configuration
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };

                }

                if (line.ToLowerInvariant().Contains("</sitecore>")) break; //safety net for next line

                if (line.ToLowerInvariant().Contains("</configuration>"))
                {
                    configuration.EndLineIndex = intLineNumTrackerIndex;
                    configuration.ConfigurationNumber = configurationNumber;
                    ConfigurationList.Add(configuration);

                }

                if (line.ToLowerInvariant().Contains("<predicate"))
                {
                    predicateNumber += 1;
                    predicate = new Predicate
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };
                }

                if (line.ToLowerInvariant().Contains("</predicate>"))
                {
                    predicate.EndLineIndex = intLineNumTrackerIndex;
                    predicate.PredicateNumber = predicateNumber;
                    PredicateList.Add(predicate);
                }

                if (line.ToLowerInvariant().Contains("<include") && line.ToLowerInvariant().Contains("/>"))
                {
                    includeNumber += 1;
                    include = new Include
                    {
                        StartLineIndex = intLineNumTrackerIndex,
                        EndLineIndex = intLineNumTrackerIndex,
                        PredicateNumber = predicateNumber,
                        IncludeNumber=includeNumber
                        
                    };
                    IncludeList.Add(include);
                }
                else if (line.ToLowerInvariant().Contains("<include") && !line.ToLowerInvariant().Contains("/>"))
                {
                    includeNumber += 1;
                    include = new Include
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };
                }

                if (line.ToLowerInvariant().Contains("</include>"))
                {                    
                    include.EndLineIndex = intLineNumTrackerIndex;
                    include.PredicateNumber = predicateNumber;
                    include.IncludeNumber = includeNumber;
                    IncludeList.Add(include);
                    
                }

                if (line.ToLowerInvariant().Contains("<exclude") && line.ToLowerInvariant().Contains("/>"))
                {
                    excludeNumber += 1;
                    exclude = new Exclude
                    {
                        StartLineIndex = intLineNumTrackerIndex,
                        EndLineIndex = intLineNumTrackerIndex,
                        PredicateNumber = predicateNumber,
                        IncludeNumber = includeNumber,
                        ExcludeNumber=excludeNumber
                    };
                    ExcludeList.Add(exclude);
                }
                else if (line.ToLowerInvariant().Contains("<exclude") && !line.ToLowerInvariant().Contains("/>"))
                {
                    excludeNumber += 1;
                    exclude = new Exclude
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };
                }

                if (line.ToLowerInvariant().Contains("</exclude>"))
                {
                    exclude.EndLineIndex = intLineNumTrackerIndex;
                    exclude.PredicateNumber = predicateNumber;
                    exclude.IncludeNumber = includeNumber;
                    exclude.ExcludeNumber = excludeNumber;
                    ExcludeList.Add(exclude);

                }

                if (line.ToLowerInvariant().Contains("<except") && line.ToLowerInvariant().Contains("/>"))
                {
                    exceptNumber += 1;
                    except = new Except
                    {
                        StartLineIndex = intLineNumTrackerIndex,
                        EndLineIndex = intLineNumTrackerIndex,
                        PredicateNumber = predicateNumber,
                        IncludeNumber = includeNumber,
                        ExceptNumber = exceptNumber
                    };
                    ExceptList.Add(except);
                }

                intLineNumTrackerIndex += 1;
            }
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
                }

                if (line.ToLowerInvariant().Contains("</predicate>"))
                {
                    predicateEndLineNum = intLineNumTrackerIndex;
                    break;
                }

                if (line.ToLowerInvariant().Contains("<include"))
                {
                    includesPresent = true;
                }

                intLineNumTrackerIndex += 1;
            } 

        }

        private string GetConfigurationLine(string currline, int occurrence)
        {
            string moduleLine = string.Empty;
            string refLine = string.Empty;

            if (currline.ToLowerInvariant().Contains("configuration") && currline.ToLowerInvariant().Contains("name"))
            {
                var intFirst = IndexofnthOccurence(currline, '"', 1);
                var intSecond = IndexofnthOccurence(currline, '"', 2);

                var intStringLen = intSecond - intFirst;
                string modulename = currline.Substring(intFirst, intStringLen + 1);

                moduleLine = "\"namespace\": " + modulename + ",";
            }

            if (currline.ToLowerInvariant().Contains("dependencies"))
                refLine = "\"references\": [" + ExtractValueBetweenQuotes(currline.Replace(",", "\",\""), "dependencies=") + "],";

            if (occurrence>1) return "\r{" + "\r\t" + moduleLine + "\r\t" + refLine + "\r\t" + "\"items\": {";

            return "{" + "\r\t" + moduleLine + "\r\t" + refLine + "\r\t" + "\"items\": {";

            //if (currline.ToLowerInvariant().Contains("</include>")) intIncludeCount += 1;
        }

        protected void btnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtConfig.Text)) return;
            ruleList = string.Empty;

            GetLineNumbers();

            GetPredicateLineNumbers();
            string strConfigText = txtConfig.Text;
            lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            CountInclude();
            string convertedLine = string.Empty;

            for (intLineNumTracker = configurations.StartLineIndex; intLineNumTracker <= configurations.EndLineIndex; intLineNumTracker++)
            {
                foreach (var config in ConfigurationList)
                {
                    if (intLineNumTracker== config.StartLineIndex)
                    {
                        var line = lstConfig[intLineNumTracker];

                        convertedLine += GetConfigurationLine(line, config.ConfigurationNumber);
                    }                    

                }

                foreach (var predicate in PredicateList)
                {

                    if (intLineNumTracker >= predicate.StartLineIndex && intLineNumTracker <= predicate.EndLineIndex)
                    {
                        if (intLineNumTracker == predicate.StartLineIndex) convertedLine += "\r\t\t" + "\"includes\": [";

                        convertedLine += GetInfoforInclude();

                        if (intLineNumTracker == predicate.EndLineIndex) convertedLine += "\r\t\t" + "]";
                    }

                }

                //closing brackets
                foreach (var config in ConfigurationList)
                {
                    if (intLineNumTracker == config.EndLineIndex)
                    {
                        var line = lstConfig[intLineNumTracker];

                        if (intLineNumTracker == config.EndLineIndex) convertedLine += itemsEndLine + endLine;
                    }
                }
            }            

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

                           // if (lstConfig[intLineNumTracker + 1].Trim() != "</exclude>" && !RulesListed)
                           // {
                                ruleList += ",";
                            //}

                            intLineNumTracker += 1;

                        } while (lstConfig[intLineNumTracker].Trim() != "</exclude>");

                        if (lstConfig[intLineNumTracker].Trim() == "</exclude>")
                        {
                            ruleList += "\r\t\t\t\t\t\t {";
                            ruleList += "\r\t\t\t\t\t\t\t \"scope\" : \"ignored\",";
                            ruleList += "\r\t\t\t\t\t\t\t \"path\" : \"*\"";
                            ruleList += "\r\t\t\t\t\t\t }";

                            RulesListed = true;
                        }
                    }

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