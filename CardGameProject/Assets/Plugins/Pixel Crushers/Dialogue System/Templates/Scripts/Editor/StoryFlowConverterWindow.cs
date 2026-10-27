using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PixelCrushers.DialogueSystem.ChatMapper;
using StoryFlow.Data;
using TextMateSharp.Grammars;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using static LuaHelper;
using static StoryFlowHelper;

namespace PixelCrushers.DialogueSystem
{
    [Serializable]
    public class StoryFlowConverterPrefs : AbstractConverterWindowPrefs
    {
    }

    /// <summary>
    /// Converts StoryFlow project assets into Pixel Crushers Dialogue System databases.
    ///
    /// Important limitation:
    /// Dialogue and graph topology are converted. StoryFlow's executable data-flow nodes
    /// (arithmetic, variable setters/getters, branch expressions, arrays, maps, media
    /// operations, etc.) do not have a universal one-to-one Dialogue System equivalent.
    /// They are retained as pass-through entries where necessary and reported as warnings;
    /// project-specific behavior should be translated into Dialogue System conditions,
    /// Script fields, or custom Lua functions.
    /// </summary>
    public class StoryFlowConverterWindow : AbstractConverterWindow<StoryFlowConverterPrefs>
    {
        [Serializable]
        private class Option
        {
            [JsonProperty("id")] public string ID;
            [JsonProperty("text")] public string Text;
            [JsonProperty("onceOnly")] public bool IsOnlyOnce;
        }

        private class PendingConnection
        {
            public DialogueEntry SourceEntry;
            public StoryFlowNode SourceNode;
            public Option Option;
        }

        private const string LanguageCode = "en.";
        private const string GlobalDatabaseName = "Global";

        private int baseID;
        private DialogueDatabase globalDB;
        private DialogueDatabase currentLocalDB;
        private StoryFlowProjectAsset project;
        private readonly HashSet<string> warnedNodeTypes = new HashSet<string>();

        public override string sourceFileExtension { get { return "asset"; } }
        public override string prefsKey { get { return "StoryFlowKey"; } }

        [MenuItem("Tools/Pixel Crushers/Dialogue System/Import/StoryFlow Asset...", false, 1)]
        public static void Init()
        {
            EditorWindow.GetWindow(typeof(StoryFlowConverterWindow), false, "StoryFlow Converter");
        }

        protected override void Convert()
        {
            baseID = 10000;
            globalDB = null;
            currentLocalDB = null;
            warnedNodeTypes.Clear();

            if (string.IsNullOrEmpty(prefs.sourceFilename))
            {
                Debug.LogError("[StoryFlow Converter] No source asset selected.");
                return;
            }

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(prefs.sourceFilename);
            }
            catch (Exception e)
            {
                Debug.LogError("[StoryFlow Converter] Invalid source path: " + e.Message);
                return;
            }

            string assetsPath = Path.GetFullPath(Application.dataPath);
            if (!fullPath.StartsWith(assetsPath, StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogError("[StoryFlow Converter] Select a StoryFlow project asset inside this Unity project's Assets folder.");
                return;
            }

            string assetPath = "Assets" + fullPath.Substring(assetsPath.Length).Replace('\\', '/');
            project = AssetDatabase.LoadAssetAtPath<StoryFlowProjectAsset>(assetPath);
            if (project == null)
            {
                Debug.LogError("[StoryFlow Converter] Could not load StoryFlowProjectAsset at: " + assetPath);
                return;
            }

            if (template == null) template = TemplateTools.LoadFromEditorPrefs();

            globalDB = GetDatabase(GlobalDatabaseName);
            if (globalDB == null) return;

            CreateGlobalActors();
            ConvertGlobalVariables();
            TouchUpDialogueDatabase(globalDB);
            EditorUtility.SetDirty(globalDB);

            ConvertScripts();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (prefs.overwrite && DialogueEditor.DialogueEditorWindow.instance != null)
                DialogueEditor.DialogueEditorWindow.instance.Reset();

            Debug.Log("[StoryFlow Converter] Conversion completed. Review warnings for StoryFlow logic that requires manual translation.");
        }

        private DialogueDatabase LoadOrCreateDatabase(string dbName)
        {
            string safeName = MakeSafeAssetName(dbName);
            string assetPath = string.Format("{0}/{1}.asset", prefs.outputFolder, safeName);
            DialogueDatabase database = null;

            if (prefs.overwrite)
            {
                database = AssetDatabase.LoadAssetAtPath<DialogueDatabase>(assetPath);
                if (database != null)
                {
                    if (!prefs.merge) database.Clear();
                    database.SyncAll();
                }
            }

            if (database == null)
            {
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                database = DatabaseUtility.CreateDialogueDatabaseInstance();
                database.name = safeName;
                AssetDatabase.CreateAsset(database, assetPath);
            }

            return database;
        }

        private DialogueDatabase GetDatabase(string databaseName)
        {
            DialogueDatabase database = LoadOrCreateDatabase(databaseName);
            if (database == null)
            {
                Debug.LogError("[StoryFlow Converter] Could not create database: " + databaseName);
                return null;
            }

            database.baseID = baseID;
            baseID += 10000;
            return database;
        }

        private static string MakeSafeAssetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "StoryFlow";
            foreach (char c in Path.GetInvalidFileNameChars()) value = value.Replace(c, '_');
            return value.Trim().Replace('/', '_').Replace('\\', '_');
        }

        private void CreateGlobalActors()
        {
            if (project.CharacterReferences == null) return;

            foreach (StoryFlowProjectAsset.CharacterReference reference in project.CharacterReferences)
            {
                StoryFlowCharacterAsset character = reference.Asset;

                string actorName = ResolveGlobalString(character.CharacterName);
                if (string.IsNullOrEmpty(actorName) || actorName == "CharacterBase") continue;

                // A character with IsGlobal=false belongs only in a local database.
                var isGlobal = character.Variables.FirstOrDefault(v => v.Name == "IsGlobal");
                if (isGlobal != null && isGlobal.Type == StoryFlowVariableType.Boolean && !isGlobal.Value.BoolValue)
                    continue;

                if (globalDB.GetActor(actorName) == null)
                {
                    Actor actor = CreateActorFromCharacter(character, actorName, globalDB);
                    globalDB.actors.Add(actor);
                }
            }
        }

        private Actor CreateActorFromCharacter(StoryFlowCharacterAsset character, string actorName, DialogueDatabase database)
        {
            int actorID = template.GetNextActorID(database);
            Actor actor = template.CreateActor(actorID, actorName, false);
            List<Field> fields = new List<Field>
            {
                new Field("Name", actorName, FieldType.Text)
            };

            if (character.Variables != null)
            {
                foreach (StoryFlowVariable variable in character.Variables)
                {
                    if (variable.Name == "IsGlobal") continue;

                    string value;
                    FieldType fieldType;
                    switch (variable.Type)
                    {
                        case StoryFlowVariableType.Boolean:
                            fieldType = FieldType.Boolean;
                            value = variable.Value.BoolValue ? "true" : "false";
                            break;
                        case StoryFlowVariableType.Integer:
                            fieldType = FieldType.Number;
                            value = variable.Value.IntValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        case StoryFlowVariableType.Float:
                            fieldType = FieldType.Number;
                            value = variable.Value.FloatValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        default:
                            fieldType = FieldType.Text;
                            value = ResolveGlobalString(variable.Value.StringValue);
                            break;
                    }

                    fields.Add(new Field(variable.Name, value ?? string.Empty, fieldType));
                }
            }

            actor.fields = fields;
            return actor;
        }

        private void ConvertGlobalVariables()
        {
            if (project.GlobalVariables == null) return;

            foreach (StoryFlowVariable sfVariable in project.GlobalVariables.Values)
            {
                if (string.IsNullOrEmpty(sfVariable.Name)) continue;
                if (globalDB.GetVariable(sfVariable.Name) != null) continue;

                FieldType fieldType;
                string initialValue = GetVariableValue(sfVariable, out fieldType);
                int id = template.GetNextVariableID(globalDB);

                Variable variable = template.CreateVariable(id, sfVariable.Name, initialValue, fieldType);
                globalDB.variables.Add(variable);
            }
        }

        private void ConvertScripts()
        {
            Dictionary<string, DialogueDatabase> databases = new();

            foreach (StoryFlowProjectAsset.ScriptReference reference in project.ScriptReferences)
            {
                StoryFlowScriptAsset script = reference.Asset;
                if (script.Nodes == null || script.Nodes.Count == 0) continue;

                string databaseName = GetScriptVariableString(script, "Database");
                string scriptName = GetScriptVariableString(script, "EventName");
                if (string.IsNullOrEmpty(databaseName))
                {
                    Debug.LogWarning("[StoryFlow Converter] Skipping script '" + script.name + "' because it has no Database variable.");
                    continue;
                }
                if (string.IsNullOrEmpty(scriptName)) scriptName = script.name;

                DialogueDatabase database;
                if (!databases.TryGetValue(databaseName, out database))
                {
                    database = GetDatabase(databaseName);
                    if (database == null) continue;

                    database.syncInfo.syncActors = true;
                    database.syncInfo.syncVariables = true;
                    database.syncInfo.syncLocations = true;
                    database.syncInfo.syncItems = true;
                    database.syncInfo.syncActorsDatabase = globalDB;
                    database.syncInfo.syncVariablesDatabase = globalDB;
                    database.syncInfo.syncLocationsDatabase = globalDB;
                    database.syncInfo.syncItemsDatabase = globalDB;
                    database.SyncAll();
                    databases.Add(databaseName, database);
                }

                currentLocalDB = database;
                ConvertScriptStart(script, scriptName, database);

                if (script.Flows == null) continue;

                List<StoryFlowNode> flowNodes = script.Nodes.Values.Where(n => n.Type == StoryFlowNodeType.EntryFlow).ToList();
                foreach (StoryFlowNode flowNode in flowNodes)
                {
                    string flowId = GetData(flowNode, "flowId");
                    var flow = script.Flows.FirstOrDefault(f => f.Id == flowId);
                    if (string.IsNullOrEmpty(flow.Name)) continue;
                    if (!flow.Name.Contains("Event")) continue;

                    ConvertConversation(flowNode, scriptName + "/" + flow.Name, script, database);
                }
            }

            foreach (KeyValuePair<string, DialogueDatabase> pair in databases)
            {
                TouchUpDialogueDatabase(pair.Value);
                EditorUtility.SetDirty(pair.Value);
                LogDatabase(pair.Key, pair.Value);
            }
            LogDatabase(GlobalDatabaseName, globalDB);
        }

        private void ConvertScriptStart(StoryFlowScriptAsset script, string scriptName, DialogueDatabase database)
        {
            StoryFlowNode start = null;
            if (script.Nodes.TryGetValue("0", out start))
                ConvertConversation(start, scriptName + "/Start", script, database);
            else
                Debug.LogWarning("[StoryFlow Converter] Script '" + script.name + "' has no node with ID '0'.");
        }

        private void ConvertConversation(StoryFlowNode startNode, string title, StoryFlowScriptAsset script, DialogueDatabase database)
        {
            if (startNode == null) return;

            int conversationID = template.GetNextConversationID(database);
            Conversation conversation = template.CreateConversation(conversationID, title);
            database.AddConversation(conversation);

            Dictionary<string, List<PendingConnection>> pendingConnections = new();
            Dictionary<string, DialogueEntry> entries = new Dictionary<string, DialogueEntry>();
            Dictionary<string, List<Option>> onceOptionsByNode = new Dictionary<string, List<Option>>();
            Dictionary<string, List<DialogueEntry>> entryFlows = new Dictionary<string, List<DialogueEntry>>();
            Dictionary<string, List<DialogueEntry>> pendingRunFlows = new Dictionary<string, List<DialogueEntry>>();
            List<DialogueEntry> resetEntries = new List<DialogueEntry>();
            Queue<StoryFlowNode> queue = new Queue<StoryFlowNode>();
            HashSet<string> visited = new HashSet<string>();
            DialogueEntry lastValidEntry = null;

            


            CreateDialogueEntry(startNode, conversation, entries, script);
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                StoryFlowNode node = queue.Dequeue();
                if (!IsValidNode(node))
                {
                    EvaluateIntermediateNode(node, script);
                }

                if (!visited.Add(node.Id)) continue;

                if (entries.TryGetValue(node.Id, out DialogueEntry sourceEntry))
                {
                    lastValidEntry = sourceEntry;

                    if (NodeHasTag(node, "OnceResetVars"))
                        resetEntries.Add(sourceEntry);
                }

                List<StoryFlowConnection> connections = script.GetEdgesFromSource(node.Id);
                List<Option> nodeOptions = GetOptions(node);

                connections = SortConnectionsByOptions(connections, nodeOptions);

                foreach (StoryFlowConnection connection in connections)
                {
                    if (!script.Nodes.TryGetValue(connection.Target, out StoryFlowNode target)) continue;

                    Option option = null;
                    if (node.Type == StoryFlowNodeType.Dialogue)
                    {
                        option = nodeOptions.FirstOrDefault(o => connection.SourceHandle.Contains(o.ID));
                    }

                    #region RunFlow

                    if (target.Type == StoryFlowNodeType.RunFlow)
                    {
                        string flowId = GetData(target, "flowId");
                        if (entryFlows.TryGetValue(flowId, out List<DialogueEntry> destinations))
                        {
                            foreach (DialogueEntry destination in destinations)
                            {
                                AddLink(conversation, lastValidEntry, destination);
                            }
                        }
                        else
                        {
                            if (!pendingRunFlows.ContainsKey(flowId))
                                pendingRunFlows[flowId] = new List<DialogueEntry>();

                            if (!pendingRunFlows[flowId].Contains(lastValidEntry))
                                pendingRunFlows[flowId].Add(lastValidEntry);
                        }
                        continue;
                    }

                    #endregion

                    if (!visited.Contains(target.Id))
                        queue.Enqueue(target);

                    if (IsValidNode(target))
                    {
                        DialogueEntry targetEntry = CreateDialogueEntry(target, conversation, entries, script, option);

                        if (targetEntry == null) continue;

                        AddSequences(targetEntry, target, script);

                        BuildLink(conversation, lastValidEntry, node, target, targetEntry, option, script, onceOptionsByNode);
                    }
                }


                #region EntryFlow
                // EntryFlow nodes can be destinations of RunFlow nodes. Register their first
                // dialogue/terminal destinations after the graph has been traversed.
                foreach (StoryFlowConnection incoming in script.GetEdgesByTarget(node.Id))
                {
                    if (!script.Nodes.TryGetValue(incoming.Source, out StoryFlowNode from) || from.Type != StoryFlowNodeType.EntryFlow) continue;

                    string flowId = GetData(from, "flowId");

                    if (!entryFlows.TryGetValue(flowId, out List<DialogueEntry> destinations))
                        entryFlows[flowId] = destinations = new();

                    if (NodeHasTag(node, "DialogueOnlyOnce"))
                    {
                        foreach (Option option in nodeOptions)
                        {
                            if (entries.TryGetValue(option.ID, out DialogueEntry optionEntry) && !destinations.Contains(optionEntry))
                                destinations.Add(optionEntry);
                        }
                    }
                    else if (!destinations.Contains(lastValidEntry))
                    {
                        destinations.Add(lastValidEntry);
                    }

                    if (pendingRunFlows.TryGetValue(flowId, out List<DialogueEntry> pending))
                    {
                        foreach (DialogueEntry origin in pending)
                        {
                            AddLink(conversation, origin, lastValidEntry);
                        }
                        pendingRunFlows.Remove(flowId);
                    }
                }

                #endregion

                
            }

            // Reset once-only options when a tagged reset node is reached.
            foreach (DialogueEntry resetEntry in resetEntries)
            {
                foreach (KeyValuePair<string, List<Option>> pair in onceOptionsByNode)
                {
                    foreach (Option option in pair.Value.Where(o => o.IsOnlyOnce))
                    {
                        string variableName = "OnlyOnce." + option.ID;
                        AppendScript(resetEntry, LuaSet(variableName, "false"));
                    }
                }
            }

            // A RunFlow whose target entry was never resolved is not silently discarded.
            foreach (KeyValuePair<string, List<DialogueEntry>> pending in pendingRunFlows)
                Debug.LogWarning("[StoryFlow Converter] Flow '" + pending.Key + "' in '" + title + "' has unresolved RunFlow links.");
        }

        private void AddSequences(DialogueEntry entry, StoryFlowNode node, StoryFlowScriptAsset script)
        {
            Actor actor = currentLocalDB.GetActor(entry.ActorID);
            if (actor != null)
            {
                string variableSequence = actor.IsPlayer ? "PlayerSequence" : "ActorSequence";
                AppendSequence(entry, GetScriptVariableData(script, "Sequence", variableSequence));
            }

            if (!node.Data.TryGetValue("audio", out string audio))
                return;

            string path = AssetDatabase.GetAssetPath(script.ResolvedAssetEntries.FirstOrDefault(asset => asset.Key == audio).Asset);
            if (path.StartsWith("Assets/Resources/"))
            {
                path = path.Substring("Assets/Resources/".Length).ToLower();
                path = Path.ChangeExtension(path, null);

                AppendSequence(entry, $"Audio({path}, speaker);");
            }

            // Sequence Tags
            if (!node.Data.TryGetValue("tags", out string tags))
                return;

            string[] results = Regex.Matches(tags, @"SQ:([^""\]]+)").Select(m => m.Groups[1].Value).ToArray();
            foreach (string result in results)
            {
                AppendSequence(entry, result + ";");
            }

            // Response Sequence Tags
            if (!node.Data.TryGetValue("tags", out string menuTags))
                return;

            string[] menuResults = Regex.Matches(tags, @"RQ:([^""\]]+)").Select(m => m.Groups[1].Value).ToArray();
            foreach (string result in menuResults)
            {
                AppendResponseMenuSequence(entry, result + ";");
            }
        }

        private void BuildLink(Conversation conversation, DialogueEntry sourceEntry, StoryFlowNode sourceNode, StoryFlowNode targetNode, DialogueEntry targetEntry,
                                Option option, StoryFlowScriptAsset script, Dictionary<string, List<Option>> onceOptionsByNode)
        {
            AddLink(conversation, sourceEntry, targetEntry);

            if (option == null) return;

            string optionText = ResolveScriptString(script, option.Text);
            targetEntry.MenuText = optionText;

            if (NodeHasTag(targetNode, "RepeatOptionText"))
            {
                targetEntry.DialogueText = optionText;
            }

            if (!option.IsOnlyOnce) return;

            string variableName = "OnlyOnce." + option.ID;

            EnsureBooleanVariable(currentLocalDB, variableName, "false", conversation.Title);

            AppendCondition(targetEntry, LuaEquals(variableName, "false"));
            AppendScript(targetEntry, LuaSet(variableName, "true"));

            if (!onceOptionsByNode.TryGetValue(sourceNode.Id, out List<Option> usedOptions))
            {
                usedOptions = new List<Option>();
                onceOptionsByNode[sourceNode.Id] = usedOptions;
            }

            if (!usedOptions.Any(o => o.ID == option.ID))
                usedOptions.Add(option);
        }

        
        private DialogueEntry CreateDialogueEntry(StoryFlowNode node, Conversation conversation, Dictionary<string, DialogueEntry> entries,
                                                  StoryFlowScriptAsset script, Option option = null)
        {
            if (entries.TryGetValue(node.Id, out DialogueEntry existing))
                return existing;

            string title = node.Type.ToString();
            string text = string.Empty;

            if (node.Type == StoryFlowNodeType.Start || node.Type == StoryFlowNodeType.EntryFlow)
            {
                existing = template.CreateDialogueEntry(0, conversation.id, "START");
                existing.Sequence = "None()";
            }
            else
            {
                if (node.Type == StoryFlowNodeType.Dialogue)
                {
                    title = ResolveScriptString(script, GetData(node, "title"));
                    text = ResolveScriptString(script, GetData(node, "text"));
                }
                else if (node.Type == StoryFlowNodeType.End)
                {
                    title = "END";
                }
                else
                {
                    title = node.Type.ToString();
                    WarnUnsupportedNode(node);
                }

                existing = CreateDialogueEntry(conversation, title);
                existing.DialogueText = text;

                if (option != null)
                {
                    ApplyOptionActor(existing, node, script);
                    entries.Add(option.ID, existing);
                }
                else
                {
                    ApplyNodeActor(existing, node, script);
                }
            }

            
            conversation.dialogueEntries.Add(existing);
            entries.Add(node.Id, existing);
            return existing;
        }

        #region StoryFlow Actions


        private void EvaluateIntermediateNode(StoryFlowNode node, StoryFlowScriptAsset script)
        {
            if (node.Type == StoryFlowNodeType.SetCharacter)
            {
                ConvertSetCharacter(node, script);
                return;
            }
        }

        private void ConvertSetCharacter(StoryFlowNode node, StoryFlowScriptAsset script)
        {
            // StoryFlow SetCharacter is stateful runtime logic. Its static initial value can
            // be represented as a Dialogue System Actor variable; changing it during a
            // conversation needs a project-specific translated Script/Lua assignment.
            string variableKey = GetData(node, "variable");
            string characterKey = GetData(node, "value");
            StoryFlowVariable sourceVariable = GetVariableFromStoryFlow(script, variableKey);
            Actor actor = GetActorFromDatabase(characterKey, script);
            if (sourceVariable == null || actor == null) return;

            Variable variable = FindDatabaseVariable(currentLocalDB, sourceVariable.Name);
            if (variable == null)
            {
                int varID = template.GetNextVariableID(currentLocalDB);
                variable = template.CreateVariable(varID, sourceVariable.Name, actor.id.ToString(), FieldType.Actor);
                currentLocalDB.variables.Add(variable);

                return;
            }

            variable.InitialValue = actor.id.ToString();
        }

        #endregion

        #region StoryFlow Methods

        private StoryFlowVariable GetVariableFromStoryFlow(StoryFlowScriptAsset script, string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            StoryFlowVariable variable = script.GetVariable(key);
            return variable ?? project.GetGlobalVariable(key);
        }

        private bool IsValidNode(StoryFlowNode node)
        {
            if (node == null) return false;

            switch (node.Type)
            {
                case StoryFlowNodeType.Dialogue:
                case StoryFlowNodeType.Start:
                case StoryFlowNodeType.EntryFlow:
                case StoryFlowNodeType.End:
                    return true;

                default:
                    return false;
            }
        }


        #endregion


        #region Option

        private List<StoryFlowConnection> SortConnectionsByOptions(List<StoryFlowConnection> connections, List<Option> options)
        {
            if (options == null || options.Count == 0)
                return connections;

            return connections.OrderBy(connection =>
            {
                if (string.IsNullOrEmpty(connection.SourceHandle))
                    return int.MaxValue;

                int index = options.FindIndex(option => connection.SourceHandle.Contains(option.ID));

                return index >= 0 ? index : int.MaxValue;
            }).ToList();
        }

        private List<Option> GetOptions(StoryFlowNode node)
        {
            List<Option> result = new List<Option>();
            if (node == null || node.Type != StoryFlowNodeType.Dialogue) return result;

            string json = GetData(node, "options");
            if (string.IsNullOrEmpty(json) || json == "[]") return result;

            try
            {
                result = JsonConvert.DeserializeObject<List<Option>>(json) ?? new List<Option>();
            }
            catch (Exception e)
            {
                Debug.LogWarning("[StoryFlow Converter] Could not parse options on node '" + node.Id + "': " + e.Message);
            }
            return result;
        }

        #endregion


        #region Conversation Methods

        private void ApplyNodeActor(DialogueEntry entry, StoryFlowNode node, StoryFlowScriptAsset script)
        {
            string characterKey = GetData(node, "characterRefId");
            if (!string.IsNullOrEmpty(characterKey))
            {
                Actor actor = GetActorFromDatabase(characterKey, script);
                if (actor != null)
                {
                    entry.ActorID = actor.id;
                    Actor player = GetActorFromDatabase(script, "Player");
                    entry.ConversantID = player.id;
                }
                return;
            }

            Variable actorVar = FindDatabaseVariable(currentLocalDB, "Actor");
            Variable conversantVar = FindDatabaseVariable(currentLocalDB, "Conversant");

            if (actorVar != null && int.TryParse(actorVar.InitialValue, out int actorID))
                entry.ActorID = actorID;

            if (conversantVar != null && int.TryParse(conversantVar.InitialValue, out int conversantID))
                entry.ConversantID = conversantID;
        }

        private void ApplyOptionActor(DialogueEntry entry, StoryFlowNode node, StoryFlowScriptAsset script)
        {
            Actor player = GetActorFromDatabase(script, "Player");
            entry.ActorID = player.id;

            string characterKey = GetData(node, "characterRefId");
            if (!string.IsNullOrEmpty(characterKey))
            {
                Actor actor = GetActorFromDatabase(characterKey, script);
                if (actor != null) entry.ConversantID = actor.id;
            }
            else
            {
                Variable actorVar = FindDatabaseVariable(currentLocalDB, "Actor");
                if (actorVar != null && int.TryParse(actorVar.InitialValue, out int conversantID))
                    entry.ConversantID = conversantID;
            }
        }

        private DialogueEntry CreateDialogueEntry(Conversation conversation, string title)
        {
            int entryID = template.GetNextDialogueEntryID(conversation);
            DialogueEntry entry = template.CreateDialogueEntry(entryID, conversation.id, title ?? string.Empty);
            entry.Sequence = string.Empty;
            return entry;
        }

        private void AddLink(Conversation conversation, DialogueEntry source, DialogueEntry target)
        {
            if (source == null || target == null) return;
            source.outgoingLinks.Add(new Link(conversation.id, source.id, conversation.id, target.id));
        }
        

        #endregion

        #region Database Methods

        private Actor GetActorFromDatabase(StoryFlowScriptAsset script, string name)
        {
            StoryFlowProjectAsset.CharacterReference characterReference = project.CharacterReferences.FirstOrDefault(c => c.Path.Contains(name, StringComparison.OrdinalIgnoreCase));
            string characterKey = characterReference.Asset.CharacterName;
            Actor actor = GetActorFromDatabase(characterKey, script);

            return actor;
        }

        private Actor GetActorFromDatabase(string characterKey, StoryFlowScriptAsset script)
        {
            if (string.IsNullOrEmpty(characterKey)) return null;

            string characterName = characterKey.Contains(".cf_name") ? characterKey : characterKey + ".cf_name";
            string name = ResolveGlobalString(characterName);
            if (string.IsNullOrEmpty(name)) name = ResolveGlobalString(characterKey);
            if (string.IsNullOrEmpty(name)) return null;

            Actor actor = currentLocalDB.GetActor(name);
            if (actor != null) return actor;

            StoryFlowCharacterAsset character = null;
            if (project.Characters != null)
            {
                project.Characters.TryGetValue("characters/" + name.ToLowerInvariant() + ".sfc", out character);
            }
            if (character == null)
            {
                Debug.LogWarning("[StoryFlow Converter] Could not resolve StoryFlow character '" + characterKey + "' (resolved name '" + name + "').");
                return null;
            }

            actor = CreateActorFromCharacter(character, name, currentLocalDB);
            currentLocalDB.actors.Add(actor);
            return actor;
        }

        private Variable FindDatabaseVariable(DialogueDatabase database, string name)
        {
            if (database == null || string.IsNullOrEmpty(name)) return null;
            Variable variable = database.GetVariable(name);
            return variable;
        }

        private void EnsureBooleanVariable(DialogueDatabase database, string name, string initialValue, string conversationTitle)
        {
            Variable variable = FindDatabaseVariable(database, name);

            if (variable == null)
            {
                int varID = template.GetNextVariableID(database);
                variable = template.CreateVariable(varID, name, initialValue, FieldType.Boolean);

                database.variables.Add(variable);
            }

            variable.Description = conversationTitle;
        }

        #endregion

        #region Get Variable

        private string GetScriptVariableString(StoryFlowScriptAsset script, string variableName)
        {
            if (script.Variables == null) return null;
            foreach (StoryFlowVariable variable in script.Variables.Values)
            {
                if (variable.Name != variableName) continue;
                string key = variable.Value.StringValue;
                return ResolveScriptString(script, key);
            }
            return null;
        }

        private string GetScriptVariableData(StoryFlowScriptAsset script, string dataName, string variableName)
        {
            if (script.Variables == null) return null;
            foreach (StoryFlowVariable variable in script.Variables.Values)
            {
                if (variable.Name != dataName) continue;
                string dataKey = variable.Value.StringValue;

                string value = GetDataVariableValue(script, dataKey, variableName);
                value = value.Replace("\\n", "");
                return value.Replace("\"", "");
            }
            return null;
        }


        private string GetDataVariableValue(StoryFlowScriptAsset script, string dataKey, string variableName)
        {
            StoryFlowDataAssetAsset currentData = project.GetDataAsset(dataKey);

            if (currentData == null)
                return null;

            // 1. Check the current data asset.
            StoryFlowVariable currentVariable = FindVariable(currentData, variableName);

            if (currentVariable != null)
                return GetVariableValue(currentVariable, out FieldType type);

            // 2. Walk up the parent hierarchy until the variable is found.
            StoryFlowDataAssetAsset parentData = project.GetDataAsset(currentData.ParentId);

            while (parentData != null)
            {
                StoryFlowVariable parentVariable = FindVariable(parentData, variableName);

                if (parentVariable != null)
                {
                    // 3. The variable was defined in a parent.
                    // Check overrides starting at the current data asset.
                    StoryFlowDataAssetAsset childData = currentData;

                    while (childData != null && childData != parentData)
                    {
                        foreach (StoryFlowDataAssetAsset.OverrideEntry overrideEntry in childData.Overrides)
                        {
                            if (overrideEntry.VariableId == parentVariable.Id)
                                return overrideEntry.ValueJson;
                        }

                        childData = project.GetDataAsset(childData.ParentId);
                    }

                    // No override exists; use the parent's default value.
                    return GetVariableValue(parentVariable, out FieldType type);
                }

                parentData = project.GetDataAsset(parentData.ParentId);
            }

            return null;
        }

        private StoryFlowVariable FindVariable(StoryFlowDataAssetAsset data, string variableName)
        {
            if (data?.Variables == null)
                return null;

            foreach (StoryFlowVariable variable in data.Variables)
            {
                if (variable.Name == variableName)
                    return variable;
            }

            return null;
        }

        #endregion

        #region String Methods


        private string ResolveScriptString(StoryFlowScriptAsset script, string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            string fullKey = key.StartsWith(LanguageCode, StringComparison.Ordinal) ? key : LanguageCode + key;
            string value;
            if (script != null && script.Strings != null && script.Strings.TryGetValue(fullKey, out value))
                return value ?? string.Empty;
            // Some exports already contain literal text rather than a string-table key.
            return key;
        }

        private string ResolveGlobalString(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            string fullKey = key.StartsWith(LanguageCode, StringComparison.Ordinal) ? key : LanguageCode + key;
            string value = project.GetGlobalString(fullKey);
            return string.IsNullOrEmpty(value) ? key : value;
        }

        #endregion



        #region Debugging

        private void WarnUnsupportedNode(StoryFlowNode node)
        {
            string type = node.Type.ToString();
            if (warnedNodeTypes.Add(type))
                Debug.LogWarning("[StoryFlow Converter] StoryFlow node type '" + type +
                    "' is represented as a pass-through Dialogue System entry. Its runtime logic is not automatically translated.");
        }

        private void LogDatabase(string name, DialogueDatabase database)
        {
            if (database == null) return;
            Debug.Log(string.Format("{0}: Created database '{1}' containing {2} actors, {3} conversations, {4} items/quests, {5} variables, and {6} locations.",
                DialogueDebug.Prefix, name, database.actors.Count, database.conversations.Count,
                database.items.Count, database.variables.Count, database.locations.Count), database);
        }

        #endregion
    }
}
