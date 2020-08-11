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
            var appAttributes =
                addToCountAttributes
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0)
                    .ToList();
            var preparationAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0)
                    .ToList();
            var stageAttributes =
                addToCountAttributes
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0)
                    .ToList();

            var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
                "Packages/com.walkio.game.flow-control.common/Data Assets/Flow Control Data.asset");

            if (flowControlData != null)
            {
                flowControlData.doneLoadingAssetWaitForApp = appAttributes.Count;
                flowControlData.doneLoadingAssetWaitForPreparation = preparationAttributes.Count;
                flowControlData.doneLoadingAssetWaitForStage = stageAttributes.Count;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {appAttributes.Count} of <AddToCountAttribute> classes for app in the project");
            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {preparationAttributes.Count} of <AddToCountAttribute> classes for preparation in the project");
            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {stageAttributes.Count} of <AddToCountAttribute> classes for stage in the project");
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
            var appAttributes =
                addToCountAttributes
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0)
                    .ToList();
            var preparationAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0)
                    .ToList();
            var stageAttributes =
                addToCountAttributes
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0)
                    .ToList();

            var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
                "Packages/com.walkio.game.flow-control.common/Data Assets/Flow Control Data.asset");

            if (flowControlData != null)
            {
                flowControlData.doneSettingAssetWaitForApp = appAttributes.Count;
                flowControlData.doneSettingAssetWaitForPreparation = preparationAttributes.Count;
                flowControlData.doneSettingAssetWaitForStage = stageAttributes.Count;

                stageAttributes.ForEach(x =>
                {
                    x.Attributes.ToList().ForEach(y =>
                    {
                        var typeName = y.GetType().ToString();
                        Debug.Log($"stage {x.Type.ToString()} type: {typeName}");
                    });
                });

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {appAttributes.Count} of <AddToCountAttribute> classes for app in the project");
            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {preparationAttributes.Count} of <AddToCountAttribute> classes for preparation in the project");
            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {stageAttributes.Count} of <AddToCountAttribute> classes for stage in the project");
        }

    }
}
