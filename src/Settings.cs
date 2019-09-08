using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExamplePlugin
{
    class Settings
    {
        public string inputA = "Wrapped Bundle of Greater Fire Arrowheads";
        public string inputB = "Wrapped Bundle of Arrowshafts";
        public string craftOutput = "Greater Fire Arrows";
        public int low = 100;
        public bool startLtOnLogin = false;
        public bool startThisOnLogin = true;
        public string startCmdOnLogin = "/vt start";

        public string filepath;

        public Settings(string filepath)
        {
            this.filepath = filepath;
            load();
        }

        public void save()
        {
            try
            {
                StreamWriter writer = new StreamWriter(filepath, false);

                writer.WriteLine("a " + inputA);
                writer.WriteLine("b " + inputB);
                writer.WriteLine("c " + craftOutput);
                writer.WriteLine("low " + low);
                writer.WriteLine("lt " + startLtOnLogin);
                writer.WriteLine("start " + startThisOnLogin);
                writer.WriteLine("cmd " + startCmdOnLogin);
                writer.Close();

                Deadeye.PrintMessageToWindow("Saved Deadeye config.");
            }
            catch
            {
                Deadeye.PrintMessageToWindow("Error saving Deadeye config.");
            }
        }

        public void load()
        {         
            try
            {
                StreamReader sr = new StreamReader(filepath);
                String line = sr.ReadLine();
                while (line != null)
                {
                    parseConfigLine(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Deadeye.PrintMessageToWindow("Exception: " + e.Message);
            }
        }

        private void parseConfigLine(string line)
        {
            string[] parts = line.Split(' ');
            switch (parts[0])
            {
                case "a":
                    inputA = line.Substring(1).Trim();
                    return;
                case "b":
                    inputB = line.Substring(1).Trim();
                    return;
                case "c":
                    craftOutput = line.Substring(1).Trim();
                    return;
                case "low":
                    low = int.Parse(line.Substring(3).Trim());
                    return;
                case "lt":
                    startLtOnLogin = bool.Parse(line.Substring(2).Trim());
                    break;
                case "start":
                    startThisOnLogin = bool.Parse(line.Substring(5).Trim());
                    break;
                case "cmd":
                    startCmdOnLogin = line.Substring(3).Trim();
                    break;
            }
        }

    }
}
