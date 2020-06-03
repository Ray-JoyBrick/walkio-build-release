namespace JoyBrick.Walkio.Game.Common.EditorPart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public static class CountHelper
    {
        [MenuItem("Assets/Walkio/Game/Common/Count Done Loading Asset Wait Attribute")]
        public static void CountDoneLoadingAssetWaitAttribute()
        {
            var typesWithAddToCountAttribute =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(GameCommon.DoneLoadingAssetWaitAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<GameCommon.DoneLoadingAssetWaitAttribute>() };

            var addToCountAttributes = typesWithAddToCountAttribute.ToList();
            var appCount =
                addToCountAttributes
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0)
                    .ToList();
            var preparationCount =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0)
                    .ToList();
            var stageCount =
                addToCountAttributes
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0)
                    .ToList();

            var gameSettings = AssetDatabase.LoadAssetAtPath<GameCommon.GameSettings>(
                "Packages/com.walkio.game.common/Data Assets/Game Settings.asset");

            if (gameSettings != null)
            {
                gameSettings.doneLoadingAssetWaitForApp = appCount.Count;
                gameSettings.doneLoadingAssetWaitForPreparation = preparationCount.Count;
                gameSettings.doneLoadingAssetWaitForStage = stageCount.Count;
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {appCount.Count} of <AddToCountAttribute> classes for app in the project");
            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {preparationCount.Count} of <AddToCountAttribute> classes for preparation in the project");
            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {stageCount.Count} of <AddToCountAttribute> classes for stage in the project");
        }
    }
}
