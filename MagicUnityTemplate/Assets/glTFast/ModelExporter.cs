using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast.Export;
using System.IO;
using System.Threading.Tasks;

public class ModelExporter
{
    public async Task<bool> ConvertGameObjectToGLB(GameObject objectToExport)
    {
        var path = Path.Combine(Application.persistentDataPath, "1.glb");
        var rootLevelNodes = new GameObject[1] { objectToExport };
        var exportSettings = new ExportSettings
        {
            format = GltfFormat.Binary,
            fileConflictResolution = FileConflictResolution.Overwrite
        };

        var export = new GameObjectExport(exportSettings);
        export.AddScene(rootLevelNodes);

        var success = await export.SaveToFileAndDispose(path);
        return success;
    }
}
