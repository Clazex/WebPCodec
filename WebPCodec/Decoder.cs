using System;
using System.IO;

using UnityEngine;

namespace WebPCodec;

/// <summary>
/// Contains methods for decoding WebP images.
/// </summary>
public static class Decoder {
	/// <inheritdoc cref="CreateWebPTexture(byte[], bool)" />
	public static Texture2D CreateWebPTexture(byte[] data) =>
		CreateWebPTexture(data, false);

	/// <summary>
	/// Create a <see cref="Texture2D"/> from the WebP image in
	/// <paramref name="data"/>.
	/// </summary>
	/// <param name="data">Array that contains image data</param>
	/// <param name="markNonReadable">
	/// Whether to mark the created <see cref="Texture2D"/> as non-readable
	/// </param>
	/// <returns>Created <see cref="Texture2D"/></returns>
	/// <exception cref="InvalidDataException">
	/// Throws when libwebp fails to inpect image data
	/// </exception>
	/// <exception cref="NotSupportedException">
	/// Throws when image contains animation
	/// </exception>
	/// <exception cref="DllNotFoundException">
	/// Throws when native DLL is not found
	/// </exception>
	/// <exception cref="EntryPointNotFoundException">
	/// Throws when using incompatible version of native DLL
	/// </exception>
	public static Texture2D CreateWebPTexture(byte[] data, bool markNonReadable) {
		(int width, int height) = Interop.InspectImage(data);

		Texture2D tex = new(width, height);

		Interop.DecodeImage(data, tex);
		tex.Apply(true, markNonReadable);

		return tex;
	}

	/// <inheritdoc cref="LoadWebPImage(Texture2D, byte[], bool)"/>
	public static void LoadWebPImage(this Texture2D self, byte[] data) =>
		self.LoadWebPImage(data, false);

	/// <summary>
	/// <para>
	/// Load the WebP image in <paramref name="data"/> into an existing
	/// <see cref="Texture2D"/>. Prefer using
	/// <see cref="CreateWebPTexture(byte[])"/>.
	/// </para>
	/// <para>
	/// It's caller's responsibility to ensure neither
	/// <paramref name="self"/> nor <paramref name="data"/> are being written
	/// by another thread.
	/// </para>
	/// </summary>
	/// <param name="self">
	/// Target <see cref="Texture2D"/> to load the image into,
	/// it is expected to have RGBA32 format and no mipmap
	/// </param>
	/// <param name="data">Array that contains image data</param>
	/// <param name="markNonReadable">
	/// Whether to mark the created <see cref="Texture2D"/> as non-readable
	/// </param>
	/// <returns>Created <see cref="Texture2D"/></returns>
	/// <exception cref="InvalidDataException">
	/// Throws when libwebp fails to inpect image data
	/// </exception>
	/// <exception cref="NotSupportedException">
	/// Throws when image contains animation
	/// </exception>
	/// <exception cref="DllNotFoundException">
	/// Throws when native DLL is not found
	/// </exception>
	/// <exception cref="EntryPointNotFoundException">
	/// Throws when using incompatible version of native DLL
	/// </exception>
	public static void LoadWebPImage(this Texture2D self, byte[] data, bool markNonReadable) {
		if (self.format != TextureFormat.RGBA32) {
			throw new NotSupportedException(
				"Only supports loading into RGBA32 format textures"
			);
		}

		(int width, int height) = Interop.InspectImage(data);

		self.width = width;
		self.height = height;

		Interop.DecodeImage(data, self);
		self.Apply(true, markNonReadable);
	}


	/// <inheritdoc cref="LoadImage(Texture2D, byte[], bool)" />
	public static bool LoadImage(this Texture2D self, byte[] data) =>
		LoadImage(self, data, false);

	/// <summary>
	/// <para>
	/// Tries to load the image in <paramref name="data"/> as WebP, and if
	/// is is not, tries to load it with
	/// <see cref="ImageConversion.LoadImage(Texture2D, byte[], bool)"/>.
	/// </para>
	/// <para>
	/// It's caller's responsibility to ensure neither
	/// <paramref name="self"/> nor <paramref name="data"/> are being written
	/// by another thread.
	/// </para>
	/// </summary>
	/// <param name="self">
	/// Target <see cref="Texture2D"/> to load the image into, it is
	/// expected to have RGBA32 format and no mipmap if it is WebP
	/// </param>
	/// <param name="data">Array that contains image data</param>
	/// <param name="markNonReadable">
	/// Whether to mark the created <see cref="Texture2D"/> as non-readable
	/// </param>
	/// <returns>Whether the operation is successful</returns>
	/// <exception cref="DllNotFoundException">
	/// Throws when native DLL is not found
	/// </exception>
	/// <exception cref="EntryPointNotFoundException">
	/// Throws when using incompatible version of native DLL
	/// </exception>
	public static bool LoadImage(this Texture2D self, byte[] data, bool markNonReadable) {
		try {
			LoadWebPImage(self, data, markNonReadable);
			return true;
		} catch (Exception e) when (e is InvalidDataException) {
			return ImageConversion.LoadImage(self, data, markNonReadable);
		}
	}
}
