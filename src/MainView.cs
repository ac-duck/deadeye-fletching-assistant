///////////////////////////////////////////////////////////////////////////////
//File: MainView.cs
//
//Description: An example plugin using the VVS MetaViewWrappers. When VVS is
//  enabled, the plugin's view appears under the VVS bar. Otherwise, it appears
//  in the regular Decal bar.
//
//This file is Copyright (c) 2009 VirindiPlugins
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Decal.Adapter.Wrappers;
using System.Timers;
using System.IO;

namespace ExamplePlugin
{
    internal static class MainView
    {
        #region Auto-generated view code
        static MyClasses.MetaViewWrappers.IView View;
        static MyClasses.MetaViewWrappers.IButton bSelectCraftOutput, bSelectCraftInputA, bSelectCraftInputB, bToggleStartStop;
        static MyClasses.MetaViewWrappers.ITextBox txtCraftOutput, txtLow, txtCraftInputA, txtCraftInputB;
        static MyClasses.MetaViewWrappers.ISlider sldLow;

        static bool IsEnabled = false;

        public static void ViewInit()
        {
            //Create view here
            View = MyClasses.MetaViewWrappers.ViewSystemSelector.CreateViewResource(PluginCore.MyHost, "ExamplePlugin.ViewXML.testlayout.xml");
            bSelectCraftOutput = (MyClasses.MetaViewWrappers.IButton)View["bSelectCraftOutput"];
            bSelectCraftInputA = (MyClasses.MetaViewWrappers.IButton)View["bSelectCraftInputA"];
            bSelectCraftInputB = (MyClasses.MetaViewWrappers.IButton)View["bSelectCraftInputB"];
            bToggleStartStop = (MyClasses.MetaViewWrappers.IButton)View["bToggleStartStop"];

            txtCraftOutput = (MyClasses.MetaViewWrappers.ITextBox)View["txtCraftOutput"];
            txtLow = (MyClasses.MetaViewWrappers.ITextBox)View["txtLow"];
            txtCraftInputA = (MyClasses.MetaViewWrappers.ITextBox)View["txtCraftInputA"];
            txtCraftInputB = (MyClasses.MetaViewWrappers.ITextBox)View["txtCraftInputB"];

            sldLow = (MyClasses.MetaViewWrappers.ISlider)View["sldLow"];
            sldLow.Change += new EventHandler<MyClasses.MetaViewWrappers.MVIndexChangeEventArgs>(sldLow_Change);

            bSelectCraftOutput.Hit += new EventHandler(bSelectCraftOutput_Hit);
            bSelectCraftInputA.Hit += new EventHandler(bSelectCraftInputA_Hit);
            bSelectCraftInputB.Hit += new EventHandler(bSelectCraftInputB_Hit);

            bToggleStartStop.Hit += new EventHandler(bToggleStartStop_Hit);

            loadConfig();
            initTimer();

            if (IsEnabled) StartAutoFletcher();
        }

        public static void loadConfig()
        {
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("def.config");

                //Read the first line of text
                line = sr.ReadLine();

                //Continue to read until you reach end of file
                while (line != null)
                {
                    parseConfigLine(line);
                    line = sr.ReadLine();
                }

                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public static void parseConfigLine(string line)
        {
            string[] parts = line.Split(' ');
            switch (parts[0])
            {
                case "a":
                    txtCraftInputA.Text = line.Substring(1).Trim();
                    return;
                case "b":
                    txtCraftInputB.Text = line.Substring(1).Trim();
                    return;
                case "c":
                    txtCraftOutput.Text = line.Substring(1).Trim();
                    return;
                case "low":
                    txtLow.Text = parts[1];
                    sldLow.Position = int.Parse(parts[1]);
                    return;
            }
        }

        public static void saveConfig()
        {
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamWriter writer = new StreamWriter("def.config", false);
                writer.WriteLine("a " + txtCraftInputA.Text);
                writer.WriteLine("b " + txtCraftInputB.Text);
                writer.WriteLine("c " + txtCraftOutput.Text);
                writer.WriteLine("low " + txtLow.Text);
                writer.Close();

            }
            catch
            {

            }
        }

        public static void ViewDestroy()
        {
            saveConfig();

            bSelectCraftOutput = null;
            bSelectCraftInputA = null;
            bSelectCraftInputB = null;

            txtCraftOutput = null;
            txtLow = null;
            txtCraftInputA = null;
            txtCraftInputB = null;

            sldLow = null;
            View.Dispose();
        }
        #endregion Auto-generated view code


        static string CurrentSelectionName()
        {
            Decal.Adapter.Wrappers.WorldObject selection = PluginCore.MyCore.WorldFilter[PluginCore.MyHost.Actions.CurrentSelection];
            if (selection == null) { PluginCore.Chat("Nothing selected."); return ""; }
            PluginCore.Chat(selection.Name);
            PluginCore.Chat("Container: " + selection.Container);
            PluginCore.Chat("Behavior: " + selection.Behavior);
            PluginCore.Chat("Category: " + selection.Category);
            PluginCore.Chat("Icon: " + selection.Icon);            
            PluginCore.Chat("Equipped slot: " + (selection.Values(LongValueKey.EquippedSlots)));
            return selection.Name;
        }

        static void bSelectCraftInputA_Hit(object sender, EventArgs e)
        {
            txtCraftInputA.Text = CurrentSelectionName();
        }

        static void bSelectCraftInputB_Hit(object sender, EventArgs e)
        {
            txtCraftInputB.Text = CurrentSelectionName();
        }

        static void bSelectCraftOutput_Hit(object sender, EventArgs e)
        {
            txtCraftOutput.Text = CurrentSelectionName();
        }

        static void bToggleStartStop_Hit(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                bToggleStartStop.Text = "Start";
                StopAutoFletcher();
                IsEnabled = false;
            }
            else
            {
                bToggleStartStop.Text = "Stop";
                StartAutoFletcher();
                IsEnabled = true;
            }
        }

        static void ApplyItemByNames(string itemA, string itemB)
        {
            int guidA = 0, guidB = 0;
            WorldObjectCollection inventory = PluginCore.MyCore.WorldFilter.GetInventory();
            foreach (WorldObject worldObject in inventory)
            {
                if (worldObject.Name.Equals(itemA))
                {
                    guidA = worldObject.Id;
                }
                if (worldObject.Name.Equals(itemB))
                {
                    guidB = worldObject.Id;
                }
                if (guidA != 0 && guidB != 0)
                {
                    break;
                }
            }
            PluginCore.MyHost.Actions.ApplyItem(guidA, guidB);
        }

        static bool IsSupplyLow(string item_name, int min_count, bool onlyEquipped)
        {
            int supply = 0;
            WorldObjectCollection inventory = PluginCore.MyCore.WorldFilter.GetInventory();
            foreach (WorldObject worldObject in inventory)
            {
                if (worldObject.Name.Equals(item_name))
                {
                    if ((worldObject.Values(LongValueKey.EquippedSlots) > 0) || !onlyEquipped)
                    {
                        supply += worldObject.Values(LongValueKey.StackCount, 0);
                    }
                }
            }
            return supply < min_count;
        }

        static void MakeArrows()
        {
            string a = txtCraftInputA.Text;
            string b = txtCraftInputB.Text;
            PluginCore.Chat("Supply low. Applying <" + a + "> to <" + b + ">");
            ApplyItemByNames(a, b);
        }

        static void MakeIfLow()
        {
            string crafting_name = txtCraftOutput.Text;
            int crafting_min_count = sldLow.Position;
            if (IsSupplyLow(crafting_name, crafting_min_count, false))
            {
                MakeArrows();
            }
            if (IsAmmoLow())
            {
                PluginCore.Chat("Ammo low (<30), equipping more.");
                EquipAmmo();
            }
        }

        static bool IsAmmoLow()
        {
            string crafting_name = txtCraftOutput.Text;
            int crafting_min_count = sldLow.Position;
            return IsSupplyLow(crafting_name, 30, true);
        }

        static void EquipAmmo()
        {
            string crafting_name = txtCraftOutput.Text;
            WorldObjectCollection inventory = PluginCore.MyCore.WorldFilter.GetInventory();
            foreach (WorldObject worldObject in inventory)
            {
                if (worldObject.Name.Equals(crafting_name) && (worldObject.Values(LongValueKey.EquippedSlots) == 0 ))
                {
                    PluginCore.MyHost.Actions.UseItem(worldObject.Id, 0);
                }
            }
        }

        private static System.Timers.Timer aTimer;

        static void initTimer()
        {
            aTimer = new System.Timers.Timer();
            aTimer.Enabled = false;
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 10000;
        }

        static void StartAutoFletcher()
        {
            aTimer.Start();
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            MakeIfLow();
        }

        static void StopAutoFletcher()
        {
            aTimer.Stop();
            aTimer.Enabled = false;
        }

        static void sldLow_Change(object sender, EventArgs e)
        {
            txtLow.Text = sldLow.Position.ToString();
        }
    }
}
