using System;
using System.IO;
using System.Runtime.InteropServices;

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using UnityEngine;

namespace WebPCodec;

// Contains mid-level code that translates low-level error into C# exceptions,
// and transmits data between C# side and Rust side.
internal static class Interop {
	internal const int WEBP_MAX_DIMENSION = 16383;

	// Restricts to 2 GiB
	internal const long MAX_PIXELS = 2L * 1024 * 1024 * 1024 / (4 * 8);

	// Inspect the WebP image in the array and return its dimension.
	public static (int width, int height) InspectImage(byte[] image) {
		ImageFeatures features;

		var hImage = GCHandle.Alloc(image, GCHandleType.Pinned);
		IntPtr pImage = hImage.AddrOfPinnedObject();

		try {
			features = Native.InspectImage(pImage, image.Length);
		} catch (Exception e) when (e is not DllNotFoundException or EntryPointNotFoundException) {
			throw new InvalidDataException("Invalid WebP image", e);
		} finally {
			hImage.Free();
		}

		if (features.width <= 0 || features.height <= 0) {
			throw new InvalidDataException("Invalid WebP image");
		}

		if (features.hasAnimation) {
			throw new NotSupportedException("Animation is not supported");
		}

		// Limits dimension to the same as libwebp's WebPPicture.
		if (features.width > WEBP_MAX_DIMENSION || features.height > WEBP_MAX_DIMENSION) {
			throw new NotSupportedException("Image dimension too large");
		}

		if ((long) features.width * features.height > MAX_PIXELS) {
			throw new NotSupportedException("Image size too large");
		}

		return (features.width, features.height);
	}

	// Decode the WebP image in the array directly into the texture.
	public static void DecodeImage(byte[] image, Texture2D target) {
		var hImage = GCHandle.Alloc(image, GCHandleType.Pinned);
		IntPtr pImage = hImage.AddrOfPinnedObject();

		NativeArray<Color32> buffer = target.GetRawTextureData<Color32>();

		bool success = false;

		try {
			unsafe {
				success = Native.DecodeImage(pImage, image.Length, buffer.GetUnsafePtr());
			}
		} catch (Exception e) when (e is not DllNotFoundException or EntryPointNotFoundException) {
			throw new InvalidDataException("Failed to decode WebP image", e);
		} finally {
			hImage.Free();
		}

		if (!success) {
			throw new InvalidDataException("Failed to decode WebP image");
		}
	}
}
