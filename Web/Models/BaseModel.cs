using System;

namespace Web.Models
{
	public abstract class BaseModel
	{
		public static int ModelCount = 0;
		public static object Lock = new object();
		public string UniqueID = DateTime.Now.ToString("mmssffffff") + "_" + ModelCount;

		public BaseModel()
		{
			lock (Lock)
			{
				if (ModelCount > 20000) ModelCount = 0;
				ModelCount++;
				this.UniqueID = DateTime.Now.ToString("mmssffffff") + "_" + ModelCount;
			}
		}
	}
}