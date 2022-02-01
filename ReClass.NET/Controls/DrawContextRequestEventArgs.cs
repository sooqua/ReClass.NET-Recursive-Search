using System;
using ReClassNET.Memory;
using ReClassNET.Nodes;
using ReClassNET.UI;

namespace ReClassNET.Controls
{
	public class DrawContextRequestEventArgs : EventArgs
	{
		public DateTime CurrentTime { get; set; } = DateTime.UtcNow;

		public Settings Settings { get; set; }

		public IconProvider IconProvider { get; set; }

		public RemoteProcess Process { get; set; }

		public MemoryBuffer Memory { get; set; }

		public BaseNode Node { get; set; }

		public IntPtr BaseAddress { get; set; }
	}

	public delegate void DrawContextRequestEventHandler(object sender, DrawContextRequestEventArgs args);
}
