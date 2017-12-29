using SharpDX.Windows;
using System;

namespace SharpDXStarter
{
	/// <summary>
	/// Root program for the application. Everything starts here. :)
	/// </summary>
	class Program
	{
		/// <summary>
		/// Main entry point for the application
		/// </summary>
		/// <param name="args"></param>
		[STAThread]
		static void Main(string[] args)
		{
			var thisForm = new RenderForm("HelloWorld#D12");

			using (var helloTriangle = new HelloWorld())
			{
				thisForm.Show();
				helloTriangle.Initialize(thisForm);

				using (var loop = new RenderLoop(thisForm))
				{
					while(loop.NextFrame())
					{
						helloTriangle.Update();
						helloTriangle.Render();
					}
				}
			}
		}
	}
}
