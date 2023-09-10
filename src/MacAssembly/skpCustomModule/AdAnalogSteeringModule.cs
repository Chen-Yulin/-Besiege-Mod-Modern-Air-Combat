using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Modding.Modules;
using Modding.Serialization;

namespace skpCustomModule
{
	// Token: 0x02000028 RID: 40
	[XmlRoot("AdAnalogSteeringProp")]
	[Reloadable]
	public class AdAnalogSteeringModule : BlockModule
	{
		// Token: 0x060000CE RID: 206 RVA: 0x000118AC File Offset: 0x0000FAAC
		protected override bool Validate(string elemName)
		{
			bool flag = !base.Validate(elemName);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.LimitsDisplay == null;
				if (flag2)
				{
					result = base.MissingElement(elemName, "LimitsDisplay");
				}
				else
				{
					bool flag3 = !this.LimitsDefaultMinSpecified;
					if (flag3)
					{
						result = base.MissingElement(elemName, "LimitsDefaultMin");
					}
					else
					{
						bool flag4 = !this.LimitsDefaultMaxSpecified;
						if (flag4)
						{
							result = base.MissingElement(elemName, "LimitsDefaultMax");
						}
						else
						{
							bool flag5 = !this.LimitsHighestAngleSpecified;
							result = !flag5 || base.MissingElement(elemName, "LimitsHighestAngle");
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040001E2 RID: 482
		[XmlElement]
		[RequireToValidate]
		public MSliderReference XYAxisSlider;

		// Token: 0x040001E3 RID: 483
		[XmlElement]
		[RequireToValidate]
		public MSliderReference SpeedSlider;

		// Token: 0x040001E4 RID: 484
		[XmlElement]
		public Direction Axis;

		// Token: 0x040001E5 RID: 485
		[XmlElement]
		[Reloadable]
		public float MaxAngularSpeed = 100f;

		// Token: 0x040001E6 RID: 486
		[XmlElement]
		[DefaultValue(null)]
		[RequireToValidate]
		[Reloadable]
		public TransformValues LimitsDisplay;

		// Token: 0x040001E7 RID: 487
		[XmlElement]
		[DefaultValue(0)]
		public float LimitsDefaultMin;

		// Token: 0x040001E8 RID: 488
		[XmlIgnore]
		public bool LimitsDefaultMinSpecified;

		// Token: 0x040001E9 RID: 489
		[XmlElement]
		public float LimitsDefaultMax;

		// Token: 0x040001EA RID: 490
		[XmlIgnore]
		public bool LimitsDefaultMaxSpecified;

		// Token: 0x040001EB RID: 491
		[XmlElement]
		[Reloadable]
		public float LimitsHighestAngle;

		// Token: 0x040001EC RID: 492
		[XmlIgnore]
		[Reloadable]
		public bool LimitsHighestAngleSpecified;
	}
}
