using System;
using System.Text;

namespace IsingSpins
{
	//Periodic lattice of spins taking +/-1 integer magnetizations
	public class PeriodicSpinLattice
	{
		public delegate void SpinDisplayUpdate(int h_pos, int v_pos, int new_spin);
		public SpinDisplayUpdate SpinDisplayUpdater;
		
		public readonly int Width, Height;
		int[,] spins;
		static Random random_generator = new Random();
		
		//Return the spin located at a given position of the periodized lattice
		public int this[int x, int y] {
			get
			{
				x = x % Width;
				if(x < 0) x+= Width;
				
				y = y % Height;
				if(y < 0) y+= Height;
				
				return spins[x, y];
			}
			set
			{
				x = x % Width;
				if(x < 0) x+= Width;
				
				y = y % Height;
				if(y < 0) y+= Height;
				
				int old_spin = spins[x, y];
				if(old_spin != value) {
					spins[x, y] = value;
					if(SpinDisplayUpdater != null) {
						SpinDisplayUpdater(x, y, spins[x, y]);
					}
				}
			}
		}
		
		//Compute and return per-spin magnetization of the lattice
		public double AverageMagnetization ()
		{
			double result = 0;
			
			for (int i=0; i < Width; ++i) {
				for (int j=0; j < Height; ++j) {
					result += this[i, j];
				}
			}
			result /= (Width * Height);
			
			return result;
		}
		
		//Copy the contents of one spin lattice into another spin lattice
		public bool CopyFrom(PeriodicSpinLattice reference)
		{
			//Make sure that the copy is actually possible
			if(reference.Width != Width) return false;
			if(reference.Height != Height) return false;
			
			//Perform the data copy
			for (int i=0; i < Width; ++i) {
				for (int j=0; j < Height; ++j) {
					this[i, j] = reference [i, j];
				}
			}
			
			return true;
		}
		
		//Check if two spin lattices have identical contents
		public bool Equals(PeriodicSpinLattice reference)
		{
			for (int i=0; i < Width; ++i) {
				for (int j=0; j < Height; ++j) {
					if (this[i, j] != reference[i, j])
					{
						return false;
					}
				}
			}
			
			return true;
		}
		
		//Initialize a spin lattice of specified Width and Height
		public PeriodicSpinLattice (int p_Width, int p_Height)
		{
			Width = p_Width;
			Height = p_Height;
			spins = new int[Width, Height];
		}
		
		//Randomize spin values
		public void Randomize() {
			for (int i=0; i < Width; ++i) {
				for (int j=0; j < Height; ++j) {
					this[i, j] = 2 * random_generator.Next (2) - 1;
				}
			}
		}
	}
}

