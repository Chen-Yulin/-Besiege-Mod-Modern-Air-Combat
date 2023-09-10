using System;
using System.Xml.Serialization;
using Modding.Serialization;

namespace skpCustomModule
{
	// Token: 0x02000007 RID: 7
	[XmlRoot("joystickConfig")]
	public class AdData : Element
	{
		// Token: 0x0600001B RID: 27 RVA: 0x0000525C File Offset: 0x0000345C
		public AdData()
		{
			this.joystickNumber = "0";
			this.LStick_X = "0";
			this.LStick_Y = "1";
			this.RStick_X = "3";
			this.RStick_Y = "4";
			this.Pad_X = "5";
			this.Pad_Y = "6";
			this.LTriger = "8";
			this.RTriger = "9";
			this.Triger_min = "-1";
			this.Triger_max = "1";
			this.deadZone = "0.5";
			this.useJoystick = true;
			this.DS4trigermode = false;
		}

		// Token: 0x04000048 RID: 72
		[RequireToValidate]
		[CanBeEmpty]
		public string joystickNumber = "0";

		// Token: 0x04000049 RID: 73
		[RequireToValidate]
		[CanBeEmpty]
		public string LStick_X = "0";

		// Token: 0x0400004A RID: 74
		[RequireToValidate]
		[CanBeEmpty]
		public string LStick_Y = "0";

		// Token: 0x0400004B RID: 75
		[RequireToValidate]
		[CanBeEmpty]
		public string RStick_X = "0";

		// Token: 0x0400004C RID: 76
		[RequireToValidate]
		[CanBeEmpty]
		public string RStick_Y = "0";

		// Token: 0x0400004D RID: 77
		[RequireToValidate]
		[CanBeEmpty]
		public string Pad_X = "0";

		// Token: 0x0400004E RID: 78
		[RequireToValidate]
		[CanBeEmpty]
		public string Pad_Y = "0";

		// Token: 0x0400004F RID: 79
		[RequireToValidate]
		[CanBeEmpty]
		public string LTriger = "0";

		// Token: 0x04000050 RID: 80
		[RequireToValidate]
		[CanBeEmpty]
		public string RTriger = "0";

		// Token: 0x04000051 RID: 81
		[RequireToValidate]
		[CanBeEmpty]
		public string Triger_min = "0";

		// Token: 0x04000052 RID: 82
		[RequireToValidate]
		[CanBeEmpty]
		public string Triger_max = "0";

		// Token: 0x04000053 RID: 83
		[RequireToValidate]
		[CanBeEmpty]
		public string deadZone = "0";

		// Token: 0x04000054 RID: 84
		[RequireToValidate]
		[CanBeEmpty]
		public bool DS4trigermode = false;

		// Token: 0x04000055 RID: 85
		[RequireToValidate]
		[CanBeEmpty]
		public bool useJoystick = true;
	}
}
