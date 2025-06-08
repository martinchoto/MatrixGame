using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.Models
{
	public class Mage : Character
	{
		public override char Symbol => '*';
		public Mage()
		{
			Strength = 2;
			Agility = 1;
			Intelligence = 3;
			Range = 3;
		}

	}
}
