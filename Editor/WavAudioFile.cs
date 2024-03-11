// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.IO;
using System.Text;
using UnityEngine;

internal static class WavAudioFile
{
	public const string EXTENSION = "wav";

	private const int HEADER_SIZE = 44;
	private const string FORMAT_EXTENSION = "." + EXTENSION;

	public static void Export(AudioClip clip, string filePath)
	{
		if (filePath.ToLower().EndsWith(FORMAT_EXTENSION) == false)
		{
			filePath += FORMAT_EXTENSION;
		}

		var fileDirectory = Path.GetDirectoryName(filePath);
		if (Directory.Exists(fileDirectory) == false)
		{
			Directory.CreateDirectory(fileDirectory!);
		}

		using var fileStream = CreateEmpty(filePath);
		ConvertAndWrite(fileStream, clip);
		WriteHeader(fileStream, clip);
	}

	private static FileStream CreateEmpty(string filePath)
	{
		const byte EMPTY_BYTE = new();
		var fileStream = new FileStream(filePath, FileMode.Create);

		// Prepare header:
		for (var index = 0; index < HEADER_SIZE; index++)
		{
			fileStream.WriteByte(EMPTY_BYTE);
		}

		return fileStream;
	}

	private static void ConvertAndWrite(Stream fileStream, AudioClip clip)
	{
		var samples = new float[clip.samples];
		clip.GetData(samples, 0);

		var intData = new short[samples.Length];
		var bytesData = new byte[samples.Length * 2];
		// Because a float converted in Int16 is 2 bytes.

		// Convert float to Int16:
		const float RESCALE_FACTOR = 32767;
		for (var index = 0; index < samples.Length; index++)
		{
			intData[index] = (short) (samples[index] * RESCALE_FACTOR);
			var bytes = BitConverter.GetBytes(intData[index]);
			bytes.CopyTo(bytesData, index * 2);
		}

		fileStream.Seek(0, SeekOrigin.Begin);
		fileStream.Write(bytesData, 0, bytesData.Length);
	}

	private static void WriteHeader(Stream stream, AudioClip clip)
	{
		var samples = clip.samples;
		var channels = clip.channels;
		var frequency = clip.frequency;

		stream.Seek(0, SeekOrigin.Begin);

		var riff = Encoding.UTF8.GetBytes("RIFF");
		stream.Write(riff, 0, 4);

		var chunkSize = BitConverter.GetBytes(stream.Length - 8);
		stream.Write(chunkSize, 0, 4);

		var wave = Encoding.ASCII.GetBytes("WAVE");
		stream.Write(wave, 0, 4);

		var fmt = Encoding.ASCII.GetBytes("fmt ");
		stream.Write(fmt, 0, 4);

		var subChunk1 = BitConverter.GetBytes(16);
		stream.Write(subChunk1, 0, 4);

		var audioFormat = BitConverter.GetBytes(1);
		stream.Write(audioFormat, 0, 2);

		var numChannels = BitConverter.GetBytes(channels);
		stream.Write(numChannels, 0, 2);

		var sampleRate = BitConverter.GetBytes(frequency);
		stream.Write(sampleRate, 0, 4);

		var byteRate = BitConverter.GetBytes(frequency * channels * 2);
		stream.Write(byteRate, 0, 4);

		var blockAlign = (ushort) (channels * 2);
		stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

		var bitsPerSample = BitConverter.GetBytes(16);
		stream.Write(bitsPerSample, 0, 2);

		var dataString = Encoding.UTF8.GetBytes("data");
		stream.Write(dataString, 0, 4);

		var subChunk2 = BitConverter.GetBytes(samples * channels * 2);
		stream.Write(subChunk2, 0, 4);

		stream.Seek(0, SeekOrigin.Begin);
	}
}