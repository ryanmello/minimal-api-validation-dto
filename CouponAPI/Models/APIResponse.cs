using System.Net;

namespace CouponAPI.Models
{
	public class APIResponse
	{
		public bool IsSucess { get; set; }
		public Object? Result { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public List<string> ErrorMessages { get; set; } = new List<string>();
	}
}
