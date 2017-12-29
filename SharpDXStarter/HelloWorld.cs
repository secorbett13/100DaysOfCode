using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.DXGI;

namespace SharpDXStarter
{
	using SharpDX.Direct3D12;

	public class HelloWorld : IDisposable
	{
		/// <summary>
		/// Number of buffers to be used for rendering
		/// </summary>
		private const int SwapBufferCount = 2;

		/// <summary>
		/// A Direct3D12 device (need to read up on these as they look like they're pretty important to DirectX.)
		/// </summary>
		private Device device;

		// Clock for managing time within the class
		private Stopwatch clock;

		/// <summary>
		/// The width of the form to be rendered
		/// </summary>
		private int width;

		/// <summary>
		/// The height of the form to be rendered.
		/// </summary>
		private int height;

		private SwapChain swapChain;

		private CommandQueue commandQueue;

		private GraphicsCommandList commandList;

		private Fence fence;

		private long currentFence;

		private int indexLastSwapBuf;

		private Resource renderTarget;

		private Rectangle scissorRectangle;

		private DescriptorHeap descriptorHeap;

		private CommandAllocator commandListAllocator;

		private ViewportF viewPort;

		private AutoResetEvent eventHandle;

		/// <summary>
		/// Constructor
		/// </summary>
		public HelloWorld()
		{
			clock = Stopwatch.StartNew();
		}

		/// <summary>
		/// Initialization this instance of <see cref="HelloWorld"/> using the provided <see cref="Form"/> for configuration information
		/// </summary>
		/// <param name="form"></param>
		public void Initialize(Form form)
		{
			width = form.ClientSize.Width;
			height = form.ClientSize.Height;

			LoadPipeline(form);
			LoadAssets();
		}

		/// <summary>
		/// Updates the instance. 
		/// </summary>
		public void Update()
		{
		}

		/// <summary>
		/// Render the top of the swap chain to the screen after executing the command list
		/// </summary>
		public void Render()
		{
			// record all of the commands we need to render the scene into the command list
			PopulateCommandList();

			// execute the command list
			commandQueue.ExecuteCommandList(commandList);

			// swap the back and front buffers
			swapChain.Present(1, 0);
			indexLastSwapBuf = (indexLastSwapBuf + 1) % SwapBufferCount;
			Utilities.Dispose(ref renderTarget);
			renderTarget = swapChain.GetBackBuffer<Resource>(indexLastSwapBuf);
			device.CreateRenderTargetView(renderTarget, null, descriptorHeap.CPUDescriptorHandleForHeapStart);

			// wait and reset EVERYTHING
			WaitForPrevFrame();
		}

		/// <summary>
		/// Cleanup allocations
		/// </summary>
		public void Dispose()
		{
			// wait for the GPU to be done with all resources
			WaitForPrevFrame();

			swapChain.SetFullscreenState(false, null);

			eventHandle.Close();

			// asset objects
			Utilities.Dispose(ref commandList);

			// pipeline objects
			Utilities.Dispose(ref descriptorHeap);
			Utilities.Dispose(ref renderTarget);
			Utilities.Dispose(ref commandListAllocator);
			Utilities.Dispose(ref commandQueue);
			Utilities.Dispose(ref device);
			Utilities.Dispose(ref swapChain);
		}


		/// <summary>
		/// Creates the rendering pipeline
		/// </summary>
		/// <param name="form"></param>
		private void LoadPipeline(Form form)
		{
			// create a swap chain descriptor
			var swapChainDescription = new SwapChainDescription()
			{
				BufferCount = SwapBufferCount,
				ModeDescription = new ModeDescription(Format.R8G8B8A8_UNorm),
				Usage = Usage.RenderTargetOutput,
				OutputHandle = form.Handle,
				SwapEffect = SwapEffect.FlipDiscard,
				SampleDescription = new SampleDescription(1, 0),
				IsWindowed = true
			};

			// try to create the device
			try
			{
				this.device = CreateDeviceWithSwapChain(
					DriverType.Hardware,
					FeatureLevel.Level_11_0,
					swapChainDescription,
					out swapChain,
					out commandQueue);

			}
			catch (SharpDXException)
			{
				device = CreateDeviceWithSwapChain(
					DriverType.Warp, FeatureLevel.Level_11_0,
					swapChainDescription,
					out swapChain,
					out commandQueue);
			}

			// create command queue and allocator objects
			commandListAllocator = device.CreateCommandAllocator(CommandListType.Direct);
		}
	
		/// <summary>
		/// Loads the assets required for the application
		/// </summary>
		private void LoadAssets()
		{
			// Create the descriptor heap for the render target view
			descriptorHeap = device.CreateDescriptorHeap(new DescriptorHeapDescription()
			{
				Type = DescriptorHeapType.RenderTargetView,
				DescriptorCount = 1
			});

			// Create the main command list
			commandList = device.CreateCommandList(CommandListType.Direct, commandListAllocator, null);

			// Get the backbuffer and create the render target view
			renderTarget = swapChain.GetBackBuffer<Resource>(0);
			device.CreateRenderTargetView(renderTarget, null, descriptorHeap.CPUDescriptorHandleForHeapStart);

			// Create the viewport
			viewPort = new ViewportF(0, 0, width, height);

			// Create the scissor
			scissorRectangle = new Rectangle(0, 0, width, height);

			// Create a fence to wait for next frame
			fence = device.CreateFence(0, FenceFlags.None);
			currentFence = 1;

			// Close command list
			commandList.Close();

			// Create an event handle for use with VTBL
			eventHandle = new AutoResetEvent(false);

			// Wait for the command list to complete
			WaitForPrevFrame();
		}

		/// <summary>
		/// Fill the command list with commands
		/// </summary>
		private void PopulateCommandList()
		{
			commandListAllocator.Reset();

			commandList.Reset(commandListAllocator, null);

			// setup viewport and scissors
			commandList.SetViewport(viewPort);
			commandList.SetScissorRectangles(scissorRectangle);

			// use barrier to notify that we are using the RenderTarget to clear it
			commandList.ResourceBarrierTransition(renderTarget, ResourceStates.Present, ResourceStates.RenderTarget);

			// Clear the RenderTarget
			var time = clock.Elapsed.TotalSeconds;
			commandList.ClearRenderTargetView(
				descriptorHeap.CPUDescriptorHandleForHeapStart,
				new Color4(
					(float)Math.Sin(time) * 0.25f + 0.5f, 
					(float)Math.Sin(time * 0.5f) * 0.4f + 0.6f, 
					(float)Math.Sin(time * 0.3f) * 0.2f, 
					1.0f),
				0,
				null);

			// Use barrier to notify that we are going to present the RenderTarget
			commandList.ResourceBarrierTransition(renderTarget, ResourceStates.RenderTarget, ResourceStates.Present);

			// Execute the command
			commandList.Close();
		}
		
		/// <summary>
		/// Wait for the previous command list to finish executing before continuing
		/// </summary>
		/// <remarks>
		/// This is not a best practice for DirectX. Need to research what the best practice is before continuing
		/// </remarks>
		private void WaitForPrevFrame()
		{
			var localFence = currentFence;
			commandQueue.Signal(fence, localFence);

			currentFence++;

			if(fence.CompletedValue < localFence)
			{
				fence.SetEventOnCompletion(localFence, eventHandle.SafeWaitHandle.DangerousGetHandle());
				eventHandle.WaitOne();
			}
		}
		
		/// <summary>
		/// Creates a new <see cref="Device"/> instance and updates the provided <see cref="SwapChain"/> and <see cref="CommandQueue"/> based
		/// on the provided configuration
		/// </summary>
		/// <param name="driverType"></param>
		/// <param name="featureLevel"></param>
		/// <param name="swapChainDescription"></param>
		/// <param name="swapChain"></param>
		/// <param name="commandQueue"></param>
		/// <returns>
		/// A <see cref="Device"/>
		/// </returns>
		private Device CreateDeviceWithSwapChain(
			DriverType driverType, 
			FeatureLevel featureLevel, 
			SwapChainDescription swapChainDescription, 
			out SwapChain swapChain, 
			out CommandQueue commandQueue)
		{
#if DEBUG
			// Enable the D3D12 debug layer
			// DebugInterface.Get().EnableDebugLayer();
#endif
			using (var factory = new Factory4())
			{
				var adapter = driverType == DriverType.Hardware ? null : factory.GetWarpAdapter();
				var device = new Device(adapter, featureLevel);

				commandQueue = device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));

				swapChain = new SwapChain(factory, commandQueue, swapChainDescription);

				return device;
			}
		}
	}
}
