// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Depra.EdenAI.Export.Editor.Module;

namespace Depra.EdenAI.Export.Editor
{
	internal sealed class EdenSpeechGenerationWindow : EditorWindow
	{
		private const string FILE_DIRECTORY = "Assets/Resources/";
		private const string FILE_NAME = nameof(EdenSpeechGenerator) + ".asset";

		[MenuItem(MENU_NAME + "Text to Speech")]
		public static void Open()
		{
			if (_window != null)
			{
				_window.Show();
				return;
			}

			_window = GetWindow<EdenSpeechGenerationWindow>("Eden/Text to Speech");
			_window.Show();
		}

		private static EdenSpeechGenerator _instance;
		private static EdenSpeechGenerationWindow _window;

		private static EdenSpeechGenerator Instance =>
			_instance ? _instance : TryLoadAsset(out var instance) ? instance : CreateAsset();

		private void CreateGUI()
		{
			rootVisualElement.Clear();
			rootVisualElement.Add(new Label("Text to speech:"));

			var editor = UnityEditor.Editor.CreateEditor(Instance);
			var editorRootElement = editor.CreateInspectorGUI();
			editorRootElement.Bind(editor.serializedObject);

			rootVisualElement.Add(editorRootElement);
		}

		private static bool TryLoadAsset(out EdenSpeechGenerator instance)
		{
			instance = Resources.Load<EdenSpeechGenerator>(nameof(EdenSpeechGenerator));
			return instance != null;
		}

		private static EdenSpeechGenerator CreateAsset()
		{
			var instance = CreateInstance<EdenSpeechGenerator>();

			if (Directory.Exists(FILE_DIRECTORY) == false)
			{
				Directory.CreateDirectory(FILE_DIRECTORY);
			}

			AssetDatabase.CreateAsset(instance, FILE_DIRECTORY + FILE_NAME);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return instance;
		}
	}
}