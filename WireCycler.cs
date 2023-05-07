using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exund.WireTools
{
    public class WireCycler : MonoBehaviour
    {
        private static readonly Dictionary<BlockTypes, BlockTypes> ForwardMapping = new Dictionary<BlockTypes, BlockTypes>
        {
            { Commons.R, Commons.G },
            { Commons.G, Commons.B },
            { Commons.B, Commons.Y },
            { Commons.Y, Commons.R }
        };

        private static readonly Dictionary<BlockTypes, BlockTypes> BackwardMapping = new Dictionary<BlockTypes, BlockTypes>
        {
            { Commons.R, Commons.Y },
            { Commons.G, Commons.R },
            { Commons.B, Commons.G },
            { Commons.Y, Commons.B }
        };

        private void Update()
        {
            if (!Singleton.playerTank)
            {
                return;
            }

            var forwards = Input.GetKeyDown(Main.config.CycleForward);
            var backwards = Input.GetKeyDown(Main.config.CycleBackward);

            if (Event.current.alt && (forwards || backwards))
            {
                Cycle(backwards);
            }
        }

        public void Cycle(bool backwards)
        {
            var mapping = backwards ? BackwardMapping : ForwardMapping;

            var tech = Singleton.playerTank;

            var spawnParams = Commons.GetTechReplaceParams(tech);
            var data = spawnParams.techData;

            var changed = false;
            for (int i = 0; i < data.m_BlockSpecs.Count; i++)
            {
                var blockSpec = data.m_BlockSpecs[i];
                var type = blockSpec.m_BlockType;
                if (Commons.Wires.Contains(type))
                {
                    blockSpec.m_BlockType = mapping[type];
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
