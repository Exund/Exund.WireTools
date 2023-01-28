using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using ModHelper;
using Nuterra.NativeOptions;

namespace Exund.WireTools
{
    public class Main : ModBase
    {
        internal static Logger logger;
        private const string ID = "Exund.WireTools";
        private static readonly Harmony harmony = new Harmony(ID);
        private static bool Inited = false;

        private static GameObject holder;

        public static ModConfig configFile;
        public static Config config = new Config();

        public override void Init()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if (!Inited)
            {
                holder = new GameObject("",
                    typeof(WireCycler),
                    typeof(WireSwapper)
                );
                GameObject.DontDestroyOnLoad(holder);

                Inited = true;
            }

            holder.SetActive(true);
        }

        public override void DeInit()
        {
            harmony.UnpatchAll(ID);

            if (holder)
            {
                holder.SetActive(false);
            }
        }

        public override bool HasEarlyInit()
        {
            return true;
        }

        public override void EarlyInit()
        {
            logger = new Logger(ID, defaultLogLevel: 0);

            const string Forward = nameof(Config._cycleForward);
            const string Backward = nameof(Config._cycleBackward);
            const string Swap = nameof(Config._swap);

            configFile = new ModConfig();

            configFile.BindConfig(config, Forward);
            configFile.BindConfig(config, Backward);
            configFile.BindConfig(config, Swap);

            const string ModName = "Wire Tools";

            var ForwardKey = new OptionKey("Cycle wires forward (Alt+?)", ModName, config.CycleForward);
            var BackwardKey = new OptionKey("Cycle wires backward (Alt+?)", ModName, config.CycleBackward);
            var SwapKey = new OptionKey("Swap wires menu (Alt+?)", ModName, config.Swap);


            ForwardKey.onValueSaved.AddListener(() => { configFile[Forward] = (int)ForwardKey.SavedValue; });

            BackwardKey.onValueSaved.AddListener(() => { configFile[Backward] = (int)BackwardKey.SavedValue; });

            SwapKey.onValueSaved.AddListener(() => { configFile[Swap] = (int)SwapKey.SavedValue; });

            NativeOptionsMod.onOptionsSaved.AddListener(() => { configFile.WriteConfigJsonFile(); });
        }

        public class Config
        {
            public int _cycleForward = (int)KeyCode.Comma;
            public int _cycleBackward = (int)KeyCode.Period;

            public int _swap = (int)KeyCode.S;

            public KeyCode CycleForward
            {
                get => (KeyCode)_cycleForward;
            }

            public KeyCode CycleBackward
            {
                get => (KeyCode)_cycleBackward;
            }

            public KeyCode Swap
            {
                get => (KeyCode)_swap;
            }
        }
    }
}
