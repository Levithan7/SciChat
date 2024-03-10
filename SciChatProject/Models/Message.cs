using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SciChatProject.Models
{
    public class Message : SQLClass
    {
        public static string? TableName { get; set; } = "messagestable";

        public int id { get; set; }

        [SQLProperty(Name="content")] public string Content { get; set; }
        [SQLProperty(Name="userid")] public int UserID { get; set; }
        [SQLProperty(Name="conversationid")] public int ConversationID { get; set; }

        public static void SendMessage(string content, int userid, int conversationid)
        {
            var msg = new Message { Content=content, UserID = userid, ConversationID = conversationid };
            SendMessage(msg);
        }
        private static void SendMessage(Message message)
        {
            DataBaseHelper.ExecuteChange("messagestable", new List<Message> { message }, DataBaseHelper.ChangeType.Insert);
        }

        public User GetUser()
        {
            return DataBaseHelper.GetObjects<User>().First(x => x.id==UserID); 
        }
        public Conversation GetConversation()
        {
            return DataBaseHelper.GetObjects<Conversation>().First(x => x.id == ConversationID);
        }

        #region MessageParser
        public void ParseAllCommands()
        {
            Content = ParseAllCommands(Content);
        }

        public static string ParseAllCommands(string msg)
        {
            msg = ParseGraphBuilder(msg);
            msg = ParseLaTeX(msg);
            return msg;
        }

        public void ParseLaTeX()
        {
            Content = ParseLaTeX(Content);
        }

        public static string ParseLaTeX(string latex)
        {
            Match m;
            while((m = Regex.Match(latex, @"\$\$(?<eq>.+?)\$\$")).Success)
            {
                var latexExpression = m.Groups["eq"].Value;
                var imageHtml = $"<img src=\"https://latex.codecogs.com/svg.image? {latexExpression}\" />";
                latex = latex.Replace(m.Value, imageHtml);
            }
            return latex;
        }

        public void ParseGraphBuilder()
        {
            Content = ParseGraphBuilder(Content);
        }

        public static string ParseGraphBuilder(string msg)
        {
            msg = (new Regex("(\r|\n)")).Replace(msg,"");
            Match m;
            while ((m = Regex.Match(msg, @"\\begingraph(?<gb>.+?)\\endgraph")).Success)
            {
                var gb = m.Groups["gb"].Value;
                var graph = CreateGraph(gb);
                msg = msg.Replace(m.Value, graph);
            }
            return msg;
        }

        public static string CreateGraph(string gb)
        {
            #region defaultvalues
            string plottype = "scatter", xlabel=string.Empty, ylabel=string.Empty, datatype=string.Empty;
            double[] xdata = Array.Empty<double>(), ydata = Array.Empty<double>();
            int[] scale = { 100, 100};
            double? xmin = null, xmax = null, ymin = null, ymax = null;
            
            Match m;
            Plot myPlot = new();
            #endregion defaultvalues

            #region graphsettings
            #region general
            // Type:
            if ((m = Regex.Match(gb, @"\\type=\'(?<type>.+?)\'")).Success)
            {
                plottype = m.Groups["type"].Value;
            }

            // Scale:
            if ((m = Regex.Match(gb, @"\\scale=\'(?<scale>\d+x\d+)\'")).Success)
            {
                scale = m.Groups["scale"].Value.Split("x").Select(x=>int.Parse(x)).ToArray();
            }

            // Data Type:
            if ((m = Regex.Match(gb, @"\\datatype\=\'(?<datatype>.+?)\'")).Success)
            {
                datatype = m.Groups["datatype"].Value;
            }
            #endregion general

            #region labels
            // xlabel:
            if ((m = Regex.Match(gb, @"\\xlabel=\'(?<xlabel>.+?)\'")).Success)
            {
                xlabel = m.Groups["xlabel"].Value;
            }

            // ylabel:
            if ((m = Regex.Match(gb, @"\\ylabel=\'(?<ylabel>.+?)\'")).Success)
            {
                ylabel = m.Groups["ylabel"].Value;
            }
            #endregion labels

            #region minmax
            // xmin:
            if ((m = Regex.Match(gb, @"\\xmin=\'(?<xmin>.+?)\'")).Success)
            {
                xmin = double.Parse(m.Groups["xmin"].Value);
            }

            // xmax:
            if ((m = Regex.Match(gb, @"\\xmax=\'(?<xmax>.+?)\'")).Success)
            {
                xmax = double.Parse(m.Groups["xmax"].Value);
            }

            // ymin:
            if ((m = Regex.Match(gb, @"\\ymin=\'(?<ymin>.+?)\'")).Success)
            {
                ymin = double.Parse(m.Groups["ymin"].Value);
            }

            // ymax:
            if ((m = Regex.Match(gb, @"\\ymax=\'(?<ymax>.+?)\'")).Success)
            {
                ymax = double.Parse(m.Groups["ymax"].Value);
            }
            #endregion minmax
            #endregion graphsettings

            #region data
            #region function
            if ((m = Regex.Match(gb, @"\\function\=\'(?<function>.+?)\'")).Success)
            {
                var functionStr = m.Groups["function"].Value;
                var function = StringToLambda(functionStr);
                myPlot.Add.Function(function);
            }
            #endregion function

            #region rawdata
            // xdata:
            if ((m = Regex.Match(gb, @"\\xdata\=\[(?<xdata>.+?)\]")).Success)
            {
                xdata = m.Groups["xdata"].Value.Split(",").Select(x=>double.Parse(x)).ToArray();
            }

            // ydata:
            if ((m = Regex.Match(gb, @"\\ydata\=\[(?<ydata>.+?)\]")).Success)
            {
                ydata = m.Groups["ydata"].Value.Split(",").Select(y=>double.Parse(y)).ToArray();
            }

            // xydata:
            if ((m = Regex.Match(gb, @"\\xydata\=\[(?<xydata>.+?)\]")).Success)
            {
                var xydata = m.Groups["xydata"].Value;
                while ((m = Regex.Match(xydata, @"\{(?<x>.+?)\,(?<y>.+?)\}\,?")).Success)
                {
                    var x = double.Parse(m.Groups["x"].Value); 
                    xdata = xdata.Append(x).ToArray();
                    var y = double.Parse(m.Groups["y"].Value); 
                    ydata = ydata.Append(y).ToArray();
                    
                    xydata = xydata.Replace(m.Value, "");   
                }
            }
            #endregion rawdata
            #endregion data

            #region plotcreation
            myPlot.XLabel(xlabel);
            myPlot.YLabel(ylabel);
            myPlot.Axes.SetLimits(xmin??xdata.Min()+1, xmax??xdata.Max()+1, ymin??ydata.Min()+1, ymax??ydata.Max()+1); // left, right, bottom, top

            switch (plottype)
            {
                case "scatter":
                    myPlot.Add.Scatter(xdata, ydata);
                    break;
                case "bar":
                    if (xdata==Array.Empty<double>())
                    {
                        double ctr = 0;
                        xdata = ydata.Select(y => ++ctr).ToArray();
                    }
                    myPlot.Add.Bars(xdata, ydata);
                    break;
                case "pie":
                    myPlot.Add.Pie(ydata);
                    break;
                default:
                    throw new($"Plottype {plottype} has not been implemented yet!");
            }

            var bytes = myPlot.GetImageBytes(scale[0], scale[1]);
            return $"<img src=\"data:image/png;base64,{Convert.ToBase64String(bytes)}\"/>";
            #endregion plotcreation
        }
        #endregion MessageParser

        private static Func<double, double> StringToLambda(string expression)
        {
            return DynamicExpressionParser.ParseLambda<double, double>(new ParsingConfig(), true, $"x=>{MathToLambda(expression)}").Compile();
        }

        private static string MathToLambda(string math)
        {
            // Generiert mit ChatGPT nach einer selbst erstellten Vorlage:
            var converter = new KeyValuePair<string, string>[]
            {
                new("e", "Math.E"),
                new("pi", "Math.PI"),
                new("abs", "Math.Abs"),
                new("acos", "Math.Acos"),
                new("asin", "Math.Asin"),
                new("atan", "Math.Atan"),
                new("atan2", "Math.Atan2"),
                new("ceiling", "Math.Ceiling"),
                new("cos", "Math.Cos"),
                new("cosh", "Math.Cosh"),
                new("exp", "Math.Exp"),
                new("floor", "Math.Floor"),
                new("log", "Math.Log"),
                new("log10", "Math.Log10"),
                new("max", "Math.Max"),
                new("min", "Math.Min"),
                new("pow", "Math.Pow"),
                new("round", "Math.Round"),
                new("sign", "Math.Sign"),
                new("sin", "Math.Sin"),
                new("sinh", "Math.Sinh"),
                new("sqrt", "Math.Sqrt"),
                new("tan", "Math.Tan"),
                new("tanh", "Math.Tanh"),
                new("truncate", "Math.Truncate")
            };

            foreach(var conversion in converter)
            {
                math = math.Replace(conversion.Key, conversion.Value);
            }
            var final = ConvertCurrentLevelParantatheses(math);
            return final;
        }

        public static int FindControParan(int idx, string test, int direction, bool returnCharacterIfNoParan=false) // direction 1-> forward ) searched -1-> backward ( searched
        {
            char lookedFor='!';
            char lookedForContra;

            lookedFor = direction == 1 ? ')' : (direction == -1 ? '(' : '!');
            lookedForContra = lookedFor == ')' ? '(' : ')';

            var charAtPosition = test[idx];
            if (charAtPosition != lookedForContra)
            {
                if (returnCharacterIfNoParan)
                    return idx;
                throw new Exception($"No {lookedForContra} at given index {idx}!");
            }
            if (lookedFor == '!')
                throw new Exception($"Direction {direction} is invalid (has to be -1 or 1)!");

            int lookedForExcess = 1;
            while(lookedForExcess != 0)
            {
                idx+=direction;
                charAtPosition = test[idx];
                lookedForExcess += charAtPosition == lookedFor ? -1 : (charAtPosition == lookedForContra ? 1 : 0);
            }
            return idx;
        }

        public static string ConvertExponentToPow(string paranthase)
        {
            Match m;
            if ((m = Regex.Match(paranthase, @"(?<base>.+)\^(?<exp>.+)")).Success)
            {
                var powbase = m.Groups["base"].Value;
                powbase = powbase.Substring(FindControParan(powbase.Length - 1, powbase, -1, true));

                var powexp = m.Groups["exp"].Value;
                powexp = powexp.Substring(0, FindControParan(0, powexp, 1, true) +1);
                
                paranthase = paranthase.Replace($"{powbase}^{powexp}", $"Math.Pow({powbase}, {powexp})");
            }
            return paranthase;
        }

        public static string ConvertCurrentLevelParantatheses(string level)
        {
            var children = new List<string>();
            int leftOpen = 0;
            var currentString = string.Empty;

            foreach(var i in level)
            {
                if (currentString == string.Empty && i != '(')
                    continue;

                currentString += i;
                leftOpen += Convert.ToInt16(i == '(');
                leftOpen -= Convert.ToInt16(i == ')');
                if(leftOpen==0 && currentString!=string.Empty && currentString.StartsWith("("))
                {
                    children.Add(currentString);
                    currentString = string.Empty;
                }
            }

            var result = level;
            
            if(children.Count() != 0)
            {
                foreach(var j in children)
                {
                    var replacement = j;
                    if(replacement.StartsWith("("))
                        replacement = replacement.Substring(1);
                    if (replacement.EndsWith(")"))
                        replacement = replacement.Substring(0, replacement.Length - 1);

                    replacement = ConvertCurrentLevelParantatheses(replacement);
                    result = result.Replace(j, $"({replacement})");
                }
            }

            var final = ConvertExponentToPow(result);
            return final;
        }
    }

}
