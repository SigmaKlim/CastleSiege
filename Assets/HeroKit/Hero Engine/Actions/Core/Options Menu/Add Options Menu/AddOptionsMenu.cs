﻿// --------------------------------------------------------------
// Copyright (c) 2016-2017 Aveyond Studios. 
// All Rights Reserved.
// --------------------------------------------------------------
using UnityEngine;
using System;

namespace HeroKit.Scene.Actions
{

    /// <summary>
    /// Add an options menu to the game, but do not show it.
    /// </summary>
    public class AddOptionsMenu : IHeroKitAction
    {
        // set up properties needed for all actions
        private HeroKitObject _heroKitObject;
        public HeroKitObject heroKitObject
        {
            get { return _heroKitObject; }
            set { _heroKitObject = value; }
        }
        private int _eventID;
        public int eventID
        {
            get { return _eventID; }
            set { _eventID = value; }
        }
        private bool _updateIsDone;
        public bool updateIsDone
        {
            get { return _updateIsDone; }
            set { _updateIsDone = value; }
        }

        // This is used by HeroKitCommon.GetAction() to add this action to the ActionDictionary. Don't delete!
        public static AddOptionsMenu Create()
        {
            AddOptionsMenu action = new AddOptionsMenu();
            return action;
        }

        // Execute the action
        public int Execute(HeroKitObject hko)
        {
            heroKitObject = hko;

            // add menu to scene if it doesn't exist
            HeroKitObject targetObject = HeroKitCommonRuntime.GetPrefabFromAssets(HeroKitCommonRuntime.settingsInfo.optionsMenu, true);
            bool runThis = (targetObject != null);

            if (runThis)
            {
                targetObject.gameObject.SetActive(true);
            }

            if (heroKitObject.debugHeroObject) Debug.Log(HeroKitCommonRuntime.GetActionDebugInfo(heroKitObject));

            return -99;
        }


        // not used
        public bool RemoveFromLongActions()
        {
            throw new NotImplementedException();
        }
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}