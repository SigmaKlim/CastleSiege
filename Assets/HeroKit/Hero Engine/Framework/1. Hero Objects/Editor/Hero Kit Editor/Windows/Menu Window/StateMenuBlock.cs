﻿// --------------------------------------------------------------
// Copyright (c) 2016-2017 Aveyond Studios. 
// All Rights Reserved.
// --------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using HeroKit.Scene;
using SimpleGUI;
using SimpleGUI.Fields;
using System.Collections.Generic;
using System.Linq;

namespace HeroKit.Editor
{
    /// <summary>
    /// Menu Block for Hero State that appears in Hero Kit Editor. (Hero Object)
    /// </summary>
    internal class StateMenuBlock : EditorWindow
    {
        // --------------------------------------------------------------
        // Variables
        // --------------------------------------------------------------

        /// <summary>
        /// The hero object.
        /// </summary>
        private static HeroObject heroObject;
        /// <summary>
        /// Name of the block.
        /// </summary>
        private const string blockName = "State";
        
        /// <summary>
        /// List of states.
        /// </summary>
        private static List<HeroState> items;
        /// <summary>
        /// List of states where context menu was openend.
        /// </summary>
        private static List<HeroState> itemsContextMenu;
        /// <summary>
        /// List of states that were copied (and can be pasted somewhere).
        /// </summary>
        private static List<HeroState> savedFieldList;
        /// <summary>
        /// List of the most recently deleted states.
        /// </summary>
        private static readonly LinkedList<HeroState> deletedFields;
        /// <summary>
        /// List of the index values for the most recently deleted states.
        /// The index tells us where the state was in the list before it was deleted.
        /// </summary>
        private static readonly LinkedList<int> deletedFieldsIndex;
        /// <summary>
        /// Indent level for the items in the menu.
        /// </summary>
        private static readonly int indentMenu = HeroObjectMenuBlock.indentLevel;

        // --------------------------------------------------------------
        // Methods (General)
        // --------------------------------------------------------------

        /// <summary>
        /// Constructor.
        /// </summary>
        static StateMenuBlock()
        {
            // create deleted field list
            deletedFields = new LinkedList<HeroState>();
            deletedFieldsIndex = new LinkedList<int>();
        }
        /// <summary>
        /// Block to display in the menu. Get list from hero kit object.
        /// </summary>
        /// <param name="heroKitObject">Hero object info to display in the menu.</param>
        public static void Block(HeroObject heroKitObject)
        {
            // exit early if object is null
            if (heroKitObject == null)
            {
                return;
            }

            // assign hero object to this class
            heroObject = heroKitObject;
            items = heroObject.states.states;

            // draw components
            DrawBlock();
        }
        /// <summary>
        /// Draw the block.
        /// </summary>
        private static void DrawBlock()
        {
            // draw the states
            DrawHeading();
            DrawItems();
            addItemButton();
        }
        /// <summary>
        /// Draw the heading for this block.
        /// </summary>
        private static void DrawHeading()
        {
            // get the box to draw around the foldout
            GUIStyle style = Box.StyleMenu;
            GUIStyle buttonStyle = Button.StyleFoldoutHeading;
            if (HeroObjectMenuBlock.stateFocus && HeroObjectMenuBlock.stateID == -1)
            {
                style = Box.StyleMenuSelected;
                buttonStyle = Button.StyleFoldoutHeadingB;
            }

            // show foldout
            SimpleLayout.BeginHorizontal(style);
            GUIStyle foldoutStyle = (heroObject.states.visible) ? Button.StyleFoldoutOpen : Button.StyleFoldoutClosed;
            SimpleLayout.Button("", toggleBlock, foldoutStyle, 10);
            SimpleLayout.Button("States", showBlockTitle, buttonStyle);
            SimpleLayout.EndHorizontal();
        }
        /// <summary>
        /// Draw the body of the block.
        /// </summary>
        private static void DrawItems()
        {
            if (!heroObject.states.visible) return;

            // exit early if there are no items
            if (items == null || items.Count == 0) return;

            // display items  
            for (int i = 0; i < items.Count; i++)
            {
                // get the box to draw around the foldout
                GUIStyle style = Box.StyleDefault;
                GUIStyle buttonStyle = Button.StyleFoldoutText;
                if (HeroObjectMenuBlock.stateFocus && HeroObjectMenuBlock.stateID == i && HeroObjectMenuBlock.eventID == -1 && HeroObjectMenuBlock.actionID == -1)
                {
                    style = Box.StyleMenuSelected;
                    buttonStyle = Button.StyleFoldoutTextB;
                }

                // show foldout
                SimpleLayout.BeginHorizontal(style);
                SimpleLayout.Space(indentMenu);
                GUIStyle foldoutStyle = (items[i].visible) ? Button.StyleFoldoutOpen : Button.StyleFoldoutClosed;
                SimpleLayout.Button("", toggleBlock, showContextMenu, i, foldoutStyle, 10);
                SimpleLayout.Button(blockName + " " + i + ": " + items[i].name, showBlockContent, showContextMenu, i, buttonStyle);
                SimpleLayout.EndHorizontal();

                // show events for state
                DrawEventForState(i);
            }
        }
        /// <summary>
        /// Draw the events for this state.
        /// </summary>
        /// <param name="eventIndex">ID of this state.</param>
        private static void DrawEventForState(int stateIndex)
        {
            // exit early if an event has been removed
            if (stateIndex >= items.Count)
            {
                return;
            }

            // draw the events for this state
            if (items[stateIndex].visible)
                EventMenuBlock.Block(heroObject, stateIndex);
        }
        /// <summary>
        /// Button to add a new item to list.
        /// </summary>
        public static void addItemButton()
        {
            if (!heroObject.states.visible) return;
            HeroKitCommon.DrawAddNewItem("State", addItem, Button.StyleAddMenuItem, indentMenu);
            SimpleLayout.Space(5);
        }

        // --------------------------------------------------------------
        // Methods (On Click State in Menu)
        // --------------------------------------------------------------

        /// <summary>
        /// Toggle the container for all state menu blocks on and off.
        /// </summary>
        private static void toggleBlock()
        {
            HeroKitCommon.deselectField();
            HeroKitCommon.toggleBool(ref heroObject.states.visible);
        }
        /// <summary>
        /// Toggle a state menu block on and off.
        /// </summary>
        /// <param name="i">ID assigned to the state.</param>
        private static void toggleBlock(int i)
        {
            HeroKitCommon.deselectField();
            HeroKitCommon.toggleBool(ref heroObject.states.states[i].visible);
        }

        /// <summary>
        /// Show the state in the canvas window.
        /// </summary>
        /// <param name="stateID">ID of the state.</param>
        public static void showBlockContent(int stateID)
        {
            HeroKitCommon.ResetCanvasContent();
            HeroObjectMenuBlock.typeID = 1;
            HeroObjectMenuBlock.stateFocus = true;
            HeroObjectMenuBlock.stateID = stateID;
        }
        /// <summary>
        /// Show States in the canvas window. 
        /// </summary>
        public static void showBlockTitle()
        {
            HeroKitCommon.ResetCanvasContent();
            HeroObjectMenuBlock.stateFocus = true;
            HeroObjectMenuBlock.typeID = 1;
        }

        // --------------------------------------------------------------
        // Methods (Context Menu)
        // --------------------------------------------------------------

        /// <summary>
        /// When a state title is right-clicked, display the context menu for it.
        /// </summary>
        /// <param name="stateID">ID of the state.</param>
        public static void showContextMenu(int stateID)
        {
            int buttonID = stateID;
            HeroObjectMenuBlock.stateIndexContext = stateID;
            HeroObjectMenuBlock.eventIndexContext = -1;
            HeroObjectMenuBlock.actionIndexContext = -1;

            itemsContextMenu = heroObject.states.states;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("New " + blockName + " Above"), false, addItemAbove, buttonID);
            menu.AddItem(new GUIContent("New " + blockName + " Below"), false, addItemBelow, buttonID);
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Copy " + blockName), false, copyItem, buttonID);
            menu.AddSeparator("");

            if (savedFieldList != null && savedFieldList.Count != 0)
            {
                menu.AddItem(new GUIContent("Paste " + blockName + " Above"), false, pasteItemAbove, buttonID);
                menu.AddItem(new GUIContent("Paste " + blockName + " Below"), false, pasteItemBelow, buttonID);
                menu.AddItem(new GUIContent("Paste " + blockName + " Here"), false, pasteItemHere, buttonID);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste " + blockName + " Above"));
                menu.AddDisabledItem(new GUIContent("Paste " + blockName + " Below"));
                menu.AddDisabledItem(new GUIContent("Paste " + blockName + " Here"));
            }
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Delete " + blockName), false, deleteItem, buttonID);
            if (deletedFieldsIndex != null && deletedFieldsIndex.Count != 0)
            {
                menu.AddItem(new GUIContent("Restore Last Deleted " + blockName), false, restoreItem, buttonID);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Restore Last Deleted " + blockName));
            }
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Move " + blockName + " Up"), false, moveItemUp, buttonID);
            menu.AddItem(new GUIContent("Move " + blockName + " Down"), false, moveItemDown, buttonID);
            menu.ShowAsContext();
        }
        /// <summary>
        /// Move item up.
        /// </summary>
        /// <param name="obj">Item to move up.</param>
        private static void moveItemUp(object obj)
        {
            int index = (int)obj;

            HeroKitCommon.deselectField();
            int indexA = index - 1;
            int indexB = index;

            if (indexA >= 0)
            {
                HeroState fieldA = itemsContextMenu[indexA];
                HeroState fieldB = itemsContextMenu[indexB];
                itemsContextMenu[indexA] = fieldB;
                itemsContextMenu[indexB] = fieldA;
            }
        }
        /// <summary>
        /// Move item down.
        /// </summary>
        /// <param name="obj">Item to move down.</param>
        private static void moveItemDown(object obj)
        {
            int index = (int)obj;

            HeroKitCommon.deselectField();
            int indexA = index;
            int indexB = index + 1;

            if (indexB < itemsContextMenu.Count)
            {
                HeroState fieldA = itemsContextMenu[indexA];
                HeroState fieldB = itemsContextMenu[indexB];
                itemsContextMenu[indexA] = fieldB;
                itemsContextMenu[indexB] = fieldA;
            }
        }

        /// <summary>
        /// Add item at end of list.
        /// </summary>
        private static void addItem()
        {
            HeroKitCommon.deselectField();
            items.Add(new HeroState());
        }
        /// <summary>
        /// Add item at specific position in the list.
        /// </summary>
        /// <param name="index">Index in list where item should be added.</param>
        private static void addItem(int index)
        {
            HeroKitCommon.deselectField();
            items.Insert(index, new HeroState());
        }
        /// <summary>
        /// Add item above another item in the list.
        /// </summary>
        /// <param name="obj">The other item.</param>
        public static void addItemAbove(object obj)
        {
            int index = (int)obj;

            HeroKitCommon.deselectField();
            itemsContextMenu.Insert(index, new HeroState());
        }
        /// <summary>
        /// Add item below another item in the list.
        /// </summary>
        /// <param name="obj">The other item.</param>
        public static void addItemBelow(object obj)
        {
            int index = (int)obj + 1;

            HeroKitCommon.deselectField();
            itemsContextMenu.Insert(index, new HeroState());
        }

        /// <summary>
        /// Copy an item.
        /// </summary>
        /// <param name="obj">The item.</param>
        private static void copyItem(object obj)
        {
            int index = (int)obj;

            savedFieldList = new List<HeroState>();
            savedFieldList.Add(new HeroState(itemsContextMenu[index]));
        }

        /// <summary>
        /// Insert item at the end of the list.
        /// </summary>
        private static void pasteItem()
        {
            // paste at end of list
            if (savedFieldList != null)
            {
                HeroKitCommon.deselectField();
                List<HeroState> itemsToPaste = new List<HeroState>(savedFieldList.Select(x => x.Clone(x)));
                itemsContextMenu.AddRange(itemsToPaste);
            }
        }
        /// <summary>
        /// Insert item at a specific index in the list.
        /// </summary>
        /// <param name="index">The index where the items should be inserted.</param>
        private static void pasteItem(int index)
        {
            // paste at specific location in list
            if (savedFieldList != null)
            {
                HeroKitCommon.deselectField();
                List<HeroState> itemsToPaste = new List<HeroState>(savedFieldList.Select(x => x.Clone(x)));
                itemsContextMenu.InsertRange(index, itemsToPaste);
            }
        }
        /// <summary>
        /// Insert item above another item in the list.
        /// </summary>
        /// <param name="obj">The other item.</param>
        public static void pasteItemAbove(object obj)
        {
            // this is called when item right clicked
            pasteItem((int)obj);
        }
        /// <summary>
        /// Insert item below another item in the list.
        /// </summary>
        /// <param name="obj">The other item.</param>
        public static void pasteItemBelow(object obj)
        {
            // this is called when item right clicked
            pasteItem((int)obj + 1);
        }
        /// <summary>
        /// Delete an item and replace it with this item.
        /// </summary>
        /// <param name="obj">The other item.</param>
        public static void pasteItemHere(object obj)
        {
            int index = (int)obj;

            // paste at specific location in list
            if (savedFieldList != null)
            {
                HeroKitCommon.deselectField();
                List<HeroState> itemsToPaste = new List<HeroState>(savedFieldList.Select(x => x.Clone(x)));
                itemsContextMenu.RemoveAt(index);
                itemsContextMenu.InsertRange(index, itemsToPaste);
            }
        }

        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="object">The item.</param>
        private static void deleteItem(object obj)
        {
            int index = (int)obj;

            if (itemsContextMenu.Count > index)
            {
                HeroKitCommon.deselectField();
                saveDeletedItem(index);
                itemsContextMenu.RemoveAt(index);
            }
            else
            {
                Debug.LogWarning("Delete " + blockName + "s" + ": Item at index [" + index + "] does not exist");
            }
        }
        /// <summary>
        /// Store a deleted item for future restoration.
        /// </summary>
        /// <param name="stateID">ID of the state.</param>
        private static void saveDeletedItem(int stateID)
        {
            // add to deleted item to front of the stack
            HeroState t = new HeroState();
            t = t.Clone(itemsContextMenu[stateID]);

            deletedFields.AddFirst(t);
            deletedFieldsIndex.AddFirst(stateID);

            // if there are too many items in the stack, pop the last item in the stack
            if (deletedFields.Count > 10)
            {
                deletedFields.RemoveLast();
                deletedFieldsIndex.RemoveLast();
            }
        }

        /// <summary>
        /// Restor the last item that was deleted from the list.
        /// </summary>
        private static void restoreItem(object obj)
        {
            if (deletedFieldsIndex.Count > 0)
            {
                HeroKitCommon.deselectField();
                int index = deletedFieldsIndex.First();
                HeroState field = deletedFields.First();

                // get state that contains the list of events
                List<HeroState> list = heroObject.states.states;

                // insert this event in the list
                list.Insert(index, field);

                // delete fields
                deletedFields.RemoveFirst();
                deletedFieldsIndex.RemoveFirst();
            }
        }
    }
}