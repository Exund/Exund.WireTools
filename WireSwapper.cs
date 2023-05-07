using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Exund.WireTools
{
    public class WireSwapper : MonoBehaviour
    {
        public class Swap
        {
            public string name;
            public BlockTypes t1;
            public BlockTypes t2;
        }

        public static List<Swap> swaps = new List<Swap>();

        static WireSwapper()
        {
            var wires = Commons.Wires;
            var l = wires.Length;
            var names = Commons.Names;

            for (var i = 0; i < l; i++)
            {
                for (var j = i + 1; j < l; j++)
                {
                    swaps.Add(new Swap
                    {
                        name = names[i] + " <-> " + names[j],
                        t1 = wires[i],
                        t2 = wires[j]
                    });
                }
            }
        }

        private static readonly int ID = GUIUtility.GetControlID(FocusType.Passive);

        private const float WIDTH = 150f;
        private const float HEIGHT = 250f;

        private void Update()
        {
            useGUILayout = Singleton.playerTank && Event.current.alt && Input.GetKey(Main.config.Swap);
        }

        private void OnGUI()
        {
            if (!useGUILayout)
            {
                return;
            }

            var rect = new Rect(Screen.width - WIDTH, (Screen.height - HEIGHT) * 0.5f, WIDTH, HEIGHT);
            GUI.Window(ID, rect, DoWindow, "Swap wires");
        }

        private void DoWindow(int id)
        {
            GUILayout.FlexibleSpace();
            foreach (var swap in swaps)
            {
                if (GUILayout.Button(swap.name))
                {
                    SwapWires(swap);
                }
                GUILayout.FlexibleSpace();
            }
        }

        public void SwapWires(Swap swap)
        {
            var tech = Singleton.playerTank;
            var spawnParams = Commons.GetTechReplaceParams(tech);
            var data = spawnParams.techData;

            var changed = false;

            for (int i = 0; i < data.m_BlockSpecs.Count; i++)
            {
                var blockSpec = data.m_BlockSpecs[i];
                var type = blockSpec.m_BlockType;

                if (type == swap.t1)
                {
                    blockSpec.m_BlockType = swap.t2;
                    data.m_BlockSpecs[i] = blockSpec;
                    changed = true;
                } 
                else if (type == swap.t2)
                {
                    blockSpec.m_BlockType = swap.t1;
                    data.m_BlockSpecs[i] = blockSpec;
                    changed = true;
                }
            }

            if (changed)
            {
                var replacement = ManSpawn.inst.SpawnTankRef(spawnParams, true);
                tech.visible.RemoveFromGame();
                ManTechs.inst.RequestSetPlayerTank(replacement.visible.tank, false);
            }
        }
    }
}
