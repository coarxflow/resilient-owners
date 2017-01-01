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

		public void Check(int newVal, ItemClass.SubService subService, bool extractor = false, bool litteral = true)
		{
            //CODebug.Log(LogChannel.Modding, Mod.modName + " - UIGoodsWrapper val " + newVal + " subService " + subService + " extractor " + extractor);
   //         if (this.m_Value != newVal)
			//{
				this.m_Value = newVal;
				switch(subService)
				{
					case ItemClass.SubService.IndustrialGeneric:
                        this.m_String = Localization.trad.GetIndustrialGenericGoodsDenomination(Mathf.RoundToInt(this.m_Value / 100f), litteral);
						break;
					case ItemClass.SubService.IndustrialFarming:
                        this.m_String = Localization.trad.GetIndustrialFarmingGoodsDenomination(Mathf.RoundToInt(this.m_Value / 2f), litteral);
						break;
					case ItemClass.SubService.IndustrialForestry:
						if(extractor)
                            this.m_String = Localization.trad.GetIndustrialForestryExtractorDenomination(Mathf.RoundToInt(this.m_Value / 200f), litteral);
						else
                            this.m_String = Localization.trad.GetIndustrialForestryGoodsDenomination(Mathf.RoundToInt(this.m_Value / 500f), litteral);
						break;
					case ItemClass.SubService.IndustrialOil:
						if(extractor)
							this.m_String = Localization.trad.GetIndustrialOilExtractorDenomination(this.m_Value / 8000f, litteral);
						else
                            this.m_String = Localization.trad.GetIndustrialOilGoodsDenomination(Mathf.RoundToInt(this.m_Value / 100f), litteral);
						break;
					case ItemClass.SubService.IndustrialOre:
						if(extractor)
							this.m_String = Localization.trad.GetIndustrialOreExtractorDenomination(this.m_Value / 400f, litteral);
						else
                            this.m_String = Localization.trad.GetIndustrialOreGoodsDenomination(Mathf.RoundToInt(this.m_Value / 800f), litteral);
						break;
				}
			}
		//}
	}
}

