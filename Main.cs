using System;
using System.Text;

namespace IsingSpins
{
	class MainClass
	{			
		public static void Main (string[] args)
		{
			//Afficher un texte d'introduction
			Console.WriteLine ("Welcome to Hadrien's wondrous Ising lattice simulator !");
			Console.WriteLine ("For best visual effect, please maximize the console window.");
			Console.WriteLine ("\nPress any key to start the simulation...");
			Console.ReadKey ();
			
			//Lancer la simulation
			double EXCHANGE_COUPLING = 10;
			double THERMAL_ENERGY = 1;
			double EXTERNAL_FIELD = 0;
			int STABILITY_LIMIT = 10;
			int MAX_ITERATIONS = 10000;
			new IsingSimulation(EXCHANGE_COUPLING, THERMAL_ENERGY, EXTERNAL_FIELD, STABILITY_LIMIT, MAX_ITERATIONS);
		}
	}
}
