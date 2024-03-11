// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.EdenAI.Export.Editor
{
	[CustomEditor(typeof(EdenSpeechGenerator), true)]
	internal sealed class EdenSpeechGeneratorEditor : UnityEditor.Editor
	{
		private const string DOCUMENTATION_URL = "https://docs.edenai.co/reference/audio_text_to_speech_create";
		private const string UNITY_DOCUMENTATION_URL = "https://github.com/edenai/unity-plugin/blob/master/README.md";

		private AudioClip _lastClip;
		private ObjectField _clipField;
		private VisualElement _rootElement;
		private EdenSpeechGenerator _generator;

		public override VisualElement CreateInspectorGUI()
		{
			var root = new VisualElement();
			root.Add(new Label($"<a href=\"{DOCUMENTATION_URL}\">Documentation</a>"));
			root.Add(new Label($"<a href=\"{UNITY_DOCUMENTATION_URL}\">Unity Documentation</a>"));

			var iterator = serializedObject.GetIterator();
			iterator.NextVisible(enterChildren: true);
			root.Add(new PropertyField(iterator.GetEndProperty()));

			while (iterator.NextVisible(enterChildren: false))
			{
				root.Add(new PropertyField(iterator.GetEndProperty()));
			}

			_generator = (EdenSpeechGenerator) target;
			var generateButton = new Button(Request) { text = nameof(Request) };
			root.Add(generateButton);
			_clipField = new ObjectField("Last result: ") { objectType = typeof(AudioClip) };
			root.Add(_clipField);
			var playButton = new Button(Play) { text = nameof(Play) };
			root.Add(playButton);
			var exportButton = new Button(Export) { text = nameof(Export) };
			root.Add(exportButton);

			return root;
		}

		private async void Request()
		{
			Debug.Log($"[{nameof(EdenAI.Export)}] Text to speech request: {_generator.Text}");

			var response = await _generator.Request();
			if (response.status == "fail")
			{
				throw new ExternalException();
			}

			_lastClip = response.audio;
			_lastClip.name = GetFileName(_generator.Text);
			_clipField.value = _lastClip;

			Debug.Log($"[{nameof(EdenAI.Export)}] Text to speech status: {response.status}" +
			          $"{nameof(response.cost)}: {response.cost};{Environment.NewLine}" +
			          $"{nameof(response.voice_type)}: {response.voice_type};{Environment.NewLine}" +
			          $"{nameof(response.provider)}: {response.provider};{Environment.NewLine}" +
			          $"{nameof(response.audio.length)}: {response.audio.length};{Environment.NewLine}" +
			          $"{nameof(response.audio_base64)}: {response.audio_base64}");
		}

		private void Play()
		{
			if (_lastClip != null)
			{
				AudioUtility.PlayClip(_lastClip);
			}
		}

		private void Export()
		{
			if (_lastClip == null)
			{
				return;
			}

			var path = EditorUtility.SaveFilePanel("Select directory",
				directory: Application.dataPath,
				defaultName: _lastClip.name,
				extension: WavAudioFile.EXTENSION);

			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			WavAudioFile.Export(_lastClip, path);
			Debug.Log($"[{nameof(EdenAI.Export)}] File exported: {path}");
		}

		private static string GetFileName(string text)
		{
			const int MAX_LENGTH = 20;
			var fileName = text.RemoveSpecialCharacters("_");
			var length = fileName.Length > MAX_LENGTH ? MAX_LENGTH : fileName.Length;
			fileName = fileName[..length];

			return fileName;
		}
	}
}