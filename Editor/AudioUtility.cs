// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Depra.EdenAI.Export.Editor
{
	internal static class AudioUtility
	{
		public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
		{
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType($"{nameof(UnityEditor)}.AudioUtil");
			var method = audioUtilClass.GetMethod(
				name: "PlayPreviewClip",
				bindingAttr: BindingFlags.Static | BindingFlags.Public,
				binder: null,
				types: new[] { typeof(AudioClip), typeof(int), typeof(bool) },
				modifiers: null
			);

			method!.Invoke(
				obj: null,
				parameters: new object[] { clip, startSample, loop }
			);
		}
	}
}