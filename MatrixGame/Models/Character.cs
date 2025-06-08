using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.Models
{
	public abstract class Character
	{
		
		public int Health { get; set; }
		public int Mana { get; set; }
		public int Damage { get; set; }
		public int Strength { get; set; }
		public int Agility { get; set; }
		public int Intelligence { get; set; }
		public int Range { get; set; }
		public abstract char Symbol { get; }
		public void SetUp()
		{
			Health = Strength * 5;
			Mana = Intelligence * 3;
			Damage = Agility * 2;
		}

	}
}
