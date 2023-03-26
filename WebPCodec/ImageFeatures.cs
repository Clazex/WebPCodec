using System.Runtime.InteropServices;

namespace WebPCodec;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct ImageFeatures {
	public readonly int width;
	public readonly int height;
	[MarshalAs(UnmanagedType.U1)]
	public readonly bool hasAnimation;
}
