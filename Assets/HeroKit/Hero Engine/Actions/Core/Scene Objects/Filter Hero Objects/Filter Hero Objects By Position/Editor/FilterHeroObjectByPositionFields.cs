﻿// --------------------------------------------------------------
// Copyright (c) 2016-2017 Aveyond Studios. 
// All Rights Reserved.
// --------------------------------------------------------------
using HeroKit.Scene;
using SimpleGUI;
using HeroKit.Editor.ActionField;
using HeroKit.Editor.HeroField;

namespace HeroKit.Editor.ActionBlockFields
{
    /// <summary>
    /// Filter hero kit objects in a hero object list field by position.
    /// </summary>
    public static class FilterHeroObjectByPositionFields 
    {
        public static void BuildField(HeroActionParams actionParams)
        {
            HeroAction heroAction = actionParams.heroAction;

            //-----------------------------------------
            // create the action fields if they don't exist
            //-----------------------------------------
            ActionCommon.CreateActionFieldsOnHeroObject(heroAction, 11);

            //-----------------------------------------
            // create the fields for this action
            //-----------------------------------------
            SimpleLayout.BeginVertical(SimpleGUI.Fields.Box.StyleB);
            SimpleLayout.Label("Get the hero objects here:");
            GetHeroObjectField.BuildFieldB("", actionParams, heroAction.actionFields[10]);

            SimpleLayout.Label("Get hero objects in the scene at a specific position:");
            SimpleLayout.BeginHorizontal();
            GetBoolValue.BuildField("X", actionParams, heroAction.actionFields[3]);
            GetBoolValue.BuildField("Y", actionParams, heroAction.actionFields[4]);
            GetBoolValue.BuildField("Z", actionParams, heroAction.actionFields[5]);
            SimpleLayout.EndHorizontal();

            SimpleLayout.BeginVertical(SimpleGUI.Fields.Box.StyleB);
            if (showContent(heroAction, 3)) GetFloatField.BuildFieldA("X:", actionParams, heroAction.actionFields[6], true);
            if (showContent(heroAction, 4)) GetFloatField.BuildFieldA("Y:", actionParams, heroAction.actionFields[7], true);
            if (showContent(heroAction, 5)) GetFloatField.BuildFieldA("Z:", actionParams, heroAction.actionFields[8], true);
            SimpleLayout.EndVertical();

            SimpleLayout.Label("Radius to include around each coordinate:");
            GetFloatField.BuildFieldA("", actionParams, heroAction.actionFields[9]);
            SimpleLayout.EndVertical();

            SimpleLayout.BeginVertical(SimpleGUI.Fields.Box.StyleB);
            SimpleLayout.Label("Operation:");
            GetDropDownField.BuildField("", actionParams, heroAction.actionFields[0], new HeroObjectOperatorField());
            SimpleLayout.EndVertical();

            SimpleLayout.BeginVertical(SimpleGUI.Fields.Box.StyleB);
            SimpleLayout.Label("Save the hero objects here:");
            GetHeroObjectField.BuildFieldB("", actionParams, heroAction.actionFields[1]);

            SimpleLayout.Label("Maximum number of hero objects to save:");
            GetIntegerField.BuildFieldA("", actionParams, heroAction.actionFields[2]);
            SimpleLayout.EndVertical();
        }

        private static bool showContent(HeroAction heroAction, int boolID)
        {
            if (heroAction.actionFields[boolID].bools != null &&
                heroAction.actionFields[boolID].bools.Count != 0 &&
                heroAction.actionFields[boolID].bools[0] == true)
                return true;
            else
                return false;
        }

    }
}