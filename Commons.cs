using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exund.WireTools
{
    public static class Commons
    {
        public static readonly BlockTypes R = BlockTypes.EXP_Circuits_Wire_Red_111;
        public static readonly BlockTypes G = BlockTypes.EXP_Circuits_Wire_Green_111;
        public static readonly BlockTypes B = BlockTypes.EXP_Circuits_Wire_Blue_111;

        public static readonly BlockTypes[] Wires = new[] { R, G, B };

        public static ManSpawn.TankSpawnParams GetTechReplaceParams(Tank tech)
        {
            var data = new TechData();
            data.SaveTech(tech, true);

            var spawnParams = new ManSpawn.TankSpawnParams()
            {
                techData = data,
                teamID = tech.Team,
                position = tech.boundsCentreWorld,
                rotation = tech.trans.rotation,
                grounded = tech.grounded,
                placement = tech.IsAnchored ? ManSpawn.TankSpawnParams.Placement.BaseCentredAtPosition : ManSpawn.TankSpawnParams.Placement.BoundsCentredAtPosition,
            };

            return spawnParams;
        }
    }
}
