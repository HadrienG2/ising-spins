using System;
using System.Text;

namespace IsingSpins
{
	class IsingLattice
	{
		public PeriodicSpinLattice.SpinDisplayUpdate SpinDisplayUpdater {
			get {
				return current_spins.SpinDisplayUpdater;
			}
			set	{
				current_spins.SpinDisplayUpdater = value;
			}
		}
		
		PeriodicSpinLattice last_spins;
		PeriodicSpinLattice current_spins;
		Random random_generator;
		int stability_turns = 0;
		
		//Compute and return the per-spin magnetization of the lattice
		public double AverageMagnetization ()
		{
			return current_spins.AverageMagnetization();
		}
		
		//Returns the energy per spin at a given position of the lattice: if multiplied by +1, gives
		//the energy associated to spin state +1, and if multiplied by -1, gives the energy associated
		//to spin state -1
		double ComputeEnergyPerSpin (PeriodicSpinLattice current_lattice, int h_pos, int v_pos)
		{
			//Measure nearest neighbour spins with periodic boundary conditions
			int left_spin = current_lattice [h_pos - 1, v_pos];
			int right_spin = current_lattice [h_pos + 1, v_pos];
			int top_spin = current_lattice [h_pos, v_pos - 1];
			int bottom_spin = current_lattice [h_pos, v_pos + 1];
			
			//Return the energy per spin associated to the requested spin state
			return -ExchangeCoupling * (left_spin + right_spin + top_spin + bottom_spin) - ExternalField;
		}
		
		//Compute the next spin state at a given position of the lattice, in a probabilistic fashion
		int ComputeFutureState (PeriodicSpinLattice current_lattice, int h_pos, int v_pos)
		{
			if (random_generator.NextDouble () < ComputeUpSpinProbability (current_lattice, h_pos, v_pos)) {
				return +1;
			} else {
				return -1;
			}
		}
		
		//Compute the probability of getting an up spin at a given position in a future state
		//that is given by Exp(-energy_up/ThermalEnergy)/(Exp(-energy_down/ThermalEnergy) + Exp(-energy_up/ThermalEnergy))
		//where energy_down = -energy_up
		double ComputeUpSpinProbability (PeriodicSpinLattice current_lattice, int h_pos, int v_pos)
		{
			if (ThermalEnergy != 0) {
				return 1 / (1 + Math.Exp (2 * ComputeEnergyPerSpin (current_lattice, h_pos, v_pos) / ThermalEnergy));
			} else {
				return (ComputeEnergyPerSpin (current_lattice, h_pos, v_pos) > 0) ? (0) : (1);
			}
		}
		
		//Display the full spin lattice on screen
		public void Display ()
		{
			for (int i=0; i < current_spins.Width; ++i) {
				for (int j=0; j < current_spins.Height; ++j) {
					SpinDisplayUpdater(i, j, current_spins[i, j]);
				}
			}
		}
		
		//Randomly compute all spins in a Monte Carlo fashion
		public void Iterate ()
		{
			//Save current spins as last spins
			last_spins.CopyFrom(current_spins);
			
			//Flip spins in random order
			int number_of_spins = current_spins.Width*current_spins.Height;
			for (int n=0; n < number_of_spins; ++n) {
				int i = random_generator.Next (current_spins.Width);
				int j = random_generator.Next (current_spins.Height);
				int previous_spin = current_spins[i, j];
				int new_spin = ComputeFutureState (current_spins, i, j);
				if(new_spin != previous_spin) {
					current_spins[i, j] = new_spin;
					SpinDisplayUpdater(i, j, current_spins[i, j]);
				}
			}
			
			//Check spin lattice for stability
			if(current_spins.Equals(last_spins)) {
				stability_turns+= 1;
			} else {
				stability_turns = 0;
			}
		}
		
		//Initialize the spin lattice to random values
		public void Randomize ()
		{
			current_spins.Randomize ();
			last_spins.Randomize ();
		}
		
		//Gestion de l'interaction d'échange
		double exchange_coupling;
		public double ExchangeCoupling {
			get {
				return exchange_coupling;
			}
			set {
				exchange_coupling = value;
				last_spins.Randomize ();
			}
		}
		
		//Gestion du champ extérieur d'un réseau de spin
		double external_field;
		public double ExternalField {
			get {
				return external_field;
			}
			set {
				external_field = value;
				last_spins.Randomize ();
			}
		}
		
		//Gestion de la limite de stabilité (nombre d'itérations sans changements)
		public double StabilityLimit;
		
		//Gestion de l'énergie thermique du réseau
		double thermal_energy;
		public double ThermalEnergy {
			get {
				return thermal_energy;
			}
			set {
				thermal_energy = value;
				last_spins.Randomize ();
			}
		}
		
		//Détecte si le réseau est stable (indicateur: pas de modification d'une itération à l'autre)
		public bool Steady ()
		{
			if(stability_turns > StabilityLimit) {
				return true;
			} else {
				return false;
			}
		}
		
		//Create a spin lattice of given width, height, exchange coupling and external field
		public IsingLattice (int p_width,
		                     int p_height,
		                     double p_ExchangeCoupling = 1,
		                     double p_ThermalEnergy = 0,
		                     double p_ExternalField = 0,
		                     int p_StabilityLimit = 1)
		{
			last_spins = new PeriodicSpinLattice (p_width, p_height);
			current_spins = new PeriodicSpinLattice (p_width, p_height);
			exchange_coupling = p_ExchangeCoupling;
			external_field = p_ExternalField;
			thermal_energy = p_ThermalEnergy;
			StabilityLimit = p_StabilityLimit;
			random_generator = new Random ();
			this.Randomize();
		}
	}
}
