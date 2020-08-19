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
        [MenuItem("Assets/Walkio/Game/Flow Control/Count Asset Wait Attribute")]
        public static void CountAssetWaitAttribute()
        {
            var typesWithAddToCountAttributeDoneLoading =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(DoneLoadingAssetWaitAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<DoneLoadingAssetWaitAttribute>() };

            var addToCountAttributesDoneLoading = typesWithAddToCountAttributeDoneLoading.ToList();
            var appAttributesDoneLoading =
                addToCountAttributesDoneLoading
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0)
                    .ToList();
            var appAssistAttributesDoneLoading =
                addToCountAttributesDoneLoading
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var preparationAttributesDoneLoading =
                addToCountAttributesDoneLoading
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0)
                    .ToList();
            var preparationAssistAttributesDoneLoading =
                addToCountAttributesDoneLoading
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var stageAttributesDoneLoading =
                addToCountAttributesDoneLoading
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0)
                    .ToList();
            var stageAssistAttributesDoneLoading =
                addToCountAttributesDoneLoading
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var typesWithAddToCountAttributeDoneSetting =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(DoneSettingAssetWaitAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<DoneSettingAssetWaitAttribute>() };

            var addToCountAttributesDoneSetting = typesWithAddToCountAttributeDoneSetting.ToList();
            var appAttributesDoneSetting =
                addToCountAttributesDoneSetting
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0)
                    .ToList();
            var appAssistAttributesDoneSetting =
                addToCountAttributesDoneSetting
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var preparationAttributesDoneSetting =
                addToCountAttributesDoneSetting
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0)
                    .ToList();
            var preparationAssistAttributesDoneSetting =
                addToCountAttributesDoneSetting
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var stageAttributesDoneSetting =
                addToCountAttributesDoneSetting
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0)
                    .ToList();
            var stageAssistAttributesDoneSetting =
                addToCountAttributesDoneSetting
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();


            // var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
            //     "Packages/com.walkio.game.flow-control.common/Data Assets/Flow Control Data.asset");
            var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
                $"Assets/_/1 - Game/Module - Flow Control - App/Data Assets/Flow Control Data.asset");

            if (flowControlData != null)
            {
                flowControlData.doneLoadingAssetWaitForApp = appAttributesDoneLoading.Count;
                flowControlData.doneLoadingAssetWaitForPreparation = preparationAttributesDoneLoading.Count;
                flowControlData.doneLoadingAssetWaitForStage = stageAttributesDoneLoading.Count;

                flowControlData.doneLoadingAssetWaitForAppAssist = appAssistAttributesDoneLoading.Count;
                flowControlData.doneLoadingAssetWaitForPreparationAssist = preparationAssistAttributesDoneLoading.Count;
                flowControlData.doneLoadingAssetWaitForStageAssist = stageAssistAttributesDoneLoading.Count;

                flowControlData.doneSettingAssetWaitForApp = appAttributesDoneSetting.Count;
                flowControlData.doneSettingAssetWaitForPreparation = preparationAttributesDoneSetting.Count;
                flowControlData.doneSettingAssetWaitForStage = stageAttributesDoneSetting.Count;

                flowControlData.doneSettingAssetWaitForAppAssist = appAssistAttributesDoneSetting.Count;
                flowControlData.doneSettingAssetWaitForPreparationAssist = preparationAssistAttributesDoneSetting.Count;
                flowControlData.doneSettingAssetWaitForStageAssist = stageAssistAttributesDoneSetting.Count;

                // appAttributesDoneLoading.ForEach(x =>
                // {
                //     x.Attributes.ToList().ForEach(y =>
                //     {
                //         var typeName = y.GetType().ToString();
                //         Debug.Log($"app {x.Type.ToString()} type: {typeName}");
                //     });
                // });

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {appAttributesDoneLoading.Count} of <AddToCountAttribute> classes for app in the project");
            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {preparationAttributesDoneLoading.Count} of <AddToCountAttribute> classes for preparation in the project");
            Debug.Log($"CountHelper - CountDoneLoadingAssetWaitAttribute - has {stageAttributesDoneLoading.Count} of <AddToCountAttribute> classes for stage in the project");
        }


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
            var appAssistAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var preparationAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0)
                    .ToList();
            var preparationAssistAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var stageAttributes =
                addToCountAttributes
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0)
                    .ToList();
            var stageAssistAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();



            // var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
            //     "Packages/com.walkio.game.flow-control.common/Data Assets/Flow Control Data.asset");
            var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
                $"Assets/_/1 - Game/Module - Flow Control - App/Data Assets/Flow Control Data.asset");

            if (flowControlData != null)
            {
                flowControlData.doneLoadingAssetWaitForApp = appAttributes.Count;
                flowControlData.doneLoadingAssetWaitForPreparation = preparationAttributes.Count;
                flowControlData.doneLoadingAssetWaitForStage = stageAttributes.Count;

                flowControlData.doneLoadingAssetWaitForAppAssist = appAssistAttributes.Count;
                flowControlData.doneLoadingAssetWaitForPreparationAssist = preparationAssistAttributes.Count;
                flowControlData.doneLoadingAssetWaitForStageAssist = stageAssistAttributes.Count;

                appAttributes.ForEach(x =>
                {
                    x.Attributes.ToList().ForEach(y =>
                    {
                        var typeName = y.GetType().ToString();
                        Debug.Log($"app {x.Type.ToString()} type: {typeName}");
                    });
                });

                AssetDatabase.SaveAssets();
                // AssetDatabase.Refresh();
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
            var appAssistAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "App", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var preparationAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0)
                    .ToList();
            var preparationAssistAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Preparation", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            var stageAttributes =
                addToCountAttributes
                    .Where(x => String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0)
                    .ToList();
            var stageAssistAttributes =
                addToCountAttributes
                    .Where(x =>
                        String.Compare(x.Attributes.First().FlowName, "Stage", StringComparison.Ordinal) == 0
                        && String.Compare(x.Attributes.First().Category, "Assist", StringComparison.Ordinal) == 0)
                    .ToList();

            // var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
            //     "Packages/com.walkio.game.flow-control.common/Data Assets/Flow Control Data.asset");
            var flowControlData = AssetDatabase.LoadAssetAtPath<Template.FlowControlData>(
                $"Assets/_/1 - Game/Module - Flow Control - App/Data Assets/Flow Control Data.asset");

            if (flowControlData != null)
            {
                flowControlData.doneSettingAssetWaitForApp = appAttributes.Count;
                flowControlData.doneSettingAssetWaitForPreparation = preparationAttributes.Count;
                flowControlData.doneSettingAssetWaitForStage = stageAttributes.Count;

                flowControlData.doneSettingAssetWaitForAppAssist = appAssistAttributes.Count;
                flowControlData.doneSettingAssetWaitForPreparationAssist = preparationAssistAttributes.Count;
                flowControlData.doneSettingAssetWaitForStageAssist = stageAssistAttributes.Count;

                stageAttributes.ForEach(x =>
                {
                    x.Attributes.ToList().ForEach(y =>
                    {
                        var typeName = y.GetType().ToString();
                        Debug.Log($"stage {x.Type.ToString()} type: {typeName}");
                    });
                });

                AssetDatabase.SaveAssets();
                // AssetDatabase.Refresh();
            }

            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {appAttributes.Count} of <AddToCountAttribute> classes for app in the project");
            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {preparationAttributes.Count} of <AddToCountAttribute> classes for preparation in the project");
            Debug.Log($"CountHelper - CountDoneSettingAssetWaitAttribute - has {stageAttributes.Count} of <AddToCountAttribute> classes for stage in the project");
        }

    }
}
