using MatrixGame.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.Database.DbModels
{
	public class PlayableCharacter : Character
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Class { get; set; } = null!;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToLocalTime();
		[NotMapped]
		public override char Symbol => ' ';
	}
}
