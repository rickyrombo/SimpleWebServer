using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebServer
{
    class BoogelyBoogelyBooIAmAscriptProcessorThingThatWorksLaLaLaLaLaLaLaLaLaLaLALALALALALALA : IScriptProcessor
    {
        public ScriptResult ProcessScript(System.IO.Stream stream, IDictionary<string, string> requestParameters)
        {
            StreamReader reader = new StreamReader(stream);
            var all = reader.ReadToEnd();
            var script = _GenerateCScript(all);
            using (var newStream = new MemoryStream(Encoding.UTF8.GetBytes(script)))
            {
                var result = new CscriptProcessor().ProcessScript(newStream, requestParameters);
                return result;
            }
        }

        private void _HtmlAndExpressions(string html, StringBuilder script)
        {
            Regex r = new Regex("(@{(.*?)}\\s*)\\<", RegexOptions.Singleline);
            int lastPos = 0;
            foreach (Match m in r.Matches(html))
            {
                var line = "wout.WriteLine(\"" + html.Substring(lastPos, m.Index - lastPos) + "\");";
                script.AppendLine(line);
                lastPos = m.Index + m.Groups[1].Length;
                line = "wout.WriteLine(" + m.Groups[2].Value + ");";
                script.AppendLine(line);
            }
            if (lastPos < html.Length - 1)
            {
                var line = "wout.WriteLine(\"" + html.Substring(lastPos) + "\");";
                script.AppendLine(line);
            }
        }

        private string _GenerateCScript(string template)
        {
            var script = new StringBuilder();
            Regex r = new Regex("\\>\\s*({\\s*(.*?)\\s*})\\s*\\<", RegexOptions.Singleline);
            var matches = r.Matches(template);
            int lastPos = 0;
            foreach (Match m in matches)
            {
                var noncode = template.Substring(lastPos, m.Groups[1].Index - lastPos);
                _HtmlAndExpressions(noncode, script);
                lastPos = m.Groups[1].Index + m.Groups[1].Length;
                script.AppendLine(m.Groups[2].Value);
            }
            if (lastPos < template.Length)
            {
                _HtmlAndExpressions(template.Substring(lastPos), script);
            }
            return script.ToString();
        }
    }
}
