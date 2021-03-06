﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Windows;

namespace SharpDXJohnFalkTutorial
{
	using D3D11 = SharpDX.Direct3D11;

	public class Game : IDisposable
	{
		private RenderForm renderForm;

		// Clock for managing time within the class
		private Stopwatch clock;

		private const int Width = 1280;
		private const int Height = 720;

		private D3D11.Device d3dDevice;
		private D3D11.DeviceContext d3dDeviceContext;
		private SwapChain swapChain;

		private D3D11.RenderTargetView renderTargetView;

		private D3D11.VertexShader vertexShader;
		private D3D11.PixelShader pixelShader;

		private VertexPositionColor[] vertices = new VertexPositionColor[]
		{
			new VertexPositionColor(new Vector3(-0.25f, 0.25f, 0.0f), SharpDX.Color.Red),
			new VertexPositionColor(new Vector3(0.25f, 0.25f, 0.0f), SharpDX.Color.Green),
			new VertexPositionColor(new Vector3(0.0f, -0.25f, 0.0f), SharpDX.Color.Blue)
		};

		private D3D11.Buffer triangleVertexBuffer;

		private D3D11.InputElement[] inputElements = new D3D11.InputElement[]
		{
			new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, D3D11.InputClassification.PerVertexData, 0),
			new D3D11.InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0, D3D11.InputClassification.PerVertexData, 0)
		};

		private ShaderSignature inputSignature;
		private D3D11.InputLayout inputLayout;

		private Viewport viewport;

		/// <summary>
		/// Constructor for the <see cref="Game"/> class
		/// </summary>
		public Game()
		{
			clock = Stopwatch.StartNew();

			renderForm = new RenderForm("My First SharpDX App")
			{
				ClientSize = new Size(Width, Height),

				AllowUserResizing = false
			};

			InitializeDeviceResources();
			InitializeShaders();
			InitializeTriangle();
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
			inputLayout.Dispose();
			inputSignature.Dispose();
			triangleVertexBuffer.Dispose();
			vertexShader.Dispose();
			pixelShader.Dispose();
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

			// Create device and swap chain
			D3D11.Device.CreateWithSwapChain(
				DriverType.Hardware,
				D3D11.DeviceCreationFlags.None,
				swapChainDesc,
				out d3dDevice,
				out swapChain);
			d3dDeviceContext = d3dDevice.ImmediateContext;

			// set viewport
			viewport = new Viewport(0, 0, Width, Height);
			d3dDeviceContext.Rasterizer.SetViewport(viewport);

			using (D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0))
			{
				renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
			}
		}

		private void InitializeShaders()
		{
			// Compile the vertex shader code
			using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile("vertexShader.hlsl", "main", "vs_4_0", ShaderFlags.Debug))
			{
				// Read input signature from shader code
				inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);

				vertexShader = new D3D11.VertexShader(d3dDevice, vertexShaderByteCode);
			}

			// Compile the pixel shader code
			using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile("pixelShader.hlsl", "main", "ps_4_0", ShaderFlags.Debug))
			{
				pixelShader = new D3D11.PixelShader(d3dDevice, pixelShaderByteCode);
			}

			// set as the current vertex and pixel shaders
			d3dDeviceContext.VertexShader.Set(vertexShader);
			d3dDeviceContext.PixelShader.Set(pixelShader);

			d3dDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

			// Create the input layout from the inputsignature and the input elements
			inputLayout = new D3D11.InputLayout(d3dDevice, inputSignature, inputElements);

			// Set the input layout to use
			d3dDeviceContext.InputAssembler.InputLayout = inputLayout;
		}

		private void InitializeTriangle()
		{
			triangleVertexBuffer = D3D11.Buffer.Create(d3dDevice, D3D11.BindFlags.VertexBuffer, vertices);
		}

		private void Draw()
		{
			// Set the back buffere as the current render target view
			d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);

			// Clear the screen and set the background color
			d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(32, 103, 178));

			var time = clock.Elapsed.TotalSeconds;
			var offset = (float)Math.Sin(time);

			// translate the vertices up and down
			vertices = new VertexPositionColor[]
					{
						new VertexPositionColor(new Vector3(-0.25f, 0.25f + offset, 0.0f), SharpDX.Color.Red),
						new VertexPositionColor(new Vector3(0.25f, 0.25f + offset, 0.0f), SharpDX.Color.Green),
						new VertexPositionColor(new Vector3(0.0f, -0.25f + offset, 0.0f), SharpDX.Color.Blue)
					};
			triangleVertexBuffer = D3D11.Buffer.Create(d3dDevice, D3D11.BindFlags.VertexBuffer, vertices);

			// Set vertex buffere
			d3dDeviceContext.InputAssembler.SetVertexBuffers(
				0, 
				new D3D11.VertexBufferBinding(triangleVertexBuffer, Utilities.SizeOf<VertexPositionColor>(), 0));

			// Draw the triangle
			d3dDeviceContext.Draw(vertices.Count(), 0);

			// Swap the front and back buffer
			swapChain.Present(1, PresentFlags.None);
		}
	}
}
