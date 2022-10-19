using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.FileDialog;
using SCKRM.Json;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SDJK.Map
{
    public static class MapLoader
    {
        public static async UniTask<MapPack> MapPackLoad(string packfolderPath, AsyncTask asyncTask)
        {
            string[] packPaths = DirectoryTool.GetFiles(packfolderPath, new ExtensionFilter(MapCompatibilitySystem.compatibleMapExtensions).ToSearchPatterns());
            if (packPaths == null || packPaths.Length <= 0)
                return null;

            MapPack pack = new MapPack();
            for (int i = 0; i < packPaths.Length; i++)
            {
                Map map = MapLoad<Map>(packPaths[i].Replace("\\", "/"));
                if (map != null)
                    pack.maps.Add(map);

                if (await UniTask.NextFrame(asyncTask.cancel).SuppressCancellationThrow())
                    return null;
            }

            return pack;
        }

        public static T MapLoad<T>(string mapFilePath) where T : Map, new()
        {
            T sdjkMap = MapCompatibilitySystem.GlobalMapCompatibility<T>(mapFilePath);
            if (sdjkMap == null)
                return null;

            sdjkMap.mapFilePathParent = Directory.GetParent(mapFilePath).ToString();
            sdjkMap.mapFilePath = mapFilePath;

            return sdjkMap;
        }
    }
}
