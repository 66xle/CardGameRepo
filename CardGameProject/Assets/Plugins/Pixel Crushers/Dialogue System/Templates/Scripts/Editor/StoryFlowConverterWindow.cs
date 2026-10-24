using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Codice.Utils;
using Newtonsoft.Json;
using PixelCrushers.DialogueSystem.Articy.Articy_4_0;
using PixelCrushers.DialogueSystem.ChatMapper;
using StoryFlow.Data;
using StoryFlow.Execution;
using TextMateSharp.Grammars;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.TextCore.Text;
using static Codice.Client.Common.Locks.ServerLocks.ForWorkingBranchOnRepoByItem;
using static UnityEngine.EventSystems.EventTrigger;

namespace PixelCrushers.DialogueSystem
{
    struct Option
    {
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("text")]
        public string Text;
        [JsonProperty("onceOnly")]
        public bool OnlyOnce;
    }

    [Serializable]
    public class StoryFlowConverterPrefs : AbstractConverterWindowPrefs
    {
        //*** Add any settings your window needs to remember here.
    }

    public class StoryFlowConverterWindow : AbstractConverterWindow<StoryFlowConverterPrefs>
    {
        string LANGUAGECODE = "en.";
        int baseID;
        DialogueDatabase globalDB;
        DialogueDatabase currentLocalDB;
        StoryFlowProjectAsset sf;

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
            baseID = 10000;
            globalDB = null;

            string fullPath = Path.GetFullPath(prefs.sourceFilename);
            string assetsPath = Path.GetFullPath(Application.dataPath);

            string assetPath = "Assets" + fullPath.Substring(assetsPath.Length).Replace('\\', '/');

            sf = AssetDatabase.LoadAssetAtPath<StoryFlowProjectAsset>(assetPath);
            if (sf == null)
            {
                Debug.LogError($"Error loading StoryFlow project asset: {assetPath}");
                return;
            }

            if (template == null)
                template = TemplateTools.LoadFromEditorPrefs();

            CreateGlobalDatabase("Global");
            if (globalDB == null)
                return;

            CreateLocalDatabase();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (prefs.overwrite && DialogueEditor.DialogueEditorWindow.instance != null)
            {
                DialogueEditor.DialogueEditorWindow.instance.Reset();
            }
        }

        void CreateGlobalDatabase(string dbName)
        {
            globalDB = GetDatabase(dbName);
            if (globalDB == null)
                return;

            // Global Variables
            

            // Characters
            CreateGlobalActors(sf);

            TouchUpDialogueDatabase(globalDB);
            EditorUtility.SetDirty(globalDB);

            Debug.Log(string.Format("{0}: Created database '{1}' containing {2} actors, {3} conversations, {4} items/quests, {5} variables, and {6} locations.",
              DialogueDebug.Prefix, dbName, globalDB.actors.Count, globalDB.conversations.Count, globalDB.items.Count, globalDB.variables.Count, globalDB.locations.Count), globalDB);
        }

        void CreateLocalDatabase()
        {
            Dictionary<string, DialogueDatabase> dictDatabases = new();

            ConvertScriptsToEvents(dictDatabases);


            SaveLocalDatabases(dictDatabases);
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

        void ConvertScriptsToEvents(Dictionary<string, DialogueDatabase> dictDatabases)
        {
            // Loop through story flow scripts
            foreach (StoryFlowProjectAsset.ScriptReference reference in sf.ScriptReferences)
            {
                string databaseName = string.Empty;
                string scriptName = string.Empty;
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
                    }

                    if (variable.Name == "EventName")
                    {
                        string key = variable.Value.StringValue;
                        scriptName = script.Strings[LANGUAGECODE + key];
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
                    dictDatabases.Add(databaseName, db);
                    db.actors.Add(globalDB.GetActor("Player"));
                }

                if (db == null)
                    continue;

                currentLocalDB = db;

                #endregion


                // Start Node Flow
                string conversationName = scriptName + "/" + "Start";
                StoryFlowNode startFlow = script.Nodes["0"];
                CreateEventConversation(startFlow, conversationName, script, db);

                if (script.Flows.Count == 0)
                    return;

                List<StoryFlowNode> flowNodes = script.Nodes.Values.Where(n => n.Type == StoryFlowNodeType.EntryFlow).ToList();
                foreach (StoryFlowNode flow in flowNodes)
                {
                    string id = flow.Data["flowId"];
                    string flowName = script.Flows.First(f => f.Id == id).Name;

                    conversationName = scriptName + "/" + flowName;
                    startFlow = script.Nodes[flow.Id];
                    CreateEventConversation(startFlow, conversationName, script, db);
                }
            }
        }

        void CreateEventConversation(StoryFlowNode startFlow, string conversationName, StoryFlowScriptAsset script, DialogueDatabase db)
        {
            int conversationID = template.GetNextConversationID(db);
            string conversationTitle = conversationName;
            Conversation conversation = template.CreateConversation(conversationID, conversationTitle);
            db.AddConversation(conversation);

            Dictionary<string, DialogueEntry> dictEntry = new();
            Dictionary<string, StoryFlowConnection> dictConnection = new();
            Queue<StoryFlowNode> queue = new();

            AddNodeToConversation(startFlow, conversation, dictEntry, script);
            queue.Enqueue(startFlow);

            while (queue.Count > 0)
            {
                StoryFlowNode currentNode = queue.Dequeue();

                List<StoryFlowConnection> sourceConnections = script.GetEdgesFromSource(currentNode.Id);

                List<Option> options = new();

                foreach (StoryFlowConnection connection in sourceConnections)
                {
                    StoryFlowNode targetNode = script.Nodes[connection.Target];

                    if (!dictEntry.ContainsKey(targetNode.Id))
                        queue.Enqueue(targetNode);

                    AddNodeToConversation(targetNode, conversation, dictEntry, script);
                    AddConnectionToConversation(connection, conversation, dictEntry, script, out Option option);
                    options.Add(option);

                    //dictConnection.Add(connection.Id, connection);

                    //List<StoryFlowConnection> targetConnections = script.GetEdgesByTarget(currentNode.Id);
                    //List<StoryFlowConnection> newConnections = targetConnections.Where(c => !dictConnection.ContainsKey(c.Id)).ToList();

                    //if (newConnections.Count == 0)
                    //    continue;
                }

                foreach (Option option in options)
                {
                    if (option.OnlyOnce)
                    {
                        // create variable
                        int varID = template.GetNextVariableID(currentLocalDB);
                        string name = $"OnlyOnce.{option.ID}";
                        Variable variable = template.CreateVariable(varID, name, "false", FieldType.Boolean);
                        currentLocalDB.variables.Add(variable);

                        // set condition
                        DialogueEntry optionEntry = dictEntry[option.ID];
                        optionEntry.conditionsString = $"Variable[\"{name}\"] == false";

                        // set script
                        optionEntry.userScript = $"Variable[\"{name}\"] = true";
                    }
                }
            }
        }

        void AddNodeToConversation(StoryFlowNode node, Conversation conversation, Dictionary<string, DialogueEntry> dictEntry, StoryFlowScriptAsset script)
        {
            if (dictEntry.ContainsKey(node.Id))
                return; 

            if (node.Type == StoryFlowNodeType.Start || node.Type == StoryFlowNodeType.EntryFlow)
            {
                DialogueEntry startEntry = template.CreateDialogueEntry(0, conversation.id, "START");
                startEntry.Sequence = "None()";
                conversation.dialogueEntries.Add(startEntry);
                dictEntry.Add(node.Id, startEntry);
            }

            if (node.Type == StoryFlowNodeType.End)
            {
                DialogueEntry entry = CreateDialogueEntry(conversation, "END");
                conversation.dialogueEntries.Add(entry);
                dictEntry.Add(node.Id, entry);
            }

            if (node.Type == StoryFlowNodeType.Dialogue)
            {
                string key = node.Data["title"];
                string title = script.GetString(LANGUAGECODE + key);

                DialogueEntry entry = CreateDialogueEntry(conversation, title);
                string dialogueID = node.Data["text"];
                entry.DialogueText = script.GetString(LANGUAGECODE + dialogueID);

                conversation.dialogueEntries.Add(entry);
                dictEntry.Add(node.Id, entry);

                if (!node.Data.TryGetValue("character", out string characterKey))
                    return;

                Actor actor = GetActorFromDatabase(characterKey);
                entry.ActorID = actor.id;
                entry.ConversantID = globalDB.GetActor("Player").id;
            }
        }

        void AddConnectionToConversation(StoryFlowConnection connection, Conversation conversation, Dictionary<string, DialogueEntry> dictEntry, StoryFlowScriptAsset script,
                                         out Option currentOption)
        {
            DialogueEntry sourceEntry = dictEntry[connection.Source];
            DialogueEntry targetEntry = dictEntry[connection.Target];

            currentOption = new();

            #region Options

            StoryFlowNode node = script.Nodes[connection.Source];
            if (node.Type == StoryFlowNodeType.Dialogue && node.Data["options"] != "[]")
            {
                string json = node.Data["options"];

                List<Option> jsonOptions = JsonConvert.DeserializeObject<List<Option>>(json);
                foreach (Option option in jsonOptions)
                {
                    if (!connection.SourceHandle.Contains(option.ID))
                        continue;

                    currentOption = option;

                    DialogueEntry optionEntry = CreateDialogueEntry(conversation, "Option");
                    optionEntry.DialogueText = script.GetString(LANGUAGECODE + option.Text);
                    conversation.dialogueEntries.Add(optionEntry);
                    dictEntry.Add(option.ID, optionEntry);

                    Link sourceLink = new Link(conversation.id, sourceEntry.id, conversation.id, optionEntry.id);
                    sourceEntry.outgoingLinks.Add(sourceLink);

                    Link targetLink = new Link(conversation.id, optionEntry.id, conversation.id, targetEntry.id);
                    optionEntry.outgoingLinks.Add(targetLink);

                    if (node.Data.TryGetValue("character", out string characterKey))
                    {
                        Actor actor = GetActorFromDatabase(characterKey);
                        optionEntry.ActorID = globalDB.GetActor("Player").id;
                        optionEntry.ConversantID = actor.id;
                    }
                }

                return;
            }

            #endregion

            Link link = new Link(conversation.id, sourceEntry.id, conversation.id, targetEntry.id);
            sourceEntry.outgoingLinks.Add(link);
        }

        DialogueEntry CreateDialogueEntry(Conversation conversation, string title)
        {
            int entryID = template.GetNextDialogueEntryID(conversation);
            DialogueEntry entry = template.CreateDialogueEntry(entryID, conversation.id, title);
            entry.Sequence = string.Empty;

            return entry;
        }


        void CreateGlobalActors(StoryFlowProjectAsset sf)
        {
            List<Actor> actors = new();

            // Loop through story flow characters
            foreach (StoryFlowProjectAsset.CharacterReference reference in sf.CharacterReferences)
            {
                StoryFlowCharacterAsset character = reference.Asset;

                string actorName = sf.GetGlobalString(LANGUAGECODE + character.CharacterName);
                string ID = string.Empty;

                if (actorName == "CharacterBase")
                    continue;

                if (character.Variables.Any(v => v.Name == "IsGlobal" && !v.Value.BoolValue))
                    continue;

                Actor actor = CreateActorFromCharacter(character, actorName, globalDB);
                actors.Add(actor);
            }

            globalDB.actors = actors;
        }

        Actor CreateActorFromCharacter(StoryFlowCharacterAsset character, string actorName, DialogueDatabase db)
        {
            int actorID = template.GetNextActorID(db);
            Actor actor = template.CreateActor(actorID, actorName, false);

            // Create temp fields
            List<Field> fields = new();
            fields.Add(new Field("Name", actorName, FieldType.Text));

            // Save all character variables to fields
            foreach (StoryFlowVariable variable in character.Variables)
            {
                // Determine Types
                string value = string.Empty;
                FieldType type = FieldType.Text;
                if (variable.Type == StoryFlowVariableType.Boolean)
                {
                    type = FieldType.Boolean;
                    value = variable.Value.BoolValue.ToString();
                }
                else if (variable.Type == StoryFlowVariableType.Float || variable.Type == StoryFlowVariableType.Integer)
                {
                    type = FieldType.Number;
                    value = variable.Value.IntValue.ToString();
                }
                else
                {
                    value = sf.GetGlobalString(LANGUAGECODE + variable.Value.StringValue);
                }

                fields.Add(new Field(variable.Name, value, type));
            }

            actor.fields = fields;
            return actor;
        }


        Actor GetActorFromDatabase(string characterKey)
        {
            string path = characterKey.Replace('\\', '/');
            StoryFlowCharacterAsset character = sf.Characters[path];
            string name = sf.GetGlobalString(LANGUAGECODE + character.CharacterName);

            Actor actor = globalDB.actors.Find(a => a.Name == name);
            if (actor == null)
            {
                actor = currentLocalDB.GetActor(name);
                if (actor == null)
                {
                    actor = CreateActorFromCharacter(character, name, currentLocalDB);
                    currentLocalDB.actors.Add(actor);
                }
            }

            return actor;
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
