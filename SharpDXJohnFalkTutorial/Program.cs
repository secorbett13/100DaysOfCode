using System;

namespace SharpDXJohnFalkTutorial
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			using (Game game = new Game())
			{
				game.Run();
			}
		}
	}
}
