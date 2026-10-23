using System;
using System.Collections.Generic;
using System.IO;
using Codice.Utils;
using StoryFlow.Data;
using UnityEditor;
using UnityEngine;

namespace PixelCrushers.DialogueSystem
{
    [Serializable]
    public class StoryFlowConverterPrefs : AbstractConverterWindowPrefs
    {
        //*** Add any settings your window needs to remember here.
    }

    public class StoryFlowConverterWindow : AbstractConverterWindow<StoryFlowConverterPrefs>
    {
        string LANGUAGECODE = "en.";
        int baseID = 10000;

        //*** Set the source file extension here:
        public override string sourceFileExtension { get { return "asset"; } }

        //*** Set the EditorPrefs key to save preferences under:
        public override string prefsKey { get { return "StoryFlowKey"; } }

        //*** Customize this menu item:
        [MenuItem("Tools/Pixel Crushers/Dialogue System/Import/StoryFlow Asset....", false, 1)]
        public static void Init()
        {
            EditorWindow.GetWindow(typeof(StoryFlowConverterWindow), false, "Converter");
        }

        //*** Basic preferences are stored in a variable named 'prefs' of type Prefs. You can
        //*** create a subclass of Prefs if you need to store additional data. If you do this,
        //*** also override the ClearPrefs(), LoadPrefs(), and SavePrefs() methods.

        //*** This is the main conversion routine.
        //*** Read prefs.SourceFile (or whatever source data you need, if you've overridden
        //*** the prefs object) and copy the data into the dialogue database object.

        protected DialogueDatabase LoadOrCreateDatabase(string dbName)
        {
            string assetPath = string.Format("{0}/{1}.asset", prefs.outputFolder, dbName);
            DialogueDatabase database = null;
            if (prefs.overwrite)
            {
                database = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogueDatabase)) as DialogueDatabase;
                if (database != null)
                {
                    if (!prefs.merge)
                    {
                        database.Clear();
                    }
                    database.SyncAll();
                }
            }
            if (database == null)
            {
                assetPath = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/{1}.asset", prefs.outputFolder, dbName));
                database = DatabaseUtility.CreateDialogueDatabaseInstance();
                database.name = prefs.databaseFilename;
                AssetDatabase.CreateAsset(database, assetPath);
            }
            return database;
        }

        protected override void Convert()
        {
            string fullPath = Path.GetFullPath(prefs.sourceFilename);
            string assetsPath = Path.GetFullPath(Application.dataPath);

            string assetPath = "Assets" + fullPath.Substring(assetsPath.Length).Replace('\\', '/');

            StoryFlowProjectAsset sf = AssetDatabase.LoadAssetAtPath<StoryFlowProjectAsset>(assetPath);
            if (sf == null)
            {
                Debug.LogError($"Error loading StoryFlow project asset: {assetPath}");
                return;
            }

            if (template == null)
                template = TemplateTools.LoadFromEditorPrefs();

            CreateGlobalDatabase("Global", sf);
            CreateLocalDatabase(sf);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (prefs.overwrite && DialogueEditor.DialogueEditorWindow.instance != null)
            {
                DialogueEditor.DialogueEditorWindow.instance.Reset();
            }
        }

        void CreateGlobalDatabase(string dbName, StoryFlowProjectAsset sf)
        {
            DialogueDatabase database = GetDatabase(dbName);
            if (database == null)
                return;

            // Global Variables
            

            // Characters
            database.actors = ConvertCharactersToActors(database, sf);

            TouchUpDialogueDatabase(database);
            EditorUtility.SetDirty(database);

            Debug.Log(string.Format("{0}: Created database '{1}' containing {2} actors, {3} conversations, {4} items/quests, {5} variables, and {6} locations.",
              DialogueDebug.Prefix, dbName, database.actors.Count, database.conversations.Count, database.items.Count, database.variables.Count, database.locations.Count), database);
        }

        void CreateLocalDatabase(StoryFlowProjectAsset sf)
        {
            Dictionary<string, DialogueDatabase> dictDatabases = new();

            ConvertScriptsToEvents(dictDatabases, sf);


            SaveLocalDatabases(dictDatabases);
        }

        void ConvertScriptsToEvents(Dictionary<string, DialogueDatabase> dictDatabases, StoryFlowProjectAsset sf)
        {
            // Loop through story flow scripts
            foreach (StoryFlowProjectAsset.ScriptReference reference in sf.ScriptReferences)
            {
                string databaseName = string.Empty;
                StoryFlowScriptAsset script = reference.Asset;

                #region Variable

                // Does script contain Database variable
                bool containsDatabase = false;
                foreach (StoryFlowVariable variable in script.Variables.Values)
                {
                    // save all variables here?

                    // If yes get Database name
                    if (variable.Name == "Database")
                    {
                        containsDatabase = true;

                        string key = variable.Value.StringValue;
                        databaseName = script.Strings[LANGUAGECODE + key];

                        break;
                    }
                }

                if (!containsDatabase)
                    continue;

                #endregion

                DialogueDatabase db = null;

                #region Database

                // Check if database already exists in dictionary
                if (dictDatabases.ContainsKey(databaseName))
                {
                    db = dictDatabases[databaseName];
                }
                else
                {
                    db = GetDatabase(databaseName);
                }

                if (db == null)
                    continue;

                dictDatabases.Add(databaseName, db);

                #endregion


                int conversationID = template.GetNextConversationID(db);
                string conversationTitle = databaseName + "/" + "Start";
                Conversation conversation = template.CreateConversation(conversationID, conversationTitle);
                db.AddConversation(conversation);

                DialogueEntry startEntry = template.CreateDialogueEntry(0, conversationID, "START");
                startEntry.Sequence = "None()";
                conversation.dialogueEntries.Add(startEntry);


                foreach (StoryFlowNode node in script.Nodes.Values)
                {
                    if (node.Type == StoryFlowNodeType.Start)
                        continue;

                    if (node.Type == StoryFlowNodeType.Dialogue)
                    {
                        string key = node.Data["title"];
                        string title = script.GetString(LANGUAGECODE + key);

                        int entryID = template.GetNextDialogueEntryID(conversation);
                        DialogueEntry entry = template.CreateDialogueEntry(entryID, conversationID, title);
                        conversation.dialogueEntries.Add(entry);
                    }
                }

            }
        }

        void SaveLocalDatabases(Dictionary<string, DialogueDatabase> dictDatabases)
        {
            foreach (string key in dictDatabases.Keys)
            {
                DialogueDatabase saveDB = dictDatabases[key];

                TouchUpDialogueDatabase(saveDB);
                EditorUtility.SetDirty(saveDB);

                Debug.Log(string.Format("{0}: Created database '{1}' containing {2} actors, {3} conversations, {4} items/quests, {5} variables, and {6} locations.",
                  DialogueDebug.Prefix, key, saveDB.actors.Count, saveDB.conversations.Count, saveDB.items.Count, saveDB.variables.Count, saveDB.locations.Count), saveDB);
            }
        }


        List<Actor> ConvertCharactersToActors(DialogueDatabase db, StoryFlowProjectAsset sf)
        {
            List<Actor> actors = new();

            // Loop through story flow characters
            foreach (StoryFlowProjectAsset.CharacterReference reference in sf.CharacterReferences)
            {
                StoryFlowCharacterAsset character = reference.Asset;

                string actorName = sf.GetGlobalString(LANGUAGECODE + character.CharacterName);
                string ID = string.Empty;


                // Create temp fields
                List<Field> fields = new();
                fields.Add(new Field("Name", actorName, FieldType.Text));

                // Save all character variables to fields
                foreach (StoryFlowVariable variable in character.Variables)
                {
                    // Determine Types
                    FieldType type = FieldType.Text;
                    if (variable.Type == StoryFlowVariableType.Boolean)
                    {
                        type = FieldType.Boolean;
                    }
                    else if (variable.Type == StoryFlowVariableType.Float || variable.Type == StoryFlowVariableType.Integer)
                    {
                        type = FieldType.Number;
                    }

                    string name = variable.Name;
                    string value = sf.GetGlobalString(LANGUAGECODE + variable.Value.StringValue);
                    fields.Add(new Field(variable.Name, value, type));

                    if (variable.Name == "ID")
                    {
                        ID = variable.Name;
                    }
                }

                // Get actor based on ID
                Actor actor = db.GetActor(ID);
                if (actor == null)
                {
                    actor = new();
                    actor.fields = new();
                }

                actor.fields = fields;
                actors.Add(actor);
            }

            return actors;
        }

        DialogueDatabase GetDatabase(string databaseName)
        {
            DialogueDatabase database = LoadOrCreateDatabase(databaseName);
            database.baseID = baseID;
            baseID += 10000;

            if (database == null)
            {
                Debug.LogError(string.Format("{0}: Couldn't create asset '{1}'.", DialogueDebug.Prefix, databaseName));
                return null;
            }

            return database;
        }








        //*** Uncomment this method and change it if you want to change the way the converter
        //*** touches up the database after copying the source data. The base version of this
        //*** method edits the START nodes of all conversations and sets their Sequence fields
        //*** to None(). For example, if you know where the actors' portrait textures are,
        //*** You can also call FindPortraitTextures(database, portraitFolder), which will 
        //*** assign the actors' portrait images based on their Textures fields.
        //protected override void TouchUpDialogueDatabase(DialogueDatabase database) {
        //	base.TouchUpDialogueDatabase(database);
        //}

        //*** This is a subclass of AbstractConverterWindow. All methods in AbstractConverterWindow
        //*** are overrideable, so you can really customize it however you want by overriding
        //*** specific methods.

    }

}
