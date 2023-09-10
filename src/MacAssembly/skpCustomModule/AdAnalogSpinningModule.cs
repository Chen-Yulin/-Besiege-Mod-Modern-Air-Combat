using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Modding.Modules;
using Modding.Serialization;

namespace skpCustomModule
{
	// Token: 0x02000027 RID: 39
	[XmlRoot("AdAnalogSpinningProp")]
	[Reloadable]
	public class AdAnalogSpinningModule : BlockModule
	{
		// Token: 0x040001D9 RID: 473
		[XmlElement]
		[RequireToValidate]
		public MKeyReference BrakeKey;

		// Token: 0x040001DA RID: 474
		[XmlElement]
		[RequireToValidate]
		public MKeyReference BackGearKey;

		// Token: 0x040001DB RID: 475
		[XmlElement]
		[RequireToValidate]
		public MSliderReference XYAxisSlider;

		// Token: 0x040001DC RID: 476
		[XmlElement]
		[RequireToValidate]
		public MSliderReference SpeedSlider;

		// Token: 0x040001DD RID: 477
		[XmlElement]
		[RequireToValidate]
		public MSliderReference AccelerationSlider;

		// Token: 0x040001DE RID: 478
		[XmlElement]
		[RequireToValidate]
		public MSliderReference DecelerationSlider;

		// Token: 0x040001DF RID: 479
		[XmlElement]
		[RequireToValidate]
		public MSliderReference BrakePowerSlider;

		// Token: 0x040001E0 RID: 480
		[XmlElement]
		[Reloadable]
		public Direction Axis;

		// Token: 0x040001E1 RID: 481
		[XmlElement]
		[DefaultValue(null)]
		[Reloadable]
		public float MaxAngularSpeed = 100f;
	}
}
