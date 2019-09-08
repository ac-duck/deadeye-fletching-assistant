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
using MyClasses.MetaViewWrappers;

namespace ExamplePlugin
{
    internal static class MainView
    {
        #region Auto-generated view code
        static MyClasses.MetaViewWrappers.IView View;
        static MyClasses.MetaViewWrappers.IButton bSelectCraftOutput, bSelectCraftInputA, bSelectCraftInputB, bToggleStartStop;
        static MyClasses.MetaViewWrappers.ITextBox txtCraftOutput, txtCraftInputA, txtCraftInputB, txtCommandOnLogin;
        static MyClasses.MetaViewWrappers.IStaticText txtLow;
        static MyClasses.MetaViewWrappers.ISlider sldLow;
        static MyClasses.MetaViewWrappers.ICheckBox chkStartOnLogin, chkLifetankOnLogin;

        static bool IsEnabled = false;

        static Settings DeadeyeSettings = new Settings("def.config");

        public static void ViewInit()
        {
            try
            {
                Deadeye.PrintMessageToWindow("Deadeye Starting!");
                View = MyClasses.MetaViewWrappers.ViewSystemSelector.CreateViewResource(PluginCore.MyHost, "ExamplePlugin.ViewXML.testlayout.xml");
                bSelectCraftOutput = (MyClasses.MetaViewWrappers.IButton)View["bSelectCraftOutput"];
                bSelectCraftInputA = (MyClasses.MetaViewWrappers.IButton)View["bSelectCraftInputA"];
                bSelectCraftInputB = (MyClasses.MetaViewWrappers.IButton)View["bSelectCraftInputB"];
                bToggleStartStop = (MyClasses.MetaViewWrappers.IButton)View["bToggleStartStop"];

                txtCraftOutput = (MyClasses.MetaViewWrappers.ITextBox)View["txtCraftOutput"];
                txtLow = (MyClasses.MetaViewWrappers.IStaticText)View["txtLow"];
                txtCraftInputA = (MyClasses.MetaViewWrappers.ITextBox)View["txtCraftInputA"];
                txtCraftInputB = (MyClasses.MetaViewWrappers.ITextBox)View["txtCraftInputB"];

                txtCommandOnLogin = (MyClasses.MetaViewWrappers.ITextBox)View["txtCommandOnLogin"];
                chkStartOnLogin = (MyClasses.MetaViewWrappers.ICheckBox)View["chkStartOnLogin"];
                chkLifetankOnLogin = (MyClasses.MetaViewWrappers.ICheckBox)View["chkLifetankOnLogin"];

                sldLow = (MyClasses.MetaViewWrappers.ISlider)View["sldLow"];
                sldLow.Change += new EventHandler<MyClasses.MetaViewWrappers.MVIndexChangeEventArgs>(sldLow_Change);

                bSelectCraftOutput.Hit += new EventHandler(bSelectCraftOutput_Hit);
                bSelectCraftInputA.Hit += new EventHandler(bSelectCraftInputA_Hit);
                bSelectCraftInputB.Hit += new EventHandler(bSelectCraftInputB_Hit);

                chkLifetankOnLogin.Change += ChkLifetankOnLogin_Change;
                chkStartOnLogin.Change += ChkStartOnLogin_Change;
                txtCommandOnLogin.Change += TxtCommandOnLogin_Change;

                bToggleStartStop.Hit += new EventHandler(bToggleStartStop_Hit);

                loadConfig();

                IsEnabled = DeadeyeSettings.startThisOnLogin;

                Deadeye.AddEventHandlers(DeadeyeSettings.startLtOnLogin, DeadeyeSettings.startCmdOnLogin);

                initTimer();

                if (IsEnabled) StartAutoFletcher();
            }
            catch
            {
                Deadeye.PrintMessageToWindow("Unknown issue starting");
            }
        }


        private static void TxtCommandOnLogin_Change(object sender, MyClasses.MetaViewWrappers.MVTextBoxChangeEventArgs e)
        {
            saveSettings();
        }

        private static void ChkStartOnLogin_Change(object sender, MyClasses.MetaViewWrappers.MVCheckBoxChangeEventArgs e)
        {
            saveSettings();
        }

        private static void ChkLifetankOnLogin_Change(object sender, MyClasses.MetaViewWrappers.MVCheckBoxChangeEventArgs e)
        {
            saveSettings();
        }

        public static void loadConfig()
        {
            DeadeyeSettings.load();

            txtCraftInputA.Text = DeadeyeSettings.inputA;
            txtCraftInputB.Text = DeadeyeSettings.inputB;
            txtCraftOutput.Text = DeadeyeSettings.craftOutput;
            txtLow.Text = DeadeyeSettings.low.ToString();
            sldLow.Position = DeadeyeSettings.low;
            chkLifetankOnLogin.Checked = DeadeyeSettings.startLtOnLogin;
            chkStartOnLogin.Checked = DeadeyeSettings.startThisOnLogin;
            txtCommandOnLogin.Text = DeadeyeSettings.startCmdOnLogin;
        }

        private static string GetTextFromTextBox(ITextBox txtBox)
        {
            return txtBox == null ? "" : txtBox.Text.Trim();
        }

        public static void saveSettings()
        {
            if (txtCraftInputA != null)
            {
                DeadeyeSettings.inputA = GetTextFromTextBox(txtCraftInputA);
            }
            if (txtCraftInputB != null) {
                DeadeyeSettings.inputB = GetTextFromTextBox(txtCraftInputB);
            }
            if (txtCraftOutput != null) {
                DeadeyeSettings.craftOutput = GetTextFromTextBox(txtCraftOutput);
            }
            if (sldLow != null)
            {
                DeadeyeSettings.low = sldLow.Position;
            }
            if (chkLifetankOnLogin != null)
            {
                DeadeyeSettings.startLtOnLogin = chkLifetankOnLogin.Checked;
            }
            if(txtCommandOnLogin != null)
            {
                DeadeyeSettings.startCmdOnLogin = GetTextFromTextBox(txtCommandOnLogin);
            }
            if (chkStartOnLogin != null)
            {
                DeadeyeSettings.startThisOnLogin = chkStartOnLogin.Checked;
            }

            DeadeyeSettings.save();
        }

        public static void ViewDestroy()
        {
            //saveSettings();

            Deadeye.RemoveEventHandlers();

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

        
        static void bSelectCraftInputA_Hit(object sender, EventArgs e)
        {     
            try {
                txtCraftInputA.Text = Deadeye.CurrentSelectionName();
                saveSettings();
            }
            catch { }
        }

        static void bSelectCraftInputB_Hit(object sender, EventArgs e)
        {
            try {
                txtCraftInputB.Text = Deadeye.CurrentSelectionName();
                saveSettings();
            }
            catch { }
        }

        static void bSelectCraftOutput_Hit(object sender, EventArgs e)
        {
            try {
                txtCraftOutput.Text = Deadeye.CurrentSelectionName();
                saveSettings();
            }
            catch { }
        }
        static void sldLow_Change(object sender, EventArgs e)
        {
            try
            {
                txtLow.Text = sldLow.Position.ToString();
                //saveSettings();
            }
            catch { }
        }

        static void bToggleStartStop_Hit(object sender, EventArgs e)
        {
            try
            {
                if (IsEnabled)
                {
                    StopAutoFletcher();
                }
                else
                {
                    StartAutoFletcher();
                }
            }
            catch { }
        }


        private static System.Timers.Timer aTimer;

        private const int ONE_SECOND = 1000;

        static void initTimer()
        {
            aTimer = new System.Timers.Timer();
            aTimer.Enabled = false;
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 15*ONE_SECOND;
        }

        static void StartAutoFletcher()
        {
            Deadeye.PrintMessageToWindow("Auto-fletching enabled!");
            bToggleStartStop.Text = "Stop";
            IsEnabled = true;

            aTimer.Start();
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                Deadeye.MakeIfLow(txtCraftOutput.Text, sldLow.Position, txtCraftInputA.Text, txtCraftInputB.Text);
            }
            catch
            {
                Deadeye.PrintMessageToWindow("Unknown Error!");
            }
        }

        static void StopAutoFletcher()
        {
            aTimer.Stop();
            aTimer.Enabled = false;
            bToggleStartStop.Text = "Start";
            IsEnabled = false;
        }
        
    }
}
