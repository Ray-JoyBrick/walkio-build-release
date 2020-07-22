// namespace JoyBrick.Walkio.Build.CreatureDesignExport.EditorPart
// {
//     using System;
//     using System.Collections.Generic;
//     using System.IO;
//     using UnityEditor;
//     using UnityEngine;
//
//     //
//     using GameCommon = JoyBrick.Walkio.Game.Common;
//
//     public partial class AssetCreator
//     {
//         [MenuItem("Assets/Walkio/Create/Generated Level Asset - Creature")]
//         public static void CreateGeneratedCreature()
//         {
//             var crossProjectData = AssetDatabase.LoadAssetAtPath<GameCommon.CrossProjectData>(
//                 "Packages/walkio.game.common/Data Assets/Cross Project Data.asset");
//
//             //
//             var absoluteAssetFolderName = Application.dataPath;
//             var relativeAssetFolderName = "Assets";
//
//             var projectBaseFolderName = crossProjectData.commonProjectData.projectBaseFolderName;
//             var baseFolderName = crossProjectData.assetLevelDesignProjectData.baseFolderName;
//             var creatureModuleFolderName = crossProjectData.assetLevelDesignProjectData.creatureModuleFolderName;
//             var generationBaseFolderName = crossProjectData.assetLevelDesignProjectData.generationBaseFolderName;
//
//
//             var absoluteGenerationBasePath = Path.Combine(
//                 Application.dataPath, projectBaseFolderName, generationBaseFolderName);
//
//             GameCommon.Utility.FileHelper.CreateDirectoryIfNotExisted(absoluteGenerationBasePath);
//
//             var absoluteModuleCreaturePath = Path.Combine(
//                 absoluteGenerationBasePath, creatureModuleFolderName);
//
//             GameCommon.Utility.FileHelper.CreateDirectoryIfNotExisted(absoluteModuleCreaturePath);
//
//             var source01 = Path.Combine(
//                 relativeAssetFolderName, projectBaseFolderName, baseFolderName, creatureModuleFolderName,
//                 "Data Assets");
//             var target01 = Path.Combine(
//                 relativeAssetFolderName, projectBaseFolderName, generationBaseFolderName, creatureModuleFolderName,
//                 "Data Assets");
//
//             var source02 = Path.Combine(
//                 relativeAssetFolderName, projectBaseFolderName, baseFolderName, creatureModuleFolderName,
//                 "Prefabs");
//             var target02 = Path.Combine(
//                 relativeAssetFolderName, projectBaseFolderName, generationBaseFolderName, creatureModuleFolderName,
//                 "Prefabs");
//
//             var dataAssetsCopied = AssetDatabase.CopyAsset(source01, target01);
//             var prefabsCopied = AssetDatabase.CopyAsset(source02, target02);
//
//             if (dataAssetsCopied && prefabsCopied)
//             {
//                 AssetDatabase.SaveAssets();
//                 Debug.Log("Asset copied");
//             }
//         }
//     }
// }
