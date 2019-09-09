using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Util;

namespace ExamplePlugin
{
    class Deadeye
    {       
        private static bool start_with_lifetank;
        private static string start_cmd;

        public static void AddEventHandlers(bool start_with_lifetank, string start_cmd)
        {
            Deadeye.start_with_lifetank = start_with_lifetank;
            Deadeye.start_cmd = start_cmd;

            PluginCore.MyCore.CharacterFilter.LoginComplete += CharacterFilter_LoginComplete;
        }

        public static void RemoveEventHandlers()
        {
            PluginCore.MyCore.CharacterFilter.LoginComplete -= CharacterFilter_LoginComplete;
        }

        private static void CharacterFilter_LoginComplete(object sender, EventArgs e)
        {
            PrintMessageToWindow("Deadeye: Logged In!");

            if (start_with_lifetank)
            {
                PrintMessageToWindow("Starting LifeTank in 4 seconds!");
                System.Threading.Timer loginDelay = null;
                loginDelay = new System.Threading.Timer((obj) =>
                {
                    SendStartLifeTank();
                    loginDelay.Dispose();
                },
                            null, 4000, System.Threading.Timeout.Infinite);
            }

            if (!String.IsNullOrEmpty(start_cmd))
            {
                PrintMessageToWindow("Deadeye: Sending start command [" + start_cmd + "]  in 5 seconds!");
                System.Threading.Timer loginDelay = null;
                loginDelay = new System.Threading.Timer((obj) =>
                {
                    PostMsg.SendEnter(PluginCore.MyCore.Decal.Hwnd);
                    PostMsg.SendMsg(PluginCore.MyCore.Decal.Hwnd, start_cmd);
                    PostMsg.SendEnter(PluginCore.MyCore.Decal.Hwnd);
                    loginDelay.Dispose();
                },
                            null, 5000, System.Threading.Timeout.Infinite);
            }
        }       

        private static void SendStartLifeTank()
        {
            PluginCore.Chat("Starting LifeTank (Ctrl+F1 & Ctrl+F4)!");
            IntPtr handle = PluginCore.MyHost.Decal.Hwnd;
            IntPtr CTRL_KEY = new IntPtr(0x11);
            uint KEY_DOWN = 0x0100;
            uint KEY_UP = 0x0101;
            IntPtr F1_KEY = new IntPtr(112);
            IntPtr F4_KEY = new IntPtr(115);

            //SetForegroundWindow(p.MainWindowHandle);
            User32.PostMessage(handle, KEY_DOWN, CTRL_KEY, UIntPtr.Zero);
            User32.PostMessage(handle, KEY_DOWN, F1_KEY, UIntPtr.Zero);
            User32.PostMessage(handle, KEY_UP, F1_KEY, UIntPtr.Zero);
            User32.PostMessage(handle, KEY_DOWN, F4_KEY, UIntPtr.Zero);
            User32.PostMessage(handle, KEY_UP, F4_KEY, UIntPtr.Zero);
            User32.PostMessage(handle, KEY_UP, CTRL_KEY, UIntPtr.Zero);
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

        static void Craft(string a, string b)
        {
            PrintMessageToWindow("Applying [" + a + "] to [" + b + "]");
            ApplyItemByNames(a, b);
        }

        public static void MakeIfLow(string crafting_name, int min_crafting_count, string inputA, string inputB)
        {
            if (IsSupplyLow(crafting_name, min_crafting_count, false))
            {
                PrintMessageToWindow("Supply low. Less than " + min_crafting_count + " [" +crafting_name + "] remain.");
                Craft(inputA, inputB);
            }
        }

        public static string CurrentSelectionName()
        {
            try
            {
                WorldObject selection = PluginCore.MyCore.WorldFilter[PluginCore.MyHost.Actions.CurrentSelection];
                if (selection == null)
                {
                    PrintMessageToWindow("Nothing selected.");
                    return "";
                }
                PluginCore.Chat("Selected: " + selection.Name);
                return selection.Name;
            }
            catch { return "Error"; }
        }

        public static void PrintMessageToWindow(string message)
        {
            try { PluginCore.Chat(message); }
            catch { }
        }
    }
}
