using System;
using System.Text;

namespace IsingSpins
{
	public class IsingSimulation
	{
		//Logical state of the simulation
		enum simulation_state {NULL, RUNNING, PAUSED, OVER};
		simulation_state current_state;
		
		//Current iteration number and spin lattice
		int iteration;
		IsingLattice lattice;
		
		void update_simulation_display()
		{
			StringBuilder output = new StringBuilder(Console.WindowWidth*Console.WindowHeight);
			
			//Prepare simulation state display
			output.Append("Simulation ");
			switch(current_state) {
			case simulation_state.RUNNING:
				output.Append("running, press 'p' to pause...");
				break;
			case simulation_state.PAUSED:
				output.Append("paused, press any key to resume.");
				break;
			case simulation_state.OVER:
				if(lattice.Steady()) {
					output.Append("reached a steady state!");
				} else {
					output.Append("timed out!");
				}
				break;
			}
			output.AppendFormat (" (Iteration {0}, <M>={1:+0.00000;-0.00000}):", iteration, lattice.AverageMagnetization());
			output.Append(' ', Console.WindowWidth-output.Length);
			
			//Commit display to screen
			Console.SetCursorPosition(0,0);
			Console.Write (output.ToString());
			Console.SetCursorPosition(Console.WindowWidth-1, 0);
		}
		
		void update_spin_display(int h_pos, int v_pos, int new_value)
		{
			Console.SetCursorPosition(h_pos, v_pos+1);
			if(new_value > 0) {
				Console.Write('+');
			} else {
				Console.Write('-');
			}
		}
		
		public IsingSimulation (double exchange_coupling,
		                        double thermal_energy,
		                        double external_field,
		                        int stability_limit,
		                        int max_iterations)
		{
			//Create the Ising lattice to be simulated
			lattice = new IsingLattice (Console.WindowWidth,
			                            Console.WindowHeight-2,
			                            exchange_coupling,
			                            thermal_energy,
			                            external_field,
			                            stability_limit);
			
			//Initialize lattice display
			Console.Clear();
			lattice.SpinDisplayUpdater+= update_spin_display;
			lattice.Display();
			Console.CursorVisible = false;
			
			//Iterate the simulation, keeping the display up to date
			current_state = simulation_state.RUNNING;
			for (iteration=0; (iteration<max_iterations) && (!lattice.Steady()); ++iteration) {
				update_simulation_display();
				lattice.Iterate ();
				
				//Pause simulation when the 'p' key is pressed
				if(Console.KeyAvailable) {
					switch(char.ToLower(Console.ReadKey().KeyChar))
					{
					case 'p':
						current_state = simulation_state.PAUSED;
						update_simulation_display();
						Console.ReadKey();
						current_state = simulation_state.RUNNING;
						break;
					}
				}
			}
			
			//Conclude when simulation is over
			current_state = simulation_state.OVER;
			update_simulation_display();
			Console.SetCursorPosition(Console.WindowWidth-1, Console.WindowHeight-2);
		}
	}
}

