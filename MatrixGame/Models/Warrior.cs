﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.Models
{
	public class Warrior : Character
	{
		public override char Symbol => '@';
		public Warrior()
		{
			Strength = 3;
			Agility = 3;
			Intelligence = 0;
			Range = 1;
		}
	}
}
