using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.Models
{
	public class Monster : Character
	{
		public override char Symbol => 'M';
		public Monster()
		{
			Strength = Random.Shared.Next(1, 4);
			Agility = Random.Shared.Next(1, 4);
			Intelligence = Random.Shared.Next(1, 4);
			Range = 1;
		}
	}
}
