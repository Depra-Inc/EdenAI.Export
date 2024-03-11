// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Text.RegularExpressions;

namespace Depra.EdenAI.Export.Editor
{
	internal static class StringExtensions
	{
		public static string RemoveSpecialCharacters(this string self, string replacement)
		{
			const string PATTERN = "[^a-zA-Z0-9]";
			return Regex.Replace(self, PATTERN, replacement);
		}
	}
}