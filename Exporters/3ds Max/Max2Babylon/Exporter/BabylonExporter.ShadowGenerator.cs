﻿using System.Collections.Generic;
using Autodesk.Max;
using BabylonExport.Entities;

namespace Max2Babylon
{
    partial class BabylonExporter
    {
        private BabylonShadowGenerator ExportShadowGenerator(IINode lightNode, BabylonScene babylonScene)
        {
            var maxLight = (lightNode.ObjectRef as ILightObject);
            var babylonShadowGenerator = new BabylonShadowGenerator();

            RaiseMessage("Exporting shadow map", 2);

            babylonShadowGenerator.lightId = lightNode.GetGuid().ToString();

            babylonShadowGenerator.mapSize = maxLight.GetMapSize(0, Tools.Forever);
            babylonShadowGenerator.usePoissonSampling = maxLight.AbsMapBias == 1;

            var list = new List<string>();

            var inclusion = maxLight.ExclList.TestFlag(1); //NT_INCLUDE 
            var checkExclusionList = maxLight.ExclList.TestFlag(4); //NT_AFFECT_SHADOWCAST 

            foreach (var meshNode in Loader.Core.RootNode.NodesListBySuperClass(SClass_ID.Geomobject))
            {
                if (meshNode.CastShadows == 1)
                {
                    var inList = maxLight.ExclList.FindNode(meshNode) != -1;

                    if (!checkExclusionList || (inList && inclusion) || (!inList && !inclusion))
                    {
                        list.Add(meshNode.GetGuid().ToString());
                    }
                }
            }
            babylonShadowGenerator.renderList = list.ToArray();

            babylonScene.ShadowGeneratorsList.Add(babylonShadowGenerator);
            return babylonShadowGenerator;
        }
    }
}
