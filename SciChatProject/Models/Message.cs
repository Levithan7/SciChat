using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
            string plottype = "scatter", xlabel=string.Empty, ylabel=string.Empty;
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
            // xdata:
            if ((m = Regex.Match(gb, @"\\xdata\=\[(?<xdata>.+?)\]")).Success)
            {
                var test = m.Groups["xdata"].Value.Split(",");
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
    }
}
