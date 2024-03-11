// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading.Tasks;
using EdenAI;
using UnityEngine;

namespace Depra.EdenAI.Export
{
	internal sealed class EdenSpeechGenerator : ScriptableObject
	{
		[TextArea] [SerializeField] private string _apiKey;
		[TextArea] [SerializeField] private string _text = "Hello how are you?";
		[SerializeField] private VoiceProperties _voiceProperties;
		[SerializeField] private SoundProperties _soundProperties;

		private EdenAIApi _api;

		public string Text => _text;
		private EdenAIApi Api => _api ??= new EdenAIApi(_apiKey);

		public async Task<TextToSpeechResponse> Request() => await Api.SendTextToSpeechRequest(text: _text,
			option: _voiceProperties.Option,
			provider: _voiceProperties.Provider,
			language: _voiceProperties.Language,
			voiceModel: _voiceProperties.Model,
			rate: _soundProperties.Rate,
			pitch: _soundProperties.Pitch,
			volume: _soundProperties.Volume,
			audioFormat: _soundProperties.Format);

		[Serializable]
		public sealed class VoiceProperties
		{
			[SerializeField] private string _language = "en";
			[SerializeField] private string _model = "en-US_Justin_Standard";
			[SerializeField] private string _provider = "amazon,microsoft,google";
			[SerializeField] private TextToSpeechOption _option = TextToSpeechOption.MALE;

			public string Model => _model;
			public string Language => _language;
			public string Provider => _provider;
			public TextToSpeechOption Option => _option;
		}

		[Serializable]
		private sealed class SoundProperties
		{
			[SerializeField] private int _rate;
			[SerializeField] private int _pitch;
			[Min(0)] [SerializeField] private int _volume = 1;

			[Tooltip("mp3 or wav")] [SerializeField]
			private string _format = "mp3";

			public int Rate => _rate;
			public int Pitch => _pitch;
			public int Volume => _volume;
			public string Format => _format;
		}
	}
}