/*

*/
using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;

namespace UnityEditor
{

    [Serializable]
    public class Variables
    {
        public Color textColor = Color.white;
        public string path;
        public bool setting;
        public string text;
        public int TextSize = 13;
        public Vector2 scrool = Vector2.zero;
        public bool save;
        public string info;
    }


    public class UnityTextEditor : EditorWindow
    {


        [SerializeField] private const string nulls = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n";

        public Variables variables;
        public static UnityTextEditor editor { get; set; }

        [MenuItem("Window/Unity Text Editor &T")]
        public static void ShowEditor()
        {
            GetWindow(typeof(UnityTextEditor), false, "Unity Text Editor", true);
        }

        [MenuItem("Assets/Open to Unity Text Editor")]
        public static void Open()
        {
            if (File.Exists(AssetDatabase.GetAssetPath(Selection.activeObject)))
            {
               
                GetWindow(typeof(UnityTextEditor), false, " Unity Text Editor", true);
                editor.variables.text = File.ReadAllText(AssetDatabase.GetAssetPath(Selection.activeObject));
                editor.variables.path = AssetDatabase.GetAssetPath(Selection.activeObject);
            }
            else
            {
                EditorUtility.DisplayDialog("Warrning", "No file selected!", "OK");
            }
        }


        [MenuItem("Assets/Open to Notepad")]
        public static void OpenNotepad()
        {
            if (File.Exists(AssetDatabase.GetAssetPath(Selection.activeObject)))
            {

                Process.Start("notepad.exe", "/a " + AssetDatabase.GetAssetPath(Selection.activeObject));

            }
            else
            {
                EditorUtility.DisplayDialog("Warrning", "No file selected!", "OK");
            }
        }

        private void LoadingVariables()
        {
            EditorJsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("TextEditor"), variables);
        }

        private void OnEnable()
        {
            variables = new Variables();
            editor = this;

            if (PlayerPrefs.HasKey("TextEditor"))
            {
                LoadingVariables();
            }
            else
            {
                variables.text = nulls;
            }

        }

        private void OnDisable()
        {
            PlayerPrefs.SetString("TextEditor", EditorJsonUtility.ToJson(variables));
            variables.text = "";
        }

        private void Referesh()
        {
            AssetDatabase.Refresh();
            AssetDatabase.RefreshSettings();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Open File"))
            {
                variables.path = EditorUtility.OpenFilePanel("Open File", "Assets/", "cs, txt, json, shader");

                if (variables.path.Length != 0)
                {

                    PlayerPrefs.SetString("TextEditor", EditorJsonUtility.ToJson(variables));
                    variables.text = File.ReadAllText(variables.path);
                    variables.info = "Loaded form:" + variables.path;
                    variables.save = true;

                }
            }

            if (variables.save)
            {
                if (GUILayout.Button("Save File"))
                {

                    if (EditorUtility.DisplayDialog("Warrning", "Want to overwrite a file?", "OK", "Canel"))
                    {
                        PlayerPrefs.SetString("TextEditor", EditorJsonUtility.ToJson(variables));
                        variables.info = "The file has been overwritten!";
                        File.WriteAllText(variables.path, variables.text);
                        Referesh();
                    }

                }

            }

            if (GUILayout.Button("Save File As.."))
            {

                variables.path = EditorUtility.SaveFilePanel("Save File", "Assets/", "", "cs, txt, json, shader");

                if (variables.path.Length != 0)
                {
                    PlayerPrefs.SetString("TextEditor", EditorJsonUtility.ToJson(variables));
                    File.WriteAllText(variables.path, variables.text);
                    Referesh();
                    variables.info = "Save to:" + variables.path;
                    variables.save = true;
                }
            }

            if (GUILayout.Button("New"))
            {
                PlayerPrefs.DeleteAll();
                variables.path = "";
                variables.text = nulls;
                variables.save = false;
            }

            GUILayout.EndHorizontal();

            variables.scrool = GUILayout.BeginScrollView(variables.scrool);
            GUI.skin.textArea.fontSize = variables.TextSize;
            GUI.contentColor = variables.textColor;
            variables.text = GUILayout.TextArea(variables.text);
            GUILayout.EndScrollView();
            GUI.contentColor = Color.white;
            EditorGUILayout.HelpBox(variables.info, MessageType.Info);

            if (variables.setting = GUILayout.Toggle(variables.setting, "Setting Editor"))
            {

                GUI.contentColor = Color.white;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Size Text:");
                variables.TextSize = EditorGUILayout.IntSlider(variables.TextSize, 10, 20);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Color Text");
                variables.textColor = EditorGUILayout.ColorField(variables.textColor);
                GUILayout.EndHorizontal();
            }
        }
    }
}
