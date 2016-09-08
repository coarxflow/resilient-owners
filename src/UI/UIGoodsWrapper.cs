using System;
using UnityEngine;
using ColossalFramework.Globalization;

namespace ResilientOwners
{
	public class UIGoodsWrapper : UIWrapper<int>
	{
		public UIGoodsWrapper(int def) : base(def)
		{
		}

		public override void Check(int newVal)
		{
			this.Check(newVal, ItemClass.SubService.IndustrialGeneric);
		}

		public void Check(int newVal, ItemClass.SubService subService, bool extractor = false)
		{
			if (this.m_Value != newVal)
			{
				this.m_Value = newVal;
				switch(subService)
				{
					case ItemClass.SubService.IndustrialGeneric:
						this.m_String = Mathf.RoundToInt(this.m_Value / 100f)+" goods";
						break;
					case ItemClass.SubService.IndustrialFarming:
						this.m_String = Mathf.RoundToInt(this.m_Value / 2f)+" kg";
						break;
					case ItemClass.SubService.IndustrialForestry:
						if(extractor)
							this.m_String = Mathf.RoundToInt(this.m_Value / 200f)+" logs";
						else
							this.m_String = Mathf.RoundToInt(this.m_Value / 500f)+" goods";
						break;
					case ItemClass.SubService.IndustrialOil:
						if(extractor)
							this.m_String = (this.m_Value / 8000f)+" barrils";
						else
							this.m_String = Mathf.RoundToInt(this.m_Value / 100f)+" goods";
						break;
					case ItemClass.SubService.IndustrialOre:
						if(extractor)
							this.m_String = (this.m_Value / 400f)+" tons";
						else
							this.m_String = (this.m_Value / 800f)+" steel bars";
						break;
				}
			}
		}
	}
}

