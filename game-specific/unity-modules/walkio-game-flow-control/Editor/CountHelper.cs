namespace JoyBrick.Walkio.Game.FlowControl.EditorPart
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
        [MenuItem("Assets/Walkio/Game/Flow Control/Count Done Loading Asset Wait Attribute")]
        public static void CountDoneLoadingAssetWaitAttribute()
        {
            var typesWithAddToCountAttribute =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(DoneLoadingAssetWaitAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<DoneLoadingAssetWaitAttribute>() };

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

            var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
                "Packages/com.walkio.game.flow-control.common/Data Assets/Flow Control Data.asset");

            if (flowControlData != null)
            {
                flowControlData.doneLoadingAssetWaitForApp = appCount.Count;
                flowControlData.doneLoadingAssetWaitForPreparation = preparationCount.Count;
                flowControlData.doneLoadingAssetWaitForStage = stageCount.Count;
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {appCount.Count} of <AddToCountAttribute> classes for app in the project");
            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {preparationCount.Count} of <AddToCountAttribute> classes for preparation in the project");
            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {stageCount.Count} of <AddToCountAttribute> classes for stage in the project");
        }
        
        [MenuItem("Assets/Walkio/Game/Flow Control/Count Done Setting Asset Wait Attribute")]
        public static void CountDoneSettingAssetWaitAttribute()
        {
            var typesWithAddToCountAttribute =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(DoneSettingAssetWaitAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<DoneSettingAssetWaitAttribute>() };

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

            var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
                "Packages/com.walkio.game.flow-control.common/Data Assets/Flow Control Data.asset");

            if (flowControlData != null)
            {
                flowControlData.doneSettingAssetWaitForApp = appCount.Count;
                flowControlData.doneSettingAssetWaitForPreparation = preparationCount.Count;
                flowControlData.doneSettingAssetWaitForStage = stageCount.Count;
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {appCount.Count} of <AddToCountAttribute> classes for app in the project");
            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {preparationCount.Count} of <AddToCountAttribute> classes for preparation in the project");
            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {stageCount.Count} of <AddToCountAttribute> classes for stage in the project");
        }

    }
}
