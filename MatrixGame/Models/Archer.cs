using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.Models
{
	public class Archer : Character
	{
		public override char Symbol => '#';
		public Archer()
		{
			Strength = 2;
			Agility = 4;
			Intelligence = 0;
			Range = 2;
		}
	}
}
