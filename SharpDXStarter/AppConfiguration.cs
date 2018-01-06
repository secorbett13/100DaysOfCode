namespace SharpDXStarter
{
	/// <summary>
	/// The app configuration.
	/// </summary>
	public class AppConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppConfiguration"/> class.
		/// </summary>
		public AppConfiguration()
			: this("A demo app. Don't forget the title", 800, 600, false)
		{
		}
		
		/// <summary>
				/// Initializes a new instance of the <see cref="AppConfiguration"/> class.
				/// </summary>
				/// <param name="title">
				/// The title of the window created with the configuration.
				/// </param>
		public AppConfiguration(string title)
			: this(title, 800, 600, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppConfiguration"/> class.
		/// </summary>
		/// <param name="title">
		/// The title of the window created with the configuration.
		/// </param>
		/// <param name="width">
		/// The width of the window created with the configuration.
		/// </param>
		/// <param name="height">
		/// The height of the window created with the configuration.
		/// </param>
		public AppConfiguration(string title, int width, int height)
			: this(title, width, height, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppConfiguration"/> class.
		/// </summary>
		/// <param name="title">
		/// The title of the window created with the configuration.
		/// </param>
		/// <param name="width">
		/// The width of the window created with the configuration.
		/// </param>
		/// <param name="height">
		/// The height of the window created with the configuration.
		/// </param>
		/// <param name="waitVerticalBlanking">
		/// A flag indicating whether vertical blanking should be considered during rendering
		/// </param>
		public AppConfiguration(string title, int width, int height, bool waitVerticalBlanking)
		{
			this.Title = title;
			this.Width = width;
			this.Height = height;
			this.WaitVerticalBlanking = waitVerticalBlanking;
		}

		/// <summary>
		/// Gets the title of the window created using the provided configuration.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Gets the width of the window created with the configuration.
		/// </summary>
		public int Width { get; }

		/// <summary>
		/// Gets the height window created with the configuration .
		/// </summary>
		public int Height { get; }

		/// <summary>
		/// Gets a value indicating whether the hardware should wait for a vertical blank before continuing to render.
		/// </summary>
		/// <remarks>
		/// This should almost always be false
		/// </remarks>
		public bool WaitVerticalBlanking { get; }
	}
}
