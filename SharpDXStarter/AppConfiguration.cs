// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfiguration.cs" company="N/A">
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//   Copyright (c) 2018 Scott E. Corbett.
// </copyright>
// <summary>
//   The app configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SharpDXStarter
{
	/// <summary>
	/// The app configuration.
	/// </summary>
	public class AppConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:SharpDXStarter.AppConfiguration" /> class.
		/// </summary>
		public AppConfiguration()
			: this("A demo app. Don't forget the title", 800, 600, false)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:SharpDXStarter.AppConfiguration" /> class.
		/// </summary>
		/// <param name="title">
		/// The title of the window created with the configuration.
		/// </param>
		public AppConfiguration(string title)
			: this(title, 800, 600, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SharpDXStarter.AppConfiguration" /> class.
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
