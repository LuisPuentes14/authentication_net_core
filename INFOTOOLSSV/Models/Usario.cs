using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace INFOTOOLSSV.Models
{
	public class Usario
	{
		public int Id_Usario { get; set; }
		public string? UserName { get; set; }

		public string? clave { get; set; }

		[NotMapped]
		public bool MantenerActivo { get; set; }
	}
}
