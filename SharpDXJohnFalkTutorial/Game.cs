using System;
using System.Drawing;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;

namespace SharpDXJohnFalkTutorial
{
	using D3D11 = SharpDX.Direct3D11;

	public class Game : IDisposable
	{
		private RenderForm renderForm;

		private const int Width = 1280;
		private const int Height = 720;

		private D3D11.Device d3dDevice;
		private D3D11.DeviceContext d3dDeviceContext;
		private SwapChain swapChain;

		private D3D11.RenderTargetView renderTargetView;

		/// <summary>
		/// Constructor for the <see cref="Game"/> class
		/// </summary>
		public Game()
		{
			renderForm = new RenderForm("My First SharpDX App");
			renderForm.ClientSize = new Size(Width, Height);

			renderForm.AllowUserResizing = false;

			InitializeDeviceResources();
		}

		/// <summary>
		/// Starts the rendering loop for the application
		/// </summary>
		public void Run()
		{
			RenderLoop.Run(renderForm, RenderCallBack);
		}

		/// <summary>
		/// Dispose method to clean up any assets/resources allocated during the class instance's lifetime
		/// </summary>
		public void Dispose()
		{
			renderTargetView.Dispose();
			swapChain.Dispose();
			d3dDevice.Dispose();
			d3dDeviceContext.Dispose();
			renderForm.Dispose();
		}

		private void RenderCallBack()
		{
			Draw();
		}

		private void InitializeDeviceResources()
		{
			ModeDescription backBufferDesc = new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);

			SwapChainDescription swapChainDesc = new SwapChainDescription
			{
				ModeDescription = backBufferDesc,
				SampleDescription = new SampleDescription(1, 0),
				Usage = Usage.RenderTargetOutput,
				BufferCount = 1,
				OutputHandle = renderForm.Handle,
				IsWindowed = true
			};

			D3D11.Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware,
				D3D11.DeviceCreationFlags.None,
				swapChainDesc,
				out d3dDevice,
				out swapChain);
			d3dDeviceContext = d3dDevice.ImmediateContext;

			using (D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0))
			{
				renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
			}
		}

		private void Draw()
		{
			d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
			d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(32, 103, 178));
			swapChain.Present(1, PresentFlags.None);
		}
	}
}
