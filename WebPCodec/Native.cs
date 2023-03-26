using System;
using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]

namespace WebPCodec;

// Contains low-level methods directly imported from Rust side
internal static class Native {
	private const string LIB_NAME = "webp_codec_native";

	// Get the features of the WebP image in the array. Content will be all
	// zero if the array does not contain a valid  WebP image.
	[DllImport(LIB_NAME, EntryPoint = "inspect_image", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ImageFeatures InspectImage(IntPtr data, int dataSize);

	// Decode the WebP image specified by data and dataSize into outputBuffer.
	// It is expected that the size of outputBuffer fit the need exactly.
	[DllImport(LIB_NAME, EntryPoint = "decode_image", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static extern unsafe bool DecodeImage(IntPtr data, int dataSize, void* outputBuffer);
}
